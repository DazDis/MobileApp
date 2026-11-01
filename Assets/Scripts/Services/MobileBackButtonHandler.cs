using UnityEngine;
using UnityEngine.InputSystem;

public class MobileBackButtonHandler : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.isPressed)
        {
            if (Time.frameCount % 2 == 0)
            {
                WindowService.Instance.Back();
            }
        }
    }
}