using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddHabitWindow : WindowBase
{
    public override WindowType Type => WindowType.AddHabit;

    [Serializable]
    public class RegularHabitPreset
    {
        public string Name;
        public string Description;
        public Sprite Icon;
    }

    [Header("Обычные привычки")]
    [SerializeField] private List<RegularHabitPreset> _regularHabits = new();
    [SerializeField] private Transform _regularListContent;
    [SerializeField] private RegularHabitItemView _regularItemPrefab;

    [Header("Управление")]
    [SerializeField] private Button _customButton;
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        _backButton.onClick.AddListener(() => WindowService.Instance.Back());
        _customButton.onClick.AddListener(() => WindowService.Instance.Open(WindowType.CustomHabit));
    }

    protected override void OnOpened() => RebuildList();

    private void RebuildList()
    {
        foreach (Transform child in _regularListContent) Destroy(child.gameObject);

        foreach (var preset in _regularHabits)
        {
            var isAlreadyChecking = false;
            foreach (var existingHabit in HabitRepository.Instance.Habits)
            {
                if (existingHabit.Name.Equals(preset.Name, StringComparison.OrdinalIgnoreCase))
                { isAlreadyChecking = true; }
            }
            if (isAlreadyChecking) continue;

            var item = Instantiate(_regularItemPrefab, _regularListContent);
            item.Bind(preset, OnRegularSelected);

            HabitClient.Instance.GetHabitCount(preset.Name, (count) =>
            {
                if (item != null)
                {
                    item.SetCount(count);
                }
            });
        }
    }

    private void OnRegularSelected(RegularHabitPreset preset)
    {
        foreach (var existingHabit in HabitRepository.Instance.Habits)
        {
            if (existingHabit.Name.Equals(preset.Name, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"Привычка '{preset.Name}' уже добавлена!");
                return; 
            }
        }

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var habit = new HabitData
        {
            Name = preset.Name, 
            Description = preset.Description,
            StartUnixTime = now,
            CreationUnixTime = now 
        };

        HabitRepository.Instance.Add(habit);

        HabitClient.Instance.SendHabitToServer(habit.Name);

        WindowService.Instance.ResetTo(WindowType.HabitList);
    }

}