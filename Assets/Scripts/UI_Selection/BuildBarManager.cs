using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBarManager : MonoBehaviour
{
    public Image buildBar;
    [SerializeField]
    private Interactable myParent;
    private IBuildable buildTarget;
    private Canvas canvas;

    // Start is called before the first frame update
    private void Start()
    {
        transform.rotation = Quaternion.Euler(35, 0, 0);
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        if (myParent.GetComponent<IBuildable>() != null)
        {
            buildTarget = myParent.GetComponent<IBuildable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posDifference = transform.position - canvas.worldCamera.transform.position;
        if (buildTarget != null)
        {
            buildBar.fillAmount = buildTarget.BuildTime / buildTarget.BaseBuildTime;
        }

    }
}
