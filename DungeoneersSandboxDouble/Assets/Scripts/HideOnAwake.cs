using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideOnAwake : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(false);
	}
}
