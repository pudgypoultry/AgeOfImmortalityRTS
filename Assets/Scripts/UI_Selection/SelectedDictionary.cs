using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
    public PlayerController player;
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    public bool SelectionExists { get => selectedTable.Count > 0; }

    public void AddSelected(GameObject obj)
    {
        int id = obj.GetInstanceID();

        if (!selectedTable.ContainsKey(id) && !obj.CompareTag("Ground"))
        {
            selectedTable.Add(id, obj);
            // obj.AddComponent<SelectionComponent>();
            Debug.Log("Added " + id + " to the selected dictionary");
        }

        Debug.Log("Units?: " + ContainsUnits() + "\nNonUnits?: " + ContainsNonUnits());
        if (ContainsNonUnits() && ContainsUnits())
        {
            RemoveNonUnits();
        }

        if (ContainsEnemies() && ContainsFriendlies())
        {
            RemoveEnemies();
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

    public bool AllSame()
    {
        if (selectedTable.Count == 0)
        {
            return false;
        }

        int count = 0;
        GameObject firstObj = null;

        foreach (GameObject obj in selectedTable.Values)
        {
            if (count == 0)
            {
                firstObj = obj;
            }

            else 
            {
                if (obj != firstObj)
                {
                    return false;
                }
            }

            count++;
        }

        return true;
    }

    public bool ContainsNonUnits()
    {
        foreach (GameObject obj in selectedTable.Values)
        { 
            if (obj.GetComponent<BaseUnit>() == null)
            { 
                return true; 
            }
        }

        return false;
    }

    public bool ContainsUnits()
    {
        foreach (GameObject obj in selectedTable.Values)
        { 
            if (obj.GetComponent<BaseUnit>() != null)
            { 
                return true;
            }
        }

        return false;
    }

    public void RemoveNonUnits()
    {
        List<int> keysToRemove = new List<int>();

        foreach (KeyValuePair<int, GameObject> kvp in selectedTable)
        {
            if (selectedTable[kvp.Key].GetComponent<BaseUnit>() == null)
            {
                // Debug.Log("Removing: " + kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (int i in keysToRemove)
        {
            if (selectedTable[i].GetComponent<Interactable>() != null)
            {
                Debug.Log("REMOVING FROM SELECTION DICTIONARY: " + selectedTable[i]);
            }

            selectedTable.Remove(i);
        }
    }

    public bool ContainsEnemies()
    {
        foreach (GameObject obj in selectedTable.Values)
        {
            if (obj.GetComponent<Interactable>() != null && obj.GetComponent<Interactable>().PlayerID != player.playerID)
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsFriendlies()
    {
        foreach (GameObject obj in selectedTable.Values)
        {
            if (obj.GetComponent<Interactable>() != null && obj.GetComponent<Interactable>().PlayerID == player.playerID)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveEnemies()
    {
        List<int> keysToRemove = new List<int>();

        foreach (KeyValuePair<int, GameObject> kvp in selectedTable)
        {
            if (kvp.Value.GetComponent<Interactable>() != null && kvp.Value.GetComponent<Interactable>().PlayerID != player.playerID)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (int i in keysToRemove)
        {
            Debug.Log("REMOVING FROM SELECTION DICTIONARY: " + selectedTable[i]);
            // selectedTable[i].GetComponent<Interactable>().Deselect();
            selectedTable.Remove(i);
        }
    }
}
