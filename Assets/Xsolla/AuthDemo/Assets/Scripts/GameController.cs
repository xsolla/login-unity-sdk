using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

public class GameController : MonoBehaviour
{
    [SerializeField] Button _exit_Btn;

    private void Start()
    {
        _exit_Btn.onClick.AddListener(LogOut);
    }

    private void LogOut()
    {
        XsollaAuthentication.Instance.SignOut();
        SceneManager.LoadScene("Auth");
    }
}