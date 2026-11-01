using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitDetailWindow : WindowBase
{
    public override WindowType Type => WindowType.HabitDetail;

    [Header("Информация")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _timerText;

    [Header("Кнопки")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _deleteButton;

    [Header("История по дням")]
    [SerializeField] private Transform _logsContent;
    [SerializeField] private HabitLogItemView _logItemPrefab;

    private HabitData _habit;
    private Coroutine _timerRoutine;
    private List<HabitLogItemView> _spawnedLogs = new();

    private void Awake()
    {
        _backButton.onClick.AddListener(() => WindowService.Instance.Back());
        _resetButton.onClick.AddListener(OnResetClicked);
        _deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void SetHabit(HabitData habit) => _habit = habit;

    protected override void OnOpened()
    {
        if (_habit == null) { WindowService.Instance.Back(); return; }

        _nameText.text = _habit.Name;
        _descriptionText.text = _habit.Description;

        if (_timerRoutine != null) StopCoroutine(_timerRoutine);
        _timerRoutine = StartCoroutine(TimerRoutine());

        RebuildLogs();
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
        long baseTime = _habit.CreationUnixTime > 0 ? _habit.CreationUnixTime : _habit.StartUnixTime;
        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int currentDay = (int)((now - baseTime) / 86400) + 1;

        var existingLog = HabitRepository.Instance.GetLog(_habit.Id, currentDay);

        if (existingLog != null)
        {
            existingLog.RelapseCount++;
            existingLog.DateUnixTime = now;
            HabitRepository.Instance.UpdateLog(existingLog);
        }
        else
        {
            var newLog = new HabitLogData
            {
                HabitId = _habit.Id,
                DayNumber = currentDay,
                RelapseCount = 1,
                DateUnixTime = now
            };
            HabitRepository.Instance.AddLog(newLog);
        }

        HabitRepository.Instance.ResetTimer(_habit);

        RebuildLogs();
    }

    private void RebuildLogs()
    {
        foreach (var log in _spawnedLogs) Destroy(log.gameObject);
        _spawnedLogs.Clear();

        var dbLogs = HabitRepository.Instance.GetLogsForHabit(_habit.Id);

        long baseTime = _habit.CreationUnixTime > 0 ? _habit.CreationUnixTime : _habit.StartUnixTime;
        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int currentDay = (int)((now - baseTime) / 86400) + 1;

        for (int day = currentDay; day >= 1; day--)
        {
            var existingLog = dbLogs.Find(x => x.DayNumber == day);

            if (existingLog != null)
            {
                var item = Instantiate(_logItemPrefab, _logsContent);
                item.Bind(existingLog);
                _spawnedLogs.Add(item);
            }
            else
            {
                var emptyLog = new HabitLogData
                {
                    DayNumber = day,
                    RelapseCount = 0
                };
                var item = Instantiate(_logItemPrefab, _logsContent);
                item.Bind(emptyLog);
                _spawnedLogs.Add(item);
            }
        }
    }

    private void OnDeleteClicked()
    {
        HabitRepository.Instance.Remove(_habit);
        WindowService.Instance.ResetTo(WindowType.HabitList);
    }
}