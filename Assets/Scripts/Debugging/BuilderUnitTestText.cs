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
    private void Start()
    {
        builderText = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (building.buildQueue.Count == 0)
        {
            builderText.text = "No buildings in build queue!";
        }

        else
        {
            builderText.text = "Current Time Building: " + building.buildProgress;
            foreach (GameObject obj in building.buildQueue)
            {
                builderText.text += "\n" + obj.name;
            }
        }
    }
}
