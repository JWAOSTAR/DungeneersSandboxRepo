using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Button3D : Button
{
    /// <summary>
    /// When the cursor is over this object check if mouse down and call all function in onClick Event
    /// </summary>
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            onClick.Invoke();
        }
    }
}
