using UnityEngine;

public class RePlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int originalObjectIndex = -1;
    private Vector3Int originalPosition;
    private Vector2Int originalSize;
    
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDatabaseSO database;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    public RePlacementState(Grid grid, 
                          PreviewSystem previewSystem, 
                          ObjectsDatabaseSO database,
                          GridData floorData, 
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (selectedObjectIndex == -1)
        {
            // Primera fase: Seleccionar el objeto a mover
            SelectObject(gridPosition);
        }
        else
        {
            // Segunda fase: Colocar el objeto en la nueva posición
            PlaceObject(gridPosition);
        }
    }

    private void SelectObject(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        int objectID = -1;

        // Primero intentamos seleccionar muebles, luego el suelo
        if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = furnitureData;
            objectID = furnitureData.GetObjectIDAt(gridPosition);
        }
        else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = floorData;
            objectID = floorData.GetObjectIDAt(gridPosition);
        }

        if (selectedData != null)
        {
            // Guardar información del objeto original
            originalObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            originalPosition = gridPosition;
            selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == objectID);
            originalSize = database.objectsData[selectedObjectIndex].Size;

            if (selectedObjectIndex != -1)
            {
                // Cambiar a modo de previsualización de colocación
                previewSystem.StopShowingPreview();
                previewSystem.StartShowingPlacementPreview(
                    database.objectsData[selectedObjectIndex].Prefab,
                    database.objectsData[selectedObjectIndex].Size
                );

                // Remover temporalmente el objeto original de la grid
                selectedData.RemoveObjectAt(gridPosition);
            }
        }
    }

    private void PlaceObject(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (!placementValidity)
        {
            // Si no es válida la nueva posición, revertimos a la posición original
            RestoreOriginalPosition();
            return;
        }

        // Remover el objeto original
        objectPlacer.RemoveObjectAt(originalObjectIndex);

        // Colocar el objeto en la nueva posición
        int newIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );

        // Actualizar la grid con la nueva posición
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            newIndex
        );

        // Resetear el estado
        ResetState();
    }

    private void RestoreOriginalPosition()
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            originalPosition,
            originalSize,
            database.objectsData[selectedObjectIndex].ID,
            originalObjectIndex
        );
        ResetState();
    }

    private void ResetState()
    {
        selectedObjectIndex = -1;
        originalObjectIndex = -1;
        previewSystem.StopShowingPreview();
        previewSystem.StartShowingRemovePreview();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int position)
    {
        if (selectedObjectIndex != -1)
        {
            // Si ya tenemos un objeto seleccionado, mostrar preview de colocación
            bool validity = CheckPlacementValidity(position);
            previewSystem.UpdatePosition(grid.CellToWorld(position), validity);
        }
        else
        {
            // Si no hay objeto seleccionado, mostrar preview de selección
            bool validity = !furnitureData.CanPlaceObjectAt(position, Vector2Int.one) ||
                          !floorData.CanPlaceObjectAt(position, Vector2Int.one);
            previewSystem.UpdatePosition(grid.CellToWorld(position), validity);
        }
    }
}