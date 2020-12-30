﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class UserProfileEntryUI : MonoBehaviour
	{
		[SerializeField] GameObject SetStateObjects = default;
		[SerializeField] GameObject UnsetStateObjects = default;
		[SerializeField] GameObject EditStateObjects = default;

		[SerializeField] Text CurrentValueText = default;

		[SerializeField] SimpleButton[] EditButtons = default;
		[SerializeField] UserProfileEntryEditor EntryEditor = default;
		[SerializeField] BaseUserProfileValueConverter ValueConverter = default;

		private UserProfileEntryType _entryType;
	
		private string CurrentValue
		{
			get => CurrentValueText.text;
			set
			{
				CurrentValueText.text = value;

				if(!string.IsNullOrEmpty(value))
					SetState(EntryState.Set);
				else
				{
					if (UnsetStateObjects)
						SetState(EntryState.Unset);
					else
						SetState(EntryState.Edit);
				}
			}
		}

		public static event Action<UserProfileEntryUI, UserProfileEntryType, string, string> UserEntryEdited;

		public void InitializeEntry(UserProfileEntryType entryType, string value)
		{
			_entryType = entryType;

			if (ValueConverter != null && !string.IsNullOrEmpty(value))
				value = ValueConverter.Convert(value);

			if (EntryEditor)
				EntryEditor.SetInitial(value);

			CurrentValue = value;
		}

		private void Awake()
		{
			foreach (var button in EditButtons)
				button.onClick += () => SetState(EntryState.Edit);

			if (EntryEditor)
				EntryEditor.UserProfileEntryEdited += OnEntryEdited;
		}

		private void SetState(EntryState entryState)
		{
			if (SetStateObjects)
				SetActive(SetStateObjects, entryState == EntryState.Set);
			if (UnsetStateObjects)
				SetActive(UnsetStateObjects, entryState == EntryState.Unset);
			if (EditStateObjects)
				SetActive(EditStateObjects, entryState == EntryState.Edit);
		}

		private void SetActive(GameObject gameObject, bool targetState)
		{
			if (targetState != gameObject.activeSelf)
				gameObject.SetActive(targetState);
		}

		private void OnEntryEdited(string newValue)
		{
			var isConverterUsed = ValueConverter != null;

			if (isConverterUsed && !string.IsNullOrEmpty(newValue))
				newValue = ValueConverter.ConvertBack(newValue);

			var isValueChanged = isConverterUsed
				? newValue == ValueConverter.ConvertBack(CurrentValue)
				: newValue == CurrentValue;

			if (isValueChanged)
			{
				CurrentValue = isConverterUsed ? ValueConverter.Convert(newValue) : newValue;
			}
			else
			{
				var oldValue = CurrentValue;
				CurrentValue = isConverterUsed ? ValueConverter.Convert(newValue) : newValue;

				UserEntryEdited?.Invoke(this, _entryType, oldValue, newValue);
			}
		}

		private enum EntryState
		{
			Set, Unset, Edit
		}
	}
}
