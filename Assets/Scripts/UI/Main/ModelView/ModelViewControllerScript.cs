using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using Cysharp.Threading.Tasks;

public class ModelViewControllerScript : MonoBehaviour
{
    [SerializeField] Camera CameraMain;
    [SerializeField] Camera CameraUI;
    ModelFitToContainerScript _fitContainerScript;
    ModelRotateScript _rotateScript;
    public ModelFitToContainerScript FitContainerScript { get { return _fitContainerScript; } }
    public ModelRotateScript RotateScript { get { return _rotateScript; } }
    void Start()
    {
        _fitContainerScript = GetComponent<ModelFitToContainerScript>();
        _rotateScript = GetComponent<ModelRotateScript>();
    }
    private void OnEnable()
    {
        CameraMain.enabled = false;
        CameraUI.enabled = true;
    }
    private void OnDisable()
    {
        if (CameraMain == null || CameraUI == null) return;
        CameraMain.enabled = true;
        CameraUI.enabled = false;
    }
    public void SetModel(byte[] modelData)
    {
        DestroyCurrentModel();
        _rotateScript.ResetPosition();
        var GO = Importer.LoadFromBytes(modelData);
        GO.transform.parent = transform;
        SetLayerRecursively(GO, gameObject.layer);
        UniTask.Create( async () =>
        {
            await UniTask.NextFrame();
            _fitContainerScript.FitToContainer();
            return UniTask.CompletedTask;
        });
        //_fitContainerScript.FitToContainer();
        GO.transform.localPosition = new Vector3(0f,0f,0f);
        UniTask.Create(async () =>
        {
            await UniTask.NextFrame();
            transform.Rotate(0, 180f, 0, Space.Self);
            return UniTask.CompletedTask;
        });
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
