using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSystem : MonoBehaviour
{

     public Image radialImage;  // La imagen radial
     public float totalTime = 60f;  // Tiempo total para que la imagen se vacíe
     private float currentTime;  // Tiempo actual del contador

     public GameObject pnPause;
     public GameObject pnGame;

     [SerializeField]
     private InputManager inputManager;
     [SerializeField]
     private Grid grid;
     [SerializeField]
     private ObjectsDatabaseSO database;
     [SerializeField]
     private GameObject gridVisualization; 
     private GridData floorData, furnitureData;
     [SerializeField]
     private ObjectPlacer objectPlacer;
     [SerializeField]
     private PreviewSystem preview;
     private Vector3Int lastDetectedPosition = Vector3Int.zero;
     IBuildingState buildingState;

     [SerializeField]
     private ContextMenu activeContextMenu; // Arrástralo desde la escena en el Inspector

     [SerializeField]
     private SaveSystem saveSystem;

     [SerializeField]
     private AudioSource audioSource;

     [SerializeField]
     private AudioClip placeSound, removeSound, wrongPlacementSound;

     private void Start()
     {
          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();
          saveSystem.Initialize(floorData,furnitureData,grid);
          inputManager.OnRightClicked += HandleRightClick;
          audioSource = GetComponent<AudioSource>();

     }

     private void Update(){

          if (Input.GetKeyDown(KeyCode.R))
          {
               if (buildingState is IRotatable rotatableState)
               {
                    rotatableState.Rotate();
               }
          }

          // Guardar con S
          if (Input.GetKeyDown(KeyCode.S))
          {
               saveSystem?.SaveScene();
          }
          
          // Cargar con L
          if (Input.GetKeyDown(KeyCode.L))
          {
               saveSystem?.LoadScene();
          }

          if(buildingState == null){
               return;
          }
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          if(lastDetectedPosition != gridPosition){
               buildingState.UpdateState(gridPosition);
               lastDetectedPosition = gridPosition;
          } 
     }

     private void HandleRightClick(Vector3 mousePosition)
     {
          Vector3Int gridPosition = grid.WorldToCell(mousePosition);
          
          if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) || 
               !floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
          {
               activeContextMenu.ShowAtPosition(mousePosition, gridPosition);
          }
     }

     public void RemoveObjectAtPosition(Vector3Int gridPosition)
     {
          GridData selectedData = null;
          int gameObjectIndex = -1;

          if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
          {
               selectedData = furnitureData;
          }
          else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
          {
               selectedData = floorData;
          }

          if (selectedData != null)
          {
               gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
               if (gameObjectIndex != -1)
               {
                    selectedData.RemoveObjectAt(gridPosition);
                    objectPlacer.RemoveObjectAt(gameObjectIndex);
               }
          }
     }

     public void StartReplacementModeForPosition(Vector3Int gridPosition)
     {
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new RePlacementState(grid, 
                                             preview, 
                                             database, 
                                             floorData, 
                                             furnitureData, 
                                             objectPlacer,
                                             audioSource,
                                             placeSound,
                                             wrongPlacementSound);
          ((RePlacementState)buildingState).SelectObject(gridPosition);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     private void PlaceStructure(){
          if(inputManager.IsPointerOverUI()){
               Debug.Log("Is pointer over UI");
               return;
          }
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          buildingState.OnAction(gridPosition);
     }

     
     private void StopPlacement()
     {
          if(buildingState == null){
               return;
          }
          gridVisualization.SetActive(false);
          buildingState.EndState();
          inputManager.OnClicked -= PlaceStructure;
          inputManager.OnExit -= StopPlacement;
          lastDetectedPosition = Vector3Int.zero;
          buildingState = null;
     }

     public void StartPlacement(int ID)
     {
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new PlacementState(ID, 
                                             grid, 
                                             preview, 
                                             database, 
                                             floorData, 
                                             furnitureData, 
                                             objectPlacer,
                                             audioSource,
                                             placeSound,
                                             wrongPlacementSound);

          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     public void StartRemoving(){
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, audioSource, removeSound);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     public void StartReplacementMode()
     {
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new RePlacementState(grid, 
                                             preview, 
                                             database, 
                                             floorData, 
                                             furnitureData, 
                                             objectPlacer,
                                             audioSource,
                                             placeSound,
                                             wrongPlacementSound);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     
}