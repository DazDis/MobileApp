using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _button;

    private HabitData _habit;
    private Action<HabitData> _onClick;
    private Coroutine _timerRoutine;

    public void Bind(HabitData habit, Action<HabitData> onClick)
    {
        _habit = habit;
        _onClick = onClick;

        _nameText.text = habit.Name;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onClick?.Invoke(_habit));

        RestartTimer();
    }

    private void RestartTimer()
    {
        if (_timerRoutine != null) StopCoroutine(_timerRoutine);
        if (!isActiveAndEnabled) return;
        _timerRoutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            _timerText.text = TimeFormatter.Format(_habit.Elapsed);
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void OnDisable()
    {
        if (_timerRoutine != null) { StopCoroutine(_timerRoutine); _timerRoutine = null; }
    }
}