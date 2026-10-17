using System;
using System.Collections.Generic;
using UnityEngine;

public class HabitRepository : MonoBehaviour
{
    [SerializeField] private List<HabitData> _habits = new();
    public static HabitRepository Instance { get; private set; }
    public IReadOnlyList<HabitData> Habits => _habits;

    public event Action Changed;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Add(HabitData habit)
    {
        _habits.Add(habit);
        Changed?.Invoke();
    }

    public void Remove(HabitData habit)
    {
        if (_habits.Remove(habit)) Changed?.Invoke();
    }

    public void ResetTimer(HabitData habit)
    {
        habit.StartUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Changed?.Invoke();
    }
}