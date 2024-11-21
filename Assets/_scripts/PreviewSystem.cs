using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;
    //private float currentRotation = 0f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;    
    private Vector2Int currentObjectSize;


    private void Start(){
        cellIndicator.SetActive(false);
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    // public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size){
    //     previewObject = Instantiate(prefab);
    //     PreparePreview(previewObject);
    //     PrepareCursor(size);
    //     cellIndicator.SetActive(true);
    // }

    // public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    // {
    //     previewObject = Instantiate(prefab);
    //     currentObjectSize = size;
    //     PreparePreview(previewObject);
    //     PrepareCursor(size);
    //     cellIndicator.SetActive(true);
    // }
    
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        currentObjectSize = size; // Almacenar el tamaño
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    public void StartShowingRemovePreview(){
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    public void PrepareCursor(Vector2Int size){
        if(size.x > 0 || size.y > 0){
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = size;
        }
        // para el commit
    }
    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }
    public void StopShowingPreview(){
        cellIndicator.SetActive(false);
        if(previewObject != null)
            Destroy(previewObject);

    }
    public void UpdatePosition(Vector3 position, bool validity){
        if(previewObject != null){
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }
    public void ApplyFeedbackToPreview(bool validity){
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
        
    }
    public void ApplyFeedbackToCursor(bool validity){
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
        
    }
    private void MoveCursor(Vector3 position){
        cellIndicator.transform.position = position;
    }

    // private void MovePreview(Vector3 position){
    //     previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    // }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }
    public void RotatePreview(float rotationY)
    {
        if (previewObject != null)
        {
            // Rotar alrededor del centro del objeto
            previewObject.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    public Vector3 GetCurrentPosition()
    {
        return previewObject?.transform.position ?? Vector3.zero;
    }

    public Vector2Int GetCurrentObjectSize()
    {
        return currentObjectSize;
    }
}
