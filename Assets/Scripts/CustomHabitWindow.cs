using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomHabitWindow : WindowBase
{
    public override WindowType Type => WindowType.CustomHabit;

    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        _backButton.onClick.AddListener(() => WindowService.Instance.Back());
        _createButton.onClick.AddListener(OnCreate);
    }

    protected override void OnOpened()
    {
        _nameInput.text = string.Empty;
        _descriptionInput.text = string.Empty;
        _createButton.interactable = false;
        _nameInput.onValueChanged.AddListener(OnNameChanged);
    }

    protected override void OnClosed()
    {
        _nameInput.onValueChanged.RemoveListener(OnNameChanged);
    }

    private void OnNameChanged(string value)
        => _createButton.interactable = !string.IsNullOrWhiteSpace(value);

    private void OnCreate()
    {
        var name = _nameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var habit = new HabitData
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = _descriptionInput.text.Trim(),
            StartUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        HabitRepository.Instance.Add(habit);
        WindowService.Instance.ResetTo(WindowType.HabitList);
    }
}