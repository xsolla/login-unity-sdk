using UnityEngine;

public class CommonStoreMenuController : BaseMenuController
{
	[SerializeField] private SimpleButton publisherAccountButton;
	[SerializeField] private SimpleButton documentationButton;
	[SerializeField] private SimpleButton testCardsButton;
	[SerializeField] private SimpleButton backButton;
	[SerializeField] private SimpleButton buyCurrencyButton;

	private void Start()
	{
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
		AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
		AttachUrlToButton(testCardsButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.TestCardsUrl));
		AttachButtonCallback(backButton, () => DemoController.Instance.SetPreviousState());
		AttachButtonCallback(buyCurrencyButton, 
			() => SetMenuState(MenuState.BuyCurrency, () => UserCatalog.Instance.IsUpdated));
	}
}
