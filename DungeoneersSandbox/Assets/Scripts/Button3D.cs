using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Button3D : Button
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            onClick.Invoke();
        }
    }
}
