using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : MonoBehaviour
{
    RaycastHit hit;
    Vector3 movePoint;
    public GameObject prefab;

    private void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 3)))
        { 
            transform.position = hit.point;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 3)))
        {
            transform.position = hit.point;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, transform.rotation);
            }

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                Destroy(gameObject);
            }
                
        }
    }
}
