using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class UserInfoDrawer : MonoBehaviour
{
    [SerializeField] private Text userEmail;
    
    IEnumerator Start()
    {
	    if (userEmail == null)
	    {
		    Debug.LogWarning("Can not draw user info because 'userEmail' field is null or empty! Waiting...");
		    yield return new WaitWhile(() => userEmail == null);
	    }
	    yield return new WaitWhile(() => XsollaLogin.Instance.Token.IsNullOrEmpty());
	    
	    var busy = true;
	    DemoController.Instance.GetImplementation().GetUserInfo(XsollaLogin.Instance.Token, info =>
	    {
			if (userEmail != null)
				userEmail.text = info?.email?.ToUpper() ?? string.Empty;
		    busy = false;
	    }, _ => busy = false);
	    
	    yield return new WaitWhile(() => busy);
		//Destroy(this, 0.1F);
		this.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}

	public void Refresh()
	{
		StartCoroutine(Start());
	}
}
