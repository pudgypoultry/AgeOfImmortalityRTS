using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;


public class BuilderUnitTestText : MonoBehaviour
{
    private TMP_Text builderText;
    [SerializeField]
    private BaseUnit builderUnit;
    [SerializeField]
    private BaseBuilding building;
    [SerializeField]
    private SelectedDictionary selected;
    private void Start()
    {
        builderText = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        builderText.text = "Contains Units: " + selected.ContainsUnits() + "\nContains Nonunits: " + selected.ContainsNonUnits();
        foreach (GameObject obj in selected.selectedTable.Values)
        {
            builderText.text += "\n" + obj.name;
        }
    }
}
