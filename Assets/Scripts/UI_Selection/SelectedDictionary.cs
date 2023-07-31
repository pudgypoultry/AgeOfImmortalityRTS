using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    public void AddSelected(GameObject obj)
    {
        int id = obj.GetInstanceID();

        if (!selectedTable.ContainsKey(id) && !obj.CompareTag("Ground"))
        { 
            selectedTable.Add(id, obj);
            // obj.AddComponent<SelectionComponent>();
            Debug.Log("Added " + id + " to the selected dictionary");
        }
    }

    public void Deselect(int id)
    { 
        // Destroy(selectedTable[id].GetComponent<SelectionComponent>());
        selectedTable.Remove(id);
    }

    public void DeselectAll()
    {
        foreach (KeyValuePair<int, GameObject> kvp in selectedTable)
        {
            if (kvp.Value != null)
            { 
                // Destroy(selectedTable[kvp.Key].GetComponent<SelectionComponent>());
            }
        }
        selectedTable.Clear();
    }
}
