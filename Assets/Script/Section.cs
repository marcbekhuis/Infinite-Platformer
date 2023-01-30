using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Section : MonoBehaviour
{
    public Transform start;
    public Transform end;
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
        if (Application.isPlaying)
            return;

        if(oldStartHeight != start.position.y)
            UpdateStart();
        if(oldLength != end.position.x)
            UpdateEnd();
        if (oldEndHeight != end.position.y)
            UpdateEnd();
        if(oldGroundLayer != groundLayer)
        {
            UpdateStart();
            UpdateEnd();
        }
    }
    private void UpdateStart()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(0, start.position.y + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(0, start.position.y + 1, 0), hits[0].point);
            start.position = hits[0].point;
        }
        oldStartHeight = start.position.y;
        oldGroundLayer = groundLayer;
    }

    private void UpdateEnd()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(end.position.x + 2, end.position.y - 0.5f, 0), Vector3.left, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(end.position.x + 2, end.position.y - 0.5f, 0), hits[0].point, Color.yellow, 20);
            end.position = new Vector3(hits[0].point.x - endMargin, end.position.y, 0);
        }
        hits = Physics.RaycastAll(new Vector3(end.position.x - 0.2f, end.position.y + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            Debug.DrawLine(new Vector3(end.position.x - 0.2f, end.position.y + 1, 0), hits[0].point, Color.yellow, 20);
            end.position = new Vector3(end.position.x, hits[0].point.y, 0);
        }
        oldEndHeight = end.position.y;
        oldLength = end.position.x;
        oldGroundLayer = groundLayer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(start.position, 0.1f);
        Gizmos.DrawSphere(end.position, 0.1f);
    }
#endif
}
