using UnityEngine;
using Xsolla;

public interface IPage
{
    void Open();
    void Close();
}

public abstract class Page : MonoBehaviour, IPage
{
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}