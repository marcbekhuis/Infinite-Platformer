using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SegmentCombiner : MonoBehaviour
{
    [SerializeField] private Vector3Int size;
    [SerializeField] private bool update = false;
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
        List<CombineInstance> floorCombine = new List<CombineInstance>();
        List<CombineInstance> bottomCombine = new List<CombineInstance>();

        Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        for (int y = 0; y < size.y; y++)
        {
            for (int z = 0; z < size.z; z++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    FloorPiece(x, y, z, floorCombine, rotation);
                    BottomPiece(x, y, z, bottomCombine, rotation);
                }
            }
        }

        if (floorCombine.Count > 0 && floorMeshFilter)
        {
            Mesh floor = new Mesh();
            floor.CombineMeshes(floorCombine.ToArray());
            floorMeshFilter.sharedMesh = floor;
        }
        if (bottomCombine.Count > 0 && bottomMeshFilter)
        {
            Mesh bottom = new Mesh();
            bottom.CombineMeshes(bottomCombine.ToArray());
            bottomMeshFilter.sharedMesh = bottom;
        }
    }

    private void FloorPiece(int x, int y, int z, List<CombineInstance> floorCombine, Quaternion rotation)
    {
        if (y != 0)
            return;

        Mesh segmentPrefab = floorCenterSegment;
        Vector3 scale = Vector3.one;

            if (x == 0 || x == size.x - 1)
                segmentPrefab = z == 0 ? floorEdgeSegment : floorEdgeBackSegment;
            else
                segmentPrefab = z == 0 ? floorCenterSegment : floorCenterBackSegment;

        if (x == size.x - 1)
            scale = new Vector3(-1, 1, 1);

        if (segmentPrefab)
        {
            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = segmentPrefab;
            Matrix4x4 matrix4X4 = new Matrix4x4();
            matrix4X4.SetTRS(new Vector3(x, y, z), rotation, scale);
            combineInstance.transform = matrix4X4;

            floorCombine.Add(combineInstance);
        }
    }

    private void BottomPiece(int x, int y, int z, List<CombineInstance> bottomCombine, Quaternion rotation)
    {
        Mesh segmentPrefab = floorCenterSegment;
        Vector3 scale = Vector3.one;

        if (x == 0 || x == size.x - 1)
            segmentPrefab = z == 0 ? bottomEdgeSegment : bottomEdgeBackSegment;
        else
            segmentPrefab = z == 0 ? bottomCenterSegment : null;

        if (x == size.x - 1)
            scale = new Vector3(-1, 1, 1);

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
    #endregion

    private void UpdateCollider()
    {
        if (floorCollider && floorMeshFilter)
        {
            floorCollider.center = floorMeshFilter.mesh.bounds.center;
            floorCollider.size = floorMeshFilter.mesh.bounds.size;
        }

        if (bottomCollider && bottomMeshFilter)
        {
            bottomCollider.center = bottomMeshFilter.mesh.bounds.center;
            bottomCollider.size = bottomMeshFilter.mesh.bounds.size;
        }
    }
}
