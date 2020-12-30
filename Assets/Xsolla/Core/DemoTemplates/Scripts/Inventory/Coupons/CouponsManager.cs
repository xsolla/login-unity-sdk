﻿using UnityEngine;
using Xsolla.Core.Popup;

public class CouponsManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleTextButton redeemCouponButton;
#pragma warning restore 0649

	void Start()
	{
		redeemCouponButton.onClick += ShowRedeemCouponPopup;
	}

	private void ShowRedeemCouponPopup()
	{
		var redeemCouponPopup = PopupFactory.Instance.CreateRedeemCoupon();
		redeemCouponPopup.SetRedeemCallback(code =>
		{
			DemoController.Instance.GetImplementation().RedeemCouponCode(code, redeemedItems =>
			{
				redeemCouponPopup.Close();
				UserInventory.Instance.Refresh();
				PopupFactory.Instance.CreateCouponRewards().SetItems(redeemedItems);
			}, error => redeemCouponPopup.ShowError());
		});
	}
}