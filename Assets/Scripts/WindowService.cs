using System;
using System.Collections.Generic;
using UnityEngine;

public enum WindowType
{
    HabitList,
    AddHabit,
    CustomHabit,
    HabitDetail
}

public class WindowService : MonoBehaviour
{
    public static WindowService Instance { get; private set; }

    [Serializable]
    public class WindowEntry
    {
        public WindowType Type;
        public WindowBase Prefab;
    }

    [SerializeField] private Transform _windowRoot;             
    [SerializeField] private List<WindowEntry> _windowPrefabs;   
    [SerializeField] private WindowType _defaultWindow = WindowType.HabitList;

    private readonly Dictionary<WindowType, WindowBase> _instances = new();
    private readonly Stack<WindowBase> _history = new();
    private WindowBase _current;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in _windowPrefabs)
        {
            var inst = Instantiate(entry.Prefab);
            inst.transform.SetParent(_windowRoot, false);
            inst.gameObject.SetActive(false);
            _instances[entry.Type] = inst;
        }
    }

    private void Start() => Open(_defaultWindow);

    public T Get<T>(WindowType type) where T : WindowBase
        => _instances.TryGetValue(type, out var w) ? w as T : null;

    public void Open(WindowType type)
    {
        if (!_instances.TryGetValue(type, out var window))
        {
            Debug.LogError($"Window {type} is not registered");
            return;
        }
        if (_current == window) return;

        if (_current != null)
        {
            _current.Close();
            _history.Push(_current);
        }
        window.Open();
        _current = window;
    }

    public void Back()
    {
        if (_history.Count == 0) return;
        _current?.Close();
        var prev = _history.Pop();
        prev.Open();
        _current = prev;
    }

    public void ResetTo(WindowType type)
    {
        foreach (var w in _instances.Values) w.Close();
        _history.Clear();
        _current = null;
        Open(type);
    }
}