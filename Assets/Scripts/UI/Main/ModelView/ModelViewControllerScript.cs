using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
public class ModelViewControllerScript : MonoBehaviour
{
    ModelFitToContainerScript _fitContainerScript;
    ModelRotateScript _rotateScript;
    public ModelFitToContainerScript FitContainerScript { get { return _fitContainerScript; } }
    public ModelRotateScript RotateScript { get { return _rotateScript; } }
    void Start()
    {
        _fitContainerScript = GetComponent<ModelFitToContainerScript>();
        _rotateScript = GetComponent<ModelRotateScript>();
    }
    public void SetModel(byte[] modelData)
    {
        DestroyCurrentModel();
        var GO = Importer.LoadFromBytes(modelData);
        GO.transform.parent = transform;
        SetLayerRecursively(GO, gameObject.layer);
        _fitContainerScript.FitToContainer();
        GO.transform.localPosition = new Vector3(0f,-0.5f,0f);
    }
    public void DestroyCurrentModel()
    {
        if (gameObject.transform.childCount != 1) return;
        var GO = gameObject.transform.GetChild(0);
        Destroy(GO.gameObject);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject,layer);
        }
    }
}
