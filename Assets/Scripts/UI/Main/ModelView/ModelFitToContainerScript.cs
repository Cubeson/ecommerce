using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ModelFitToContainerScript : MonoBehaviour
{
    private MeshRenderer _containerMeshRenderer;

    void Start()
    {
        _containerMeshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void FitToContainer() {
        if (gameObject.transform.childCount < 0) throw new System.Exception("Attempted to resize mesh, but Container GameObject has no children");
        Transform TransformToScale = gameObject.transform.GetChild(0).transform;
        Vector3 referenceSize = _containerMeshRenderer.bounds.size;
        List<MeshRenderer> meshes = new();
        FindAndStoreComponentsFromChildren(TransformToScale, meshes);
        Vector3 furthest = GetFurthestBounds(meshes);

        float resizeX = referenceSize.x / furthest.x;
        float resizeY = referenceSize.y / furthest.y;
        float resizeZ = referenceSize.z / furthest.z;

        float[] arr = { resizeX, resizeY, resizeZ };
        var min = arr.Min();

        TransformToScale.localScale = new Vector3(  TransformToScale.localScale.x * min,
                                                    TransformToScale.localScale.y * min,
                                                    TransformToScale.localScale.z * min);
    }
    private void FindAndStoreComponentsFromChildren<T>(Transform parent, ICollection<T> container)
    {
        foreach (Transform child in parent)
        {
            T component;
            var ok = child.gameObject.TryGetComponent<T>(out component);
            if (ok) container.Add(component);
            if (child.childCount > 0) FindAndStoreComponentsFromChildren(child, container);
        }
    }
    private Vector3 GetFurthestBounds(ICollection<MeshRenderer> meshRenderers)
    {
        Vector3 furthest = Vector3.zero;
        foreach (var meshRenderer in meshRenderers)
        {
            var x = meshRenderer.bounds.size.x;
            var y = meshRenderer.bounds.size.y;
            var z = meshRenderer.bounds.size.z;
            if (x > furthest.x) furthest.x = x;
            if (y > furthest.y) furthest.y = y;
            if (z > furthest.z) furthest.z = z;
        }
        return furthest;
    }
}
