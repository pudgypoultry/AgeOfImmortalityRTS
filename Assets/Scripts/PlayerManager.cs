using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    RaycastHit hit;
    List<UnitController> selectedUnits = new List<UnitController>();
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
                if (hit.transform.CompareTag("PlayerUnit"))
                {
                    SelectUnit(hit.transform.GetComponent<UnitController>(), Input.GetKey(KeyCode.LeftShift));
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

            foreach (var selectableObject in FindObjectsOfType<UnitController>())
            {
                if (IsWithinSelectionBounds(selectableObject.transform))
                {
                    SelectUnit(selectableObject.gameObject.GetComponent<UnitController>(), true);
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
                        // Debug.Log("Made it ehre!!!!");
                        selectableObj.MoveUnit(hit.point);
                    }
                }
                else if (hit.transform.CompareTag("EnemyUnit"))
                {
                    foreach (var selectableObj in selectedUnits)
                    {
                        selectableObj.SetNewTarget(hit.transform);
                    }
                }
            }
        }

    }

    private void SelectUnit(UnitController unit, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            DeselectUnits();
        }

        selectedUnits.Add(unit.gameObject.GetComponent<UnitController>());
        unit.SetSelected(true);
    }

    private void DeselectUnits()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            //selectedUnits[i].Find("Highlight").gameObject.SetActive(false);
            selectedUnits[i].SetSelected(false);
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
