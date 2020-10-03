﻿using System;
using UnityEngine;

public class GroupUI : MonoBehaviour, IGroup
{
	[SerializeField] MenuButton menuButton;

	void Awake()
	{
		menuButton.onClick = ((s) =>
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke(s);
		});
	}

	public string Id
	{
		get { return menuButton.Id; }
		set { menuButton.Id = value; }
	}

	public string Name
	{
		get { return menuButton.Text; }
		set { menuButton.Text = value; }
	}

	public Action<string> OnGroupClick { get; set; }

	public void Select()
	{
		menuButton.Select();
	}

	public void Deselect()
	{
		menuButton.Deselect();
	}

	public bool IsSelected()
	{
		return menuButton.IsSelected;
	}
}