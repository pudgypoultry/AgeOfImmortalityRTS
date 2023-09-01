using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalSelection : MonoBehaviour
{
    private SelectedDictionary selectedTable;
    private RaycastHit hit;

    private bool dragSelect;
    [SerializeField]
    private float dragTolerance;

    #region Collider Variables
    MeshCollider selectionBox;
    Mesh selectionMesh;

    Vector3 p1;
    Vector3 p2;

    Vector2[] corners;

    Vector3[] vertices;
    Vector3[] vecs;

    #endregion
    void Start()
    {
        selectedTable = GetComponent<SelectedDictionary>();
        dragSelect = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            p1 = Input.mousePosition;
            //if (!Input.GetKey(KeyCode.LeftShift))
            //{
            //    selectedTable.DeselectAll();
            //}
        }

        if (Input.GetMouseButton(0))
        {
            if ((p1 - Input.mousePosition).magnitude > dragTolerance)
            {
                dragSelect = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!dragSelect)
            {
                Ray ray = Camera.main.ScreenPointToRay(p1);

                if (Physics.Raycast(ray, out hit, 50000.0f))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        selectedTable.AddSelected(hit.transform.gameObject);
                    }
                    else if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        selectedTable.DeselectAll();
                        selectedTable.AddSelected(hit.transform.gameObject);
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        selectedTable.DeselectAll();
                    }
                }
            }
            else // Marquee select
            {
                vertices = new Vector3[4];
                vecs = new Vector3[4];

                int i = 0;
                p2 = Input.mousePosition;
                corners = GetBoundingBox(p1, p2);

                foreach (Vector3 vec in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(vec);

                    if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 3)))
                    {
                        vertices[i] = new Vector3(hit.point.x, 0, hit.point.z);
                        vecs[i] = ray.origin - hit.point;
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(vec), hit.point, Color.red, 1.0f);
                        Debug.Log("Drawing line at vertex: " + vertices[i]);
                    }
                    i++;
                }

                selectionMesh = GenerateSelectionMesh(vertices, vecs);

                selectionBox = gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    selectedTable.DeselectAll();
                }

                Destroy(selectionBox, 0.02f);
            }

            dragSelect = false;
        }
    }

    private void OnGUI()
    {
        if (dragSelect)
        {
            var rect = Utils.GetScreenRect(p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x)
        {
            if (p1.y > p2.y)
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else
        {
            if (p1.y > p2.y)
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }
        }

        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }

    Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j-4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        selectedTable.AddSelected(other.gameObject);
    }
}
