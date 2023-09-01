using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectBarManager : MonoBehaviour
{
    public Image progressBar;
    [SerializeField]
    private Interactable myParent;
    private BaseBuilding projectDoer;
    public IProject projectTarget;
    private Canvas canvas;

    // Start is called before the first frame update
    private void Start()
    {
        transform.rotation = Quaternion.Euler(35, 0, 0);
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        if (myParent.GetComponent<BaseBuilding>() != null)
        {
            projectDoer = myParent.GetComponent<BaseBuilding>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posDifference =  transform.position - canvas.worldCamera.transform.position;
        if (projectTarget != null)
        {
            progressBar.fillAmount = projectDoer.buildProgress / projectTarget.BuildTime;
        }

    }
}
