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
        _db.CreateTable<HabitLogData>();

        LoadHabits();
    }

    private void LoadHabits()
    {
        _habits = _db.Table<HabitData>().ToList();

        bool needsUpdate = false;
        foreach (var habit in _habits)
        {
            if (habit.CreationUnixTime == 0)
            {
                habit.CreationUnixTime = habit.StartUnixTime;
                _db.Update(habit);
                needsUpdate = true;
            }
        }

        if (needsUpdate)
        {
            Debug.Log("База данных обновлена: установлено время создания для старых привычек.");
        }
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

    public void AddLog(HabitLogData log)
    {
        _db.Insert(log);
    }

    public List<HabitLogData> GetLogsForHabit(int habitId)
    {
        return _db.Table<HabitLogData>()
                  .Where(x => x.HabitId == habitId)
                  .OrderByDescending(x => x.DayNumber)
                  .ToList();
    }

    public HabitLogData GetLog(int habitId, int dayNumber)
    {
        return _db.Table<HabitLogData>()
                  .FirstOrDefault(x => x.HabitId == habitId && x.DayNumber == dayNumber);
    }

    public void UpdateLog(HabitLogData log)
    {
        _db.Update(log);
    }
}