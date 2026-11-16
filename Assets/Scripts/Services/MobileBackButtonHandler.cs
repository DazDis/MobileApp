using UnityEngine;
using UnityEngine.InputSystem;

public class MobileBackButtonHandler : MonoBehaviour
{
    private InputAction _backAction;

    private void Awake()
    {
        // Создаём действие, привязанное к клавише Escape (эмуляция кнопки "Назад")
        _backAction = new InputAction(binding: "<Keyboard>/escape");
        _backAction.performed += OnBackPressed;
    }

    private void OnEnable()
    {
        _backAction.Enable();
    }

    private void OnDisable()
    {
        _backAction.Disable();
    }

    private void OnBackPressed(InputAction.CallbackContext context)
    {
        if (WindowService.Instance != null)
        {
            WindowService.Instance.Back();
        }
    }
}