using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageErrorShower : MonoBehaviour
	{
		[SerializeField] Text ErrorText = default;

		private void Awake()
		{
			if (DemoController.Instance.IsAccessTokenAuth)
				DisableCommonButtons();
		}

		private void DisableCommonButtons()
		{
			var buttonsProvider = GetComponent<LoginPageCommonButtonsProvider>();
			if (buttonsProvider != null)
			{
				buttonsProvider.DemoUserButton.gameObject.SetActive(false);
				buttonsProvider.LogInButton.gameObject.SetActive(false);
			}
		}

		public void ShowError(string errorMessage)
		{
			ErrorText.text = errorMessage;
		}

		public void ShowError(Error error)
		{
			if (error == null)
				ShowError("Unknown error");
			else if (!string.IsNullOrEmpty(error.errorMessage))
				ShowError(error.errorMessage);
			else
				ShowError(error.ErrorType.ToString());
		}
	}
}
