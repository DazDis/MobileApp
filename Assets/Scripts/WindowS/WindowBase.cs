using UnityEngine;

public abstract class WindowBase : MonoBehaviour
{
    public abstract WindowType Type { get; }

    public void Open()
    {
        gameObject.SetActive(true);
        OnOpened();
    }

    public void Close()
    {
        OnClosed();
        gameObject.SetActive(false);
    }

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}