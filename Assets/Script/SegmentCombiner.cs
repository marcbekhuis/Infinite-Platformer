using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class SegmentCombiner : MonoBehaviour
{
    [SerializeField] private Vector3Int size;
    [SerializeField] private bool update = false;
    [SerializeField] private bool floor = true;
    [SerializeField] private bool floorLeftEdge = true;
    [SerializeField] private bool floorRightEdge = true;
    [SerializeField] private bool bottomLeftEdge = true;
    [SerializeField] private bool bottomRightEdge = true;
    [SerializeField] private string path;
    [SerializeField] private string fileName;
    [Header("Mesh filters")]
    [SerializeField] private MeshFilter floorMeshFilter;
    [SerializeField] private MeshFilter bottomMeshFilter;
    [Header("Colliders")]
    [SerializeField] private BoxCollider floorCollider;
    [SerializeField] private BoxCollider bottomCollider;
    [Header("Floor Prefabs")]
    [SerializeField] private Mesh floorEdgeSegment;
    [SerializeField] private Mesh floorEdgeBackSegment;
    [SerializeField] private Mesh floorCenterSegment;
    [SerializeField] private Mesh floorCenterBackSegment;
    [Header("Bottom Prefabs")]
    [SerializeField] private Mesh bottomEdgeSegment;
    [SerializeField] private Mesh bottomEdgeBackSegment;
    [SerializeField] private Mesh bottomCenterSegment;

    private void Update()
    {
        if (!update)
            return;

        update = false;

        UpdateVisual();
        UpdateCollider();
    }

    #region Visual
    private void UpdateVisual()
    {
        string systemPath = $"{Application.dataPath}/{path}";

        Directory.CreateDirectory(systemPath);

        FloorPiece();
        BottomPiece(); 
    }

    private void FloorPiece()
    {
        string floorPath = $"Assets/{path}/{fileName}_floor_{size.x}x{size.z}_{floorRightEdge}_{floorLeftEdge}.asset";

        Mesh floorMesh = AssetDatabase.LoadAssetAtPath<Mesh>(floorPath);
        if (floorMesh)
        {
            floorMeshFilter.sharedMesh = floorMesh;
            return;
        }
        if (!floor)
        {
            floorMeshFilter.sharedMesh = null;
        }

        List<CombineInstance> floorCombine = new List<CombineInstance>();
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Mesh segmentPrefab = floorCenterSegment;
        Vector3 scale = Vector3.one;

        for (int x = 0; x < size.x; x++)
        {
            if (x == size.x - 1)
                scale = new Vector3(-1, 1, 1);
            for (int z = 0; z < size.z; z++)
            {

                if (x == 0 && floorLeftEdge)
                    segmentPrefab = z == 0 ? floorEdgeSegment : floorEdgeBackSegment;
                else if (x == size.x - 1 && floorRightEdge)
                    segmentPrefab = z == 0 ? floorEdgeSegment : floorEdgeBackSegment;
                else
                    segmentPrefab = z == 0 ? floorCenterSegment : floorCenterBackSegment;

                if (segmentPrefab)
                {
                    CombineInstance combineInstance = new CombineInstance();
                    combineInstance.mesh = segmentPrefab;
                    Matrix4x4 matrix4X4 = new Matrix4x4();
                    matrix4X4.SetTRS(new Vector3(x, 0, z), rotation, scale);
                    combineInstance.transform = matrix4X4;

                    floorCombine.Add(combineInstance);
                }
            }
        }

        if (floorCombine.Count > 0)
        {
            Mesh floor = new Mesh();
            floor.CombineMeshes(floorCombine.ToArray());
            AssetDatabase.CreateAsset(floor, floorPath);
            AssetDatabase.SaveAssets();
            Mesh savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(floorPath);
            floorMeshFilter.sharedMesh = savedMesh;
        }
        else
        {
            floorMeshFilter.sharedMesh = null;
        }
    }

    private void BottomPiece()
    {
        string bottomPath = $"Assets/{path}/{fileName}_bottom_{size.x}x{size.y}x{size.z}_{bottomRightEdge}_{bottomLeftEdge}.asset";
        Mesh bottomMesh = AssetDatabase.LoadAssetAtPath<Mesh>(bottomPath);
        if (bottomMesh)
        {
            bottomMeshFilter.sharedMesh = bottomMesh;
            return;
        }

        List<CombineInstance> bottomCombine = new List<CombineInstance>();
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Mesh segmentPrefab = floorCenterSegment;
        Vector3 scale = Vector3.one;

        for (int x = 0; x < size.x; x++)
        {
            if (x == size.x - 1)
                scale = new Vector3(-1, 1, 1);
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {

                    if (x == 0 && bottomLeftEdge)
                        segmentPrefab = z == 0 ? bottomEdgeSegment : bottomEdgeBackSegment;
                    else if (x == size.x - 1 && bottomRightEdge)
                        segmentPrefab = z == 0 ? bottomEdgeSegment : bottomEdgeBackSegment;
                    else
                        segmentPrefab = z == 0 ? bottomCenterSegment : null;

                    if (segmentPrefab)
                    {
                        CombineInstance combineInstance = new CombineInstance();
                        combineInstance.mesh = segmentPrefab;
                        Matrix4x4 matrix4X4 = new Matrix4x4();
                        matrix4X4.SetTRS(new Vector3(x, -y, z), rotation, scale);
                        combineInstance.transform = matrix4X4;

                        bottomCombine.Add(combineInstance);
                    }
                }
            }
        }

        if (bottomCombine.Count > 0)
        {
            Mesh bottom = new Mesh();
            bottom.CombineMeshes(bottomCombine.ToArray());
            AssetDatabase.CreateAsset(bottom, bottomPath);
            AssetDatabase.SaveAssets();
            Mesh savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(bottomPath);
            bottomMeshFilter.sharedMesh = savedMesh;
        }
        else
        {
            bottomMeshFilter.sharedMesh = null;
        }
    }
    #endregion

    private void UpdateCollider()
    {
        if (floorCollider && floorMeshFilter)
        {
            floorCollider.gameObject.SetActive(floor);
            if (floor)
            {
                floorCollider.center = floorMeshFilter.sharedMesh.bounds.center;
                floorCollider.size = floorMeshFilter.sharedMesh.bounds.size;
            }
        }

        if (bottomCollider && bottomMeshFilter)
        {
            bottomCollider.center = bottomMeshFilter.sharedMesh.bounds.center;
            bottomCollider.size = bottomMeshFilter.sharedMesh.bounds.size;
        }
    }
}
