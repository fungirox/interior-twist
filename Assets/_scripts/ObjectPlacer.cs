using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List <GameObject> placedGameObjects = new ();

    public int PlaceObject(GameObject prefab, Vector3 position, Vector3 rotation = default){
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(rotation);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex){
        if(placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null){
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public void RemoveAllObjects()
    {
        foreach (var obj in placedGameObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        placedGameObjects.Clear();
    }

    public bool IsObjectRotated(int index)
    {
        if (index >= 0 && index < placedGameObjects.Count)
        {
            // Verificar si la rotación es cercana a 90 grados
            return Mathf.Abs(placedGameObjects[index].transform.rotation.eulerAngles.y - 90f) < 1f;
        }
        return false;
    }
}
