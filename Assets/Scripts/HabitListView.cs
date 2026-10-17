using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HabitListView : WindowBase
{
    public override WindowType Type => WindowType.HabitList;

    [Header("Список привычек")]
    [SerializeField] private Transform _content;
    [SerializeField] private HabitItemView _itemPrefab;

    [Header("Кнопки")]
    [SerializeField] private Button _addButton; 

    private readonly List<HabitItemView> _items = new();

    private void Awake()
    {
        if (_addButton != null)
        {
            _addButton.onClick.AddListener(() => WindowService.Instance.Open(WindowType.AddHabit));
        }
    }

    private void OnEnable()
    {
        if (HabitRepository.Instance != null) HabitRepository.Instance.Changed += Rebuild;
    }

    private void OnDisable()
    {
        if (HabitRepository.Instance != null) HabitRepository.Instance.Changed -= Rebuild;
    }

    protected override void OnOpened() => Rebuild();

    private void Rebuild()
    {
        foreach (var it in _items) if (it) Destroy(it.gameObject);
        _items.Clear();

        if (HabitRepository.Instance == null) return;

        foreach (var habit in HabitRepository.Instance.Habits)
        {
            var v = Instantiate(_itemPrefab, _content);
            v.Bind(habit, OnHabitClicked);
            _items.Add(v);
        }
    }

    private void OnHabitClicked(HabitData habit)
    {
        var detail = WindowService.Instance.Get<HabitDetailWindow>(WindowType.HabitDetail);
        detail.SetHabit(habit);
        WindowService.Instance.Open(WindowType.HabitDetail);
    }
}