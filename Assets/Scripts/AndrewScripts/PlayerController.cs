using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTypes;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    protected RaycastHit hit;
    public SelectedDictionary selectedDictionary;
    public List<Interactable> selectedUnits = new List<Interactable>();
    public List<IDropOff> dropOffPoints = new List<IDropOff>();

    public ResourceTypes[] resourceList = { ResourceTypes.BLOOD, ResourceTypes.FAITH, ResourceTypes.FOOD, ResourceTypes.GOLD, ResourceTypes.MADNESS, ResourceTypes.RAGE, ResourceTypes.STONE, ResourceTypes.WOOD };
    public int[] resourceAmounts = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public bool holdingSomething = false;
    public bool isDragging = false;
    public Vector3 originalMousePosition;

    public UIButtonControl buttonController;

    private void OnGUI()
    {
        if (isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(originalMousePosition, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.1f));
            ScreenHelper.DrawScreenRectBorder(rect, 1, Color.blue);
        }

    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();

        if (selectedUnits.Count != 0 && selectedUnits[0].GetComponent<BaseBuilding>() != null)
        {
            RestructureButtons(selectedUnits[0].gameObject);
        }
    }

    private void MouseControl()
    {
        //Detect is mouse is down
        if (Input.GetMouseButtonDown(0))
        {

            originalMousePosition = Input.mousePosition;
            //Create a ray from camera to ground
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Shoot that raycast and get hit data
            if (Physics.Raycast(camRay, out hit))
            {
                //Do something with that data
                Debug.Log("What you clicked: "+hit.transform.tag);
                if (hit.transform.GetComponent<Interactable>() != null && hit.transform.GetComponent<Interactable>().playerID == playerID)
                {
                    SelectUnit(hit.transform.GetComponent<Interactable>(), Input.GetKey(KeyCode.LeftShift));
                }
                else
                {

                    isDragging = true;
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                DeselectUnits();
            }

            foreach (var selectableObject in FindObjectsOfType<Interactable>())
            {
                if (IsWithinSelectionBounds(selectableObject.transform) && selectableObject.playerID == playerID)
                {
                    SelectUnit(selectableObject.gameObject.GetComponent<Interactable>(), true);
                }
            }

            isDragging = false;

        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Debug.Log(selectedUnits.Count);

            originalMousePosition = Input.mousePosition;
            //Create a ray from camera to ground
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Shoot that raycast and get hit data
            if (Physics.Raycast(camRay, out hit))
            {
                //Do something with that data
                //Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("Ground"))
                {
                    foreach (var selectableObj in selectedUnits)
                    {
                        if (selectableObj.GetComponent<IMoveable>() != null)
                        {
                            selectableObj.GetComponent<IMoveable>().MoveTo(this.gameObject, hit.point);
                        }
                        // Debug.Log("Made it ehre!!!!");

                    }
                }

                // If it's an IDamageable and the player ID is different than yours, attack if possible
                else if (hit.transform.GetComponent<IDamageable>() != null && hit.transform.GetComponent<Interactable>().playerID != playerID)
                {
                    Debug.Log("Clicked Enemy to attack");
                    foreach (var selectableObj in selectedUnits)
                    {
                        if (selectableObj.CanAttack)
                        {
                            selectableObj.GetComponent<BaseUnit>().AttackTarget(this.gameObject, hit.transform.GetComponent<IDamageable>());
                        }
                    }
                }

                // If it's an IBuildable and the player ID is the same as yours, build if possible
                else if (hit.transform.GetComponent<IBuildable>() != null && hit.transform.GetComponent<Interactable>().playerID == playerID)
                {
                    Debug.Log("Clicked Building to build");
                    foreach (var selectableObj in selectedUnits)
                    {
                        if (selectableObj.CanBuild)
                        {
                            selectableObj.GetComponent<BaseUnit>().BuildTarget(this.gameObject, hit.transform.GetComponent<IBuildable>());
                        }
                    }
                }

                else if (hit.transform.GetComponent<IGatherable>() != null)
                {
                    Debug.Log("Clicked Node to gather");
                    foreach (var selectableObj in selectedUnits)
                    {
                        Debug.Log("Telling " + selectableObj.gameObject.name + " to gather from " + hit.transform.name);
                        if (selectableObj.CanGather)
                        {
                            selectableObj.GetComponent<BaseUnit>().GatherTarget(this.gameObject, hit.transform.GetComponent<IGatherable>());
                        }
                    }
                }
            }
        }

    }

    private void SelectUnit(Interactable unit, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            DeselectUnits();
        }

        selectedUnits.Add(unit);
        unit.Select();
    }

    private void DeselectUnits()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            //selectedUnits[i].Find("Highlight").gameObject.SetActive(false);
            selectedUnits[i].Deselect();
        }

        selectedUnits.Clear();
    }

    private bool IsWithinSelectionBounds(Transform thing)
    {
        if (!isDragging)
        {
            return false;
        }

        var cam = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(cam, originalMousePosition, Input.mousePosition);
        return viewportBounds.Contains(cam.WorldToViewportPoint(thing.position));
    }

    // Add resources to the player's bank
    public void AddResources(ResourceTypes type, int amount)
    {
        int count = 0;
        foreach (ResourceTypes types in resourceList)
        {
            if (type == types)
            {
                resourceAmounts[count] += amount;
                break;
            }
            count++;
        }
    }

    // Check if player has the resources necessary for something
    public bool CheckResources(ResourceTypes[] types, int[] amounts)
    {
        for (int i = 0; i < types.Length; i++)
        {
            foreach (ResourceTypes type in resourceList)
            {
                if (type == types[i])
                {
                    if (amounts[i] > resourceAmounts[System.Array.IndexOf(resourceList, type)])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // Spend resources, always use check first to make sure it will be ok
    public void SpendResources(ResourceTypes[] types, int[] amounts)
    {
        for (int i = 0; i < types.Length; i++)
        {
            foreach (ResourceTypes type in resourceList)
            {
                Debug.Log("Currently trying to spend " + amounts[i] + " of " + types[i]);
                if (type == types[i])
                {
                    
                    resourceAmounts[System.Array.IndexOf(resourceList, type)] -= amounts[i];
                    break;
                }
            }
        }
    }

    public void RestructureButtons(GameObject source)
    { 
        buttonController.RestructureButtons(source);
    }

}
