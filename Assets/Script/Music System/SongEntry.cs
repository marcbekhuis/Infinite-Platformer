using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Song", menuName = "Music/Song")]
public class SongEntry : ScriptableObject
{
    public AudioClip song;
    public AnimationCurve startFade;
    public AnimationCurve endFade;
    public float startOffset;
    public bool loopAble;

}
