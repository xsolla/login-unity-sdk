using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SavedTokenAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			StartCoroutine(LoadToken());
		}

		private IEnumerator LoadToken()
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)
				yield return new WaitWhile(() => SdkLoginLogic.Instance.IsOAuthTokenRefreshInProgress);

			if (Token.Load())
			{
				Debug.Log("SavedTokenAuth.TryAuth: Token loaded");
				base.OnSuccess?.Invoke(Token.Instance);
			}
			else
			{
				Debug.Log("SavedTokenAuth.TryAuth: No token");
				base.OnError?.Invoke(null);
			}
		}
	}
}
