using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitDetailWindow : WindowBase
{
    public override WindowType Type => WindowType.HabitDetail;

    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _resetButton;

    private HabitData _habit;
    private Coroutine _timerRoutine;

    private void Awake()
    {
        _backButton.onClick.AddListener(() => WindowService.Instance.Back());
        _resetButton.onClick.AddListener(OnResetClicked);
    }

    public void SetHabit(HabitData habit) => _habit = habit;

    protected override void OnOpened()
    {
        if (_habit == null) { WindowService.Instance.Back(); return; }

        _nameText.text = _habit.Name;
        _descriptionText.text = _habit.Description;

        if (_timerRoutine != null) StopCoroutine(_timerRoutine);
        _timerRoutine = StartCoroutine(TimerRoutine());
    }

    protected override void OnClosed()
    {
        if (_timerRoutine != null) { StopCoroutine(_timerRoutine); _timerRoutine = null; }
    }

    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            _timerText.text = TimeFormatter.Format(_habit.Elapsed);
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void OnResetClicked()
    {
        _habit.StartUnixTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}