using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewMenu", menuName = "DungoneersSandbox/Sub-Context Menu", order = 4)]
public class Menu : ScriptableObject
{
	[Serializable]
	public class MenuEvent : UnityEvent<UnityEngine.Object> { }

	[Serializable]
	public struct MenuItem
	{
		public string text;
		public UnityEngine.Object item;
	}

	[SerializeField]
	List<MenuItem> items = new List<MenuItem>();

	public MenuEvent onInvoke = new MenuEvent();

	public int itemListLength { get { return items.Count; } }

	public MenuItem GetItem(int i)
	{
		return items[i];
	}

	public void AddItem(MenuItem _item)
	{
		items.Add(_item);
	}

	public void OnInvoke(int i)
	{
		onInvoke.Invoke(items[i].item);
	}

}
