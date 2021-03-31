using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBrush : MonoBehaviour
{
    public GameObject currentObject;
    [SerializeField]
    bool active = false;
    public bool ToolActive { get { return active; } set { active = value; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ToolActive) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Tile cursorTile;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name.Contains("Tile"))
            {
                if(!currentObject.activeSelf)
				{
                    currentObject.SetActive(true);
                }
                currentObject.transform.position = new Vector3(hit.collider.transform.parent.position.x, hit.collider.transform.parent.position.y + 0.05f, hit.collider.transform.parent.position.z);
                if(Input.GetMouseButtonDown(0))
				{
                    currentObject = null;
                    ToolActive = false;
                    //FindObjectOfType<MapBuilder>()
				}
            }
			else if(currentObject.activeSelf)
			{
                currentObject.SetActive(false);
			}
        }

    }
}
