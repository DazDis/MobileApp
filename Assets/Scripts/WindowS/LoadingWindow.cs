using System.Collections;
using UnityEngine;

public class LoadingWindow : WindowBase
{
    public override WindowType Type => WindowType.Loading;

    [Header("Настройки перехода")]
    [Tooltip("Какое окно открыть после загрузки")]
    [SerializeField] private WindowType _nextWindow = WindowType.HabitList;

    [Tooltip("Время ожидания перед началом исчезновения (в секундах)")]
    [SerializeField] private float _delay = 1f;

    [Tooltip("Время плавного исчезновения (в секундах)")]
    [SerializeField] private float _fadeDuration = 0.5f;

    [Tooltip("Компонент для управления прозрачностью")]
    [SerializeField] private CanvasGroup _canvasGroup;

    private Coroutine _loadingRoutine;

    protected override void OnOpened()
    {
        if (_loadingRoutine != null) StopCoroutine(_loadingRoutine);
        _loadingRoutine = StartCoroutine(LoadingRoutine());
    }

    protected override void OnClosed()
    {
        if (_loadingRoutine != null) StopCoroutine(_loadingRoutine);
    }

    private IEnumerator LoadingRoutine()
    {
        // 1. Убеждаемся, что окно полностью видимо в начале
        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;

        // 2. Ждем заданное время (пока идет "загрузка")
        yield return new WaitForSecondsRealtime(_delay);

        // 3. Плавно скрываем окно (Fade Out)
        if (_canvasGroup != null && _fadeDuration > 0)
        {
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                // Используем unscaledDeltaTime, чтобы эффект работал даже если игра на паузе (Time.timeScale = 0)
                elapsed += Time.unscaledDeltaTime;

                // Плавно меняем прозрачность от 1 до 0
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
        }

        // 4. Переключаемся на следующее окно
        WindowService.Instance.ResetTo(_nextWindow);
    }
}