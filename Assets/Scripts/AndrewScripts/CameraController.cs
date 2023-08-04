using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Quaternion startRotation;
    private Vector3 pos;
    public Transform refObject;
    public float dragSpeed = 50f;
    public float scrollSpeed = 5f;
    public float speed = 10f;

    void start()
    {
        transform.rotation = startRotation;
        pos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            pos.z += speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            pos.x -= speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            pos.z -= speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            pos.x += speed * Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(transform.forward * -scrollSpeed);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(transform.forward * scrollSpeed);
        }

        /*
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            pos.x += x * dragSpeed * Time.deltaTime;
            pos.z += y * dragSpeed * Time.deltaTime;

        }
        */

        transform.position += pos;
        pos = Vector3.zero;
    }
}