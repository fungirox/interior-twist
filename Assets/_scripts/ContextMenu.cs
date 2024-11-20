using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button moveButton;
    [SerializeField]
    private Button deleteButton;
    
    private Vector3Int selectedPosition;
    private PlacementSystem placementSystem;

    private void Awake()
    {
        
    }

    private void Start()
    {
        placementSystem = FindObjectOfType<PlacementSystem>();
        menuPanel.SetActive(false);
        
        moveButton.onClick.AddListener(() => {
            placementSystem.StartReplacementModeForPosition(selectedPosition);
            HideMenu();
        });
        
        deleteButton.onClick.AddListener(() => {
            placementSystem.RemoveObjectAtPosition(selectedPosition);
            HideMenu();
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HideMenu();
        }
    }


    public void ShowAtPosition(Vector3 worldPosition, Vector3Int gridPosition)
    {
        selectedPosition = gridPosition;

        // Posición del mouse en coordenadas de pantalla
        Vector2 screenPosition = Input.mousePosition;

        // Ajusta la posición para no salir de los límites de la pantalla
        float xOffset = 20; // Ajusta según el tamaño de tu menú
        float yOffset = -50;  // Ajusta según el tamaño de tu menú

        screenPosition.x = Mathf.Clamp(screenPosition.x + xOffset, 0, Screen.width - xOffset);
        screenPosition.y = Mathf.Clamp(screenPosition.y + yOffset, 0, Screen.height - yOffset);

        // Aplica la posición ajustada al menú
        menuPanel.transform.position = screenPosition;
        menuPanel.SetActive(true);
    }


    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }
}