using System;
using System.Collections;
using PuppeteerSharp;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core.Popup;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(Image))]
	public class SinglePageBrowser2D : MonoBehaviour
	{
#pragma warning disable
		[SerializeField] private Button CloseButton = default;
		[SerializeField] private Button BackButton = default;
		[SerializeField] private Vector2 Viewport = new Vector2(1920.0F, 1080.0F);
		[SerializeField] private GameObject PreloaderPrefab = default;

		public event Action<IXsollaBrowser> BrowserInitEvent;
		public event Action BrowserClosedEvent;
#pragma warning restore

#if UNITY_EDITOR || UNITY_STANDALONE
		private XsollaBrowser xsollaBrowser;
		private Display2DBehaviour display;
		private Keyboard2DBehaviour keyboard;
		private Mouse2DBehaviour mouse;
		private string _urlBeforePopup;

		private void Awake()
		{
			BackButton.onClick.AddListener(OnBackButtonPressed);

			CloseButton.gameObject.SetActive(false);
			BackButton.gameObject.SetActive(false);

			var canvasTrn = GetComponentInParent<Canvas>().transform;
			var canvasRect = ((RectTransform) canvasTrn).rect;

			if (Viewport.x > canvasRect.width)
				Viewport.x = canvasRect.width;
			if (Viewport.y > canvasRect.height)
				Viewport.y = canvasRect.height;

			xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
			xsollaBrowser.LogEvent += Debug.Log;
			xsollaBrowser.Launch
			(
				new LaunchBrowserOptions{
					Width = (int) Viewport.x,
					Height = (int) Viewport.y,
				},
				new BrowserFetcherOptions{
#if UNITY_STANDALONE && !UNITY_EDITOR
					Path = !XsollaSettings.PackInAppBrowserInBuild ? Application.persistentDataPath : String.Empty,
#endif

#if UNITY_EDITOR
					Path = Application.persistentDataPath,
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_64
					Platform = Platform.Win64
#else
					Platform = Platform.Win32
#endif
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
					Platform = Platform.MacOS
#endif

#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
					Platform = Platform.Linux
#endif
				}
			);

			xsollaBrowser.Navigate.SetOnPopupListener((popupUrl =>
			{
				xsollaBrowser.Navigate.GetUrl(currentUrl =>
				{
					if (string.IsNullOrEmpty(_urlBeforePopup))
					{
						_urlBeforePopup = currentUrl;
					}
				});
				xsollaBrowser.Navigate.To(popupUrl, newUrl => { BackButton.gameObject.SetActive(true); });
			}));

			xsollaBrowser.Navigate.SetOnAlertListener(HandleBrowserAlert);

			display = this.GetOrAddComponent<Display2DBehaviour>();
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(WaitPreloaderCoroutine());

			display.StartRedraw((int) Viewport.x, (int) Viewport.y);
			display.RedrawFrameCompleteEvent += EnableCloseButton;
			display.ViewportChangedEvent += (width, height) => Viewport = new Vector2(width, height);

			InitializeInput();
			BrowserInitEvent?.Invoke(xsollaBrowser);
		}

		private void EnableCloseButton()
		{
			display.RedrawFrameCompleteEvent -= EnableCloseButton;

			CloseButton.gameObject.SetActive(true);
			CloseButton.onClick.AddListener(OnCloseButtonPressed);
		}

		private IEnumerator WaitPreloaderCoroutine()
		{
			gameObject.AddComponent<Preloader2DBehaviour>().Prefab = PreloaderPrefab;
			yield return new WaitWhile(() => gameObject.GetComponent<Preloader2DBehaviour>() != null);
		}

		private void InitializeInput()
		{
			mouse = this.GetOrAddComponent<Mouse2DBehaviour>();
			keyboard = this.GetOrAddComponent<Keyboard2DBehaviour>();
			keyboard.EscapePressed += OnKeyboardEscapePressed;
		}

		private void OnCloseButtonPressed()
		{
			Debug.Log("`Close` button pressed");
			Destroy(gameObject, 0.001f);
		}

		private void OnBackButtonPressed()
		{
			Debug.Log("`Back` button pressed");
			xsollaBrowser.Navigate.Back((newUrl =>
			{
				if (newUrl.Equals(_urlBeforePopup))
				{
					BackButton.gameObject.SetActive(false);
					_urlBeforePopup = string.Empty;
				}
			}));
		}

		private void OnKeyboardEscapePressed()
		{
			Debug.Log("`Escape` button pressed");
			Destroy(gameObject, 0.001f);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			BrowserClosedEvent?.Invoke();

			if (mouse != null)
			{
				Destroy(mouse);
				mouse = null;
			}

			if (display != null)
			{
				Destroy(display);
				display = null;
			}

			if (keyboard != null)
			{
				keyboard.EscapePressed -= OnKeyboardEscapePressed;
				Destroy(keyboard);
				keyboard = null;
			}

			if (xsollaBrowser != null)
			{
				Destroy(xsollaBrowser);
				xsollaBrowser = null;
			}

			Destroy(transform.parent.gameObject);
		}

		private static void HandleBrowserAlert(Dialog alert)
		{
			switch (alert.DialogType)
			{
				case DialogType.Alert:
					ShowSimpleAlertPopup(alert);
					break;
				case DialogType.Prompt:
					CloseAlert(alert);
					break;
				case DialogType.Confirm:
					ShowConfirmAlertPopup(alert);
					break;
				case DialogType.BeforeUnload:
					CloseAlert(alert);
					break;
				default:
					CloseAlert(alert);
					break;
			}
		}

		private static void ShowSimpleAlertPopup(Dialog dialog)
		{
			PopupFactory.Instance.CreateSuccess()
				.SetTitle("Attention")
				.SetMessage(dialog.Message)
				.SetCallback(() => dialog.Accept());
		}

		private static void ShowConfirmAlertPopup(Dialog dialog)
		{
			PopupFactory.Instance.CreateConfirmation()
				.SetMessage(dialog.Message)
				.SetConfirmCallback(() => dialog.Accept())
				.SetCancelCallback(() => dialog.Dismiss());
		}

		private static void CloseAlert(Dialog dialog)
		{
			Debug.Log("Browser alert was closed automatically");
			dialog.Accept();
		}
#endif
	}
}