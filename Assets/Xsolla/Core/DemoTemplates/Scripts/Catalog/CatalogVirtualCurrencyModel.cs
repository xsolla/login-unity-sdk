public class CatalogVirtualCurrencyModel : CatalogItemModel
{
	public uint Amount { get; set; }
	public string CurrencySku { get; set; }
	public override bool IsVirtualCurrency() => true;
	public override bool IsSubscription() => false;
}