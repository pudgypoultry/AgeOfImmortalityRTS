using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadParent : MonoBehaviour
{
    public int maxChildAmount = 12;

    public GameObject target;
    public GameObject childPrefab;
    public List<GameObject> children;

    private void Start()
    {
        children = new List<GameObject>();

        for (int i = 0; i < maxChildAmount; i++)
        {
            Vector3 relativeSpawn = new Vector3(i % 4, 0.33f, i / 4);
            GameObject temp = Instantiate(childPrefab, transform.position + (relativeSpawn * 6.0f), transform.rotation);
            temp.GetComponent<BaseBehavior>().target = gameObject;
            children.Add(temp);
        }
    }

    private void Update()
    {
        transform.position += (target.transform.position - transform.position).normalized * Time.deltaTime * 5.0f;
    }
}
