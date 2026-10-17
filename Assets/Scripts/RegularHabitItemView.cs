using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegularHabitItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;

    private AddHabitWindow.RegularHabitPreset _preset;
    private Action<AddHabitWindow.RegularHabitPreset> _onClick;

    public void Bind(AddHabitWindow.RegularHabitPreset preset,
                     Action<AddHabitWindow.RegularHabitPreset> onClick)
    {
        _preset = preset;
        _onClick = onClick;

        _nameText.text = preset.Name;
        if (_icon != null && preset.Icon != null) _icon.sprite = preset.Icon;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onClick?.Invoke(_preset));
    }
}