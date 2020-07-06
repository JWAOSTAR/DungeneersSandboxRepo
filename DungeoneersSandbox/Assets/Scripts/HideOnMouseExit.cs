using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideOnMouseExit : MonoBehaviour
{
    bool mouseOver = false;
    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.CompareTag("HideOnMouseExit") )
        {
            int i = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int i = 0;
    }

    private void OnMouseOver()
    {
        int i = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int i = 0;
    }

    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
