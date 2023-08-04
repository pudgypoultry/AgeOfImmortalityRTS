using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    RaycastHit hit;
    List<Interactable> selectedUnits = new List<Interactable>();
    bool isDragging = false;
    Vector3 originalMousePosition;

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
                //Debug.Log(hit.transform.tag);
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
}
