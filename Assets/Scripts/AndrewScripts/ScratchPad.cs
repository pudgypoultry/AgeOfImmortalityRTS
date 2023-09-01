using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchPad : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController player;
    public Dictionary<int, GameObject> selectedObjectTable = new Dictionary<int, GameObject>();
    public Dictionary<int, Interactable> selectedInteractableTable = new Dictionary<int, Interactable>();

    public bool SelectionExists { get => selectedObjectTable.Count > 0; }

    public void AddSelected(GameObject objToAdd)
    { 
        int id = objToAdd.GetInstanceID();


        if (!selectedObjectTable.ContainsKey(id) && objToAdd.GetComponent<Interactable>() != null)
        {
            selectedObjectTable.Add(id, objToAdd);
            selectedInteractableTable.Add(id, objToAdd.GetComponent<Interactable>());
            Debug.Log("Added " + objToAdd.name + " to the selected dictionary with id " + id);
        }
    }

}
