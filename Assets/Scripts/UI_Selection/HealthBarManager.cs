using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Image healthBar;
    [SerializeField]
    private Interactable myParent;
    private IDamageable healthTarget;
    private Canvas canvas;

    // Start is called before the first frame update
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;


        if (myParent.GetComponent<IDamageable>() != null)
        {
            healthTarget = myParent.GetComponent<IDamageable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posDifference = transform.position - canvas.worldCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(0, posDifference.y, posDifference.z));
        if (healthTarget != null)
        {
            healthBar.fillAmount = healthTarget.CurrentHP / healthTarget.BaseHP;
        }

    }
}
