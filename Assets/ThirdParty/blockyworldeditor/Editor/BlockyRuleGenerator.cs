using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static PeartreeGames.Blocky2DEditor.Editor.PrefabHelper;
using static UnityEngine.GraphicsBuffer;

namespace PeartreeGames.Blocky2DEditor.Editor
{
    public static class PrefabHelper
    {
        /// <summary>
        /// shortcut to loop trough children of object
        /// </summary>
        /// <param name="gameObject">Parent object</param>
        /// <param name="action">Action to perform on each child</param>
        public static void ForEach(this GameObject gameObject, Action<GameObject> action)
        {
            foreach (Transform item in gameObject.transform)
            {
                action.Invoke(item.gameObject);
            }
        }

        /// <summary>
        /// Loop recursively trough all the children of the object
        /// </summary>
        /// <param name="gameObject">Parent object</param>
        /// <param name="action">Action to perform on each child</param>
        public static void RecursiveForEach(this GameObject gameObject, RecursiveAction<GameObject> action)
        {
            if (action.Invoke(gameObject, 0) == RecursiveBreaker.Continue)
            {
                RecursiveForEachOnChildren(gameObject, action, 1);
            }
        }

        private static RecursiveBreaker RecursiveForEachOnChildren(this GameObject gameObject, RecursiveAction<GameObject> action, int depth)
        {
            foreach (Transform item in gameObject.transform)
            {
                switch (action.Invoke(item.gameObject, depth))
                {
                    case RecursiveBreaker.Stop:
                        return RecursiveBreaker.Stop;

                    case RecursiveBreaker.Continue:
                        if (RecursiveForEachOnChildren(item.gameObject, action, depth + 1) == RecursiveBreaker.Stop)
                        {
                            return RecursiveBreaker.Stop;
                        }
                        break;
                }
            }

            return RecursiveBreaker.Continue;
        }

        /// <param name="item">current item</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>Recursion break type</returns>
        public delegate RecursiveBreaker RecursiveAction<T>(T item, int depth);

        /// <summary>
        /// Enum that allows you to control the breaking of recursive functions
        /// </summary>
        public enum RecursiveBreaker : byte
        {
            /// <summary>
            /// Stop the loop
            /// </summary>
            Stop,
            /// <summary>
            /// Skip and go to the next branch
            /// </summary>
            Skip,
            /// <summary>
            /// Continue searching into the branch
            /// </summary>
            Continue
        }
    }

    public class BlockyRuleGenerator : EditorWindow
    {
        public GameObject m_ModelCollection { get; set; }

        [MenuItem("Tools/Blocky/BlockyRuleGenerator")]
        private static void ShowWindow()
        {
            var window = GetWindow<BlockyRuleGenerator>();
            window.titleContent = new GUIContent("BlockyRuleGenerator");
            window.Show();
        }

        void OnGUI()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            m_ModelCollection = (GameObject)EditorGUILayout.ObjectField(m_ModelCollection, typeof(GameObject));
#pragma warning restore CS0618 // Type or member is obsolete

            if (GUILayout.Button("Generate"))
            {
                var path = EditorUtility.SaveFilePanel("Save as...", Application.dataPath, m_ModelCollection.name + "_BlockRule", "asset");

                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetRelativePath(Application.dataPath + "/..", path);
                    //path = AssetDatabase.GenerateUniqueAssetPath(path);
                    var ruleset = Instantiate(CreateInstance<BlockyRuleSet>());

                    AssetDatabase.CreateAsset(ruleset, path);
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    var prefabPath = $"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}";

                    m_ModelCollection.RecursiveForEach((gameObject, depth) =>
                    {
                        var match = Regex.Match(gameObject.name, "[x01]{3}_[x01]{3}_[x01]{3}");
                        if (match.Success)
                        {
                            var blockyRule = new BlockyRule();
                            blockyRule.block = PrefabUtility.SaveAsPrefabAsset(GameObject.Instantiate(gameObject), $"{prefabPath}/{gameObject.name}.prefab");
                            int c = 0;
                            for (int i = 0; i < match.Value.Length; i++)
                            {
                                switch (match.Value[i])
                                {
                                    case '0':
                                        blockyRule.grid[c++] = BlockyCellRule.Diff;
                                        break;

                                    case '1':
                                        blockyRule.grid[c++] = BlockyCellRule.Same;
                                        break;

                                    case 'x':
                                        blockyRule.grid[c++] = BlockyCellRule.None;
                                        break;
                                }
                            }
                            ruleset.rules.Add(blockyRule);
                            return RecursiveBreaker.Skip;
                        }
                        return RecursiveBreaker.Continue;
                    });

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = ruleset;
                }
            }
        }
    }
}
