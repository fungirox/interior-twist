using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState, IRotatable
{
    private bool isRotated = false;
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback; 

    public PlacementState(int ID, 
                        Grid grid, 
                        PreviewSystem previewSystem, 
                        ObjectsDatabaseSO database,
                        GridData floorData, 
                        GridData furnitureData, 
                        ObjectPlacer objectPlacer,
                        SoundFeedback soundFeedback) 
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
    
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if( selectedObjectIndex > -1 ){
            //gridVisualization.SetActive(true);
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size
            );
        }
        else{
            throw new System.Exception($"No object with ID {ID}");
        } 
    }
    

    public void EndState(){
        previewSystem.StopShowingPreview();

    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            return;
        }
        soundFeedback.PlaySound(SoundType.Place);
        GameObject prefab = database.objectsData[selectedObjectIndex].Prefab;
        Vector3 rotation = isRotated ? new Vector3(0, 90, 0) : Vector3.zero;
        
        int index = objectPlacer.PlaceObject(prefab, 
                                            grid.CellToWorld(gridPosition),
                                            rotation);
        
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        
        selectedData.AddObjectAt(gridPosition, 
                                GetCurrentObjectSize(), 
                                database.objectsData[selectedObjectIndex].ID, 
                                index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    public void UpdateState(Vector3Int position){
        bool placementValidity = CheckPlacementValidity(position, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(position), placementValidity);
    }
    
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex){
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData  : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, GetCurrentObjectSize());
    }

    // public Vector2Int GetCurrentObjectSize()
    // {
    //     return currentObjectSize;
    // }
    
    public void Rotate()
    {
        isRotated = !isRotated;
        previewSystem.RotatePreview(isRotated ? 90f : 0f);
        
        // Obtener la posición actual en el grid
        Vector3Int currentGridPosition = grid.WorldToCell(previewSystem.GetCurrentPosition());
        UpdateState(currentGridPosition);
    }

    public Vector2Int GetCurrentObjectSize()
    {
        Vector2Int originalSize = database.objectsData[selectedObjectIndex].Size;
        return isRotated ? new Vector2Int(originalSize.y, originalSize.x) : originalSize;
    }

} 
