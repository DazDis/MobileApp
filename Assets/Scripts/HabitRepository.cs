using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HabitRepository : MonoBehaviour
{
    public static HabitRepository Instance { get; private set; }
    private List<HabitData> _habits = new();
    public IReadOnlyList<HabitData> Habits => _habits;

    private SQLiteConnection _db;
    private string _dbPath;

    public event Action Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _dbPath = Path.Combine(Application.persistentDataPath, "habits.db");
        _db = new SQLiteConnection(_dbPath);

        _db.CreateTable<HabitData>();

        LoadHabits();
    }

    private void LoadHabits()
    {
        _habits = _db.Table<HabitData>().ToList();
    }

    public void Add(HabitData habit)
    {
        _db.Insert(habit);
        _habits.Add(habit);
        Changed?.Invoke();
    }

    public void Remove(HabitData habit)
    {
        _db.Delete(habit);
        if (_habits.Remove(habit))
        {
            Changed?.Invoke();
        }
    }

    public void ResetTimer(HabitData habit)
    {
        habit.StartUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        _db.Update(habit);

        Changed?.Invoke();
    }

    private void OnDestroy()
    {
        _db?.Close();
    }
}