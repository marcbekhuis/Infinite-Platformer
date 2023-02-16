using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, start.localPosition.y + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0];
            foreach (var hit in hits)
            {
                if (Vector3.Distance(hit.point, end.position) < Vector3.Distance(closestHit.point, end.position))
                    closestHit = hit;
            }
            Debug.DrawLine(transform.position + new Vector3(0, start.localPosition.y + 1, 0), closestHit.point);
            start.position = closestHit.point;
        }
        oldStartHeight = start.localPosition.y;
        oldGroundLayer = groundLayer;
    }

    private void UpdateEnd()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(end.localPosition.x + 2, end.localPosition.y - 0.5f, 0), Vector3.left, 5, groundLayer);
        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0];
            foreach (var hit in hits)
            {
                if (Vector3.Distance(hit.point, end.position) < Vector3.Distance(closestHit.point, end.position))
                    closestHit = hit;
            }
            Debug.DrawLine(new Vector3(end.localPosition.x + 2, end.position.y - 0.5f, 0), closestHit.point, Color.yellow, 20);
            end.position = new Vector3(closestHit.point.x - endMargin, end.position.y, 0);
        }

        hits = Physics.RaycastAll(transform.position + new Vector3(end.localPosition.x - 0.2f, end.localPosition.y + 1, 0), Vector3.down, 5, groundLayer);
        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0];
            foreach (var hit in hits)
            {
                if (Vector3.Distance(hit.point, end.position) < Vector3.Distance(closestHit.point, end.position))
                    closestHit = hit;
            }
            Debug.DrawLine(transform.position + new Vector3(end.localPosition.x - 0.2f, end.localPosition.y + 1, 0), closestHit.point, Color.yellow, 20);
            end.position = new Vector3(end.position.x, closestHit.point.y, 0);
        }
        oldEndHeight = end.localPosition.y;
        oldLength = end.localPosition.x;
        oldGroundLayer = groundLayer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + start.localPosition, 0.1f);
        Gizmos.DrawSphere(transform.position + end.localPosition, 0.1f);
    }
#endif
}
