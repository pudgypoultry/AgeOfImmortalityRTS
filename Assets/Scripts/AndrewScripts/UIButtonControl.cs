using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonControl : MonoBehaviour
{

    public List<Button> buttons = new List<Button>();

    public void RestructureButtons(GameObject source)
    {
        if (source.GetComponent<BaseBuilding>() != null)
        {
            BaseBuilding sourceBuilding = source.GetComponent<BaseBuilding>();
            List<GameObject> potentialProjects = sourceBuilding.buildOptions;

            int i = 0;

            for (i = 0; i < potentialProjects.Count; i++)
            {
                // Debug.Log("CHECKING FOR I: " + i);
                buttons[i].gameObject.SetActive(true);
                buttons[i].transform.GetChild(0).GetComponent<TMP_Text>().text = potentialProjects[i].name;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => sourceBuilding.StartProject(i));
            }

            for (int j = i; j < buttons.Count; j++)
            {
                buttons[j].onClick.RemoveAllListeners();
                buttons[j].gameObject.SetActive(false);
            }
        }

        else 
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

    }
}
