using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Section : MonoBehaviour
{
    public float startHeight;
    public float endHeight;
    public float length;
    public float minBiomeSpawnDistance;
    public float maxBiomeSpawnDistance;

#if UNITY_EDITOR
    [Header("Editor Tools")]
    public LayerMask groundLayer;
    public float endMargin = 0.04f;

    private float oldLength;
    private float oldStartHeight;
    private float oldEndHeight;
    private LayerMask oldGroundLayer;

    private void Update()
    {
        if(oldStartHeight != startHeight)
            UpdateStart();
        if(oldLength != length)
            UpdateEnd();
        if (oldEndHeight != endHeight)
            UpdateEnd();
        if(oldGroundLayer != groundLayer)
        {
            UpdateStart();
            UpdateEnd();
        }
    }
    private void UpdateStart()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(0, startHeight + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(0, startHeight + 1, 0), hits[0].point);
            startHeight = hits[0].point.y;
        }
        oldStartHeight = startHeight;
        oldGroundLayer = groundLayer;
    }

    private void UpdateEnd()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(length + 2, endHeight - 0.5f, 0), Vector3.left, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(length + 2, endHeight - 0.5f, 0), hits[0].point, Color.yellow, 20);
            length = hits[0].point.x - endMargin;
        }
        hits = Physics.RaycastAll(new Vector3(length - 0.2f, endHeight + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(length - 0.2f, endHeight + 1, 0), hits[0].point, Color.yellow, 20);
            endHeight = hits[0].point.y;
        }
        oldEndHeight = endHeight;
        oldLength = length;
        oldGroundLayer = groundLayer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(0,startHeight, 0), 0.1f);
        Gizmos.DrawSphere(new Vector3(length, endHeight, 0), 0.1f);
    }
#endif
}
