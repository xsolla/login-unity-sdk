using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	public void GetVirtualCurrencies(Action<List<VirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<VirtualCurrencyModel>());
	}

	public void GetCatalogVirtualItems(Action<List<CatalogVirtualItemModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<CatalogVirtualItemModel>());
	}

	public void GetCatalogVirtualCurrencyPackages(Action<List<CatalogVirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<CatalogVirtualCurrencyModel>());
	}

	public void GetCatalogSubscriptions(Action<List<CatalogSubscriptionItemModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<CatalogSubscriptionItemModel>());
	}

	public void GetCatalogBundles(Action<List<CatalogBundleItemModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<CatalogBundleItemModel>());
	}

	public List<string> GetCatalogGroupsByItem(ItemModel item)
	{
		return new List<string>();
	}

	public void GetInventoryItems(Action<List<InventoryItemModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<InventoryItemModel>());
	}

	public void GetVirtualCurrencyBalance(Action<List<VirtualCurrencyBalanceModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<VirtualCurrencyBalanceModel>());
	}

	public void GetUserSubscriptions(Action<List<UserSubscriptionModel>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(new List<UserSubscriptionModel>());
	}

	public void ConsumeVirtualCurrency(InventoryItemModel currency, uint count, Action onSuccess, Action onFailed = null)
	{
		onSuccess?.Invoke();
	}

	public void ConsumeInventoryItem(InventoryItemModel item, uint count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null)
	{
		onSuccess?.Invoke(item);
	}

	public void PurchaseForRealMoney(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
	{
		onSuccess?.Invoke(item);
	}

	public void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
	{
		onSuccess?.Invoke(item);
	}

	public void PurchaseCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError = null)
	{
		onSuccess?.Invoke(items);
	}

	public Token GetDemoUserToken()
	{
		return new Token();
	}

	public void RedeemCouponCode(string couponCode, Action<List<CouponRedeemedItemModel>> onSuccess, Action<Error> onError)
	{
		onSuccess?.Invoke(new List<CouponRedeemedItemModel>());
	}
}
