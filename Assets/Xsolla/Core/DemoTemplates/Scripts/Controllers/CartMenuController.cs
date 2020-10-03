﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core.Popup;

public class CartMenuController : MonoBehaviour
{
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private ItemContainer itemsContainer;
	[SerializeField] private CartControls cartControls;

	private readonly List<GameObject> _items = new List<GameObject>();
	
	protected virtual void Start()
	{
		if (itemPrefab == null || itemsContainer == null || cartControls == null)
		{
			var message = "Cart prefab is broken. Some fields is null.";
			Debug.LogError(message);
			PopupFactory.Instance.CreateError()
				.SetMessage(message)
				.SetCallback(() => DemoController.Instance.SetState(MenuState.Main));
			return;
		}
		UserCart.Instance.ClearCartEvent += Refresh;
		UserCart.Instance.UpdateItemEvent += OnUpdateItemEvent;
		UserCart.Instance.RemoveItemEvent += OnRemoveItemEvent;

		cartControls.OnClearCart = UserCart.Instance.Clear;
		cartControls.OnBuyCart = OnBuyCart;
		Refresh();
	}

	private void OnDestroy()
	{
		UserCart.Instance.ClearCartEvent -= Refresh;
		UserCart.Instance.UpdateItemEvent -= OnUpdateItemEvent;
		UserCart.Instance.RemoveItemEvent -= OnRemoveItemEvent;
	}

	private void OnUpdateItemEvent(UserCartItem item, int deltaCount) => Refresh();
	private void OnRemoveItemEvent(UserCartItem item) => Refresh();

	private void Refresh()
	{
		ClearCartItems();
		PutItemsToContainer(UserCart.Instance.GetItems());
		InitPrices();
	}
	
	private void ClearCartItems()
	{
		_items.ForEach(Destroy);
		_items.Clear();
	}

	private void InitPrices()
	{
		var totalPrice = UserCart.Instance.CalculateFullPrice();
		var discount = UserCart.Instance.CalculateCartDiscount();
		if (discount >= 0.01F)
		{
			totalPrice -= discount;
			cartControls.Initialize(totalPrice, discount);
		}else
			cartControls.Initialize(totalPrice);
	}
	
	private void OnBuyCart()
	{
		if (cartControls.IsBuyButtonLocked()) return;
		cartControls.LockBuyButton();
		UserCart.Instance.Purchase(onSuccess: () => DemoController.Instance.SetPreviousState(), onError: _ => cartControls.UnlockBuyButton());
	}

	private void PutItemsToContainer(List<UserCartItem> items)
	{
		itemsContainer.Clear();
		items.ForEach(i =>
		{
			var go = itemsContainer.AddItem(itemPrefab);
			go.GetComponent<CartItemUI>().Initialize(i);
			_items.Add(go);
		}); 
	}
}