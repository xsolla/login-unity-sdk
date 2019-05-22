using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

// REVIEW Is it still used?
public class GameController : MonoBehaviour
{
    [SerializeField] private Button _exit_Btn;
    private void Start()
    {
        _exit_Btn.onClick.AddListener(LogOut);
        XsollaAuthentication.Instance.OnSuccessfulSignOut += OnSignOut;
    }

    private void OnSignOut()
    {
        Debug.Log("Successfully sign out");
    }

    private void LogOut()
    {
        XsollaAuthentication.Instance.SignOut();
        SceneManager.LoadScene("Auth");
    }
}
