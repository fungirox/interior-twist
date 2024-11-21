using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;
    public event Action OnClicked, OnExit;
    public event Action<Vector3> OnRightClicked;

    [SerializeField]
    public GameObject pnPause;
    public GameObject pnFloors;
    public GameObject pnChairs;
    public GameObject pnBeds;
    public GameObject pnTables;



    private void Update(){
        if (Input.GetMouseButtonDown(0)){
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            pnFloors.SetActive(false);
            pnChairs.SetActive(false);
            OnExit?.Invoke();
        }
        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            OnRightClicked?.Invoke(GetSelectedMapPosition());
        }

        if (Input.GetKey(KeyCode.Tab)){
            OnExit?.Invoke();
            pnPause.SetActive(true);
        }
    }
 
    public void activatePanel(GameObject panel){
        pnFloors.SetActive(false);
        pnChairs.SetActive(false);
        pnBeds.SetActive(false);
        pnTables.SetActive(false);

        panel.SetActive(true);
        OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}