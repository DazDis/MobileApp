using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

public class HabitClient : MonoBehaviour
{
    public static HabitClient Instance { get; private set; }

    private bool _isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        if (_isInitialized) return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            _isInitialized = true;
            Debug.Log("[HabitClient] Сервисы инициализированы, игрок авторизован.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[HabitClient] Ошибка инициализации: {e.Message}");
        }
    }

    [Serializable]
    private class HabitResponse
    {
        public string status;
        public int count;
    }

    public async void SendHabitToServer(string habitName)
    {
        if (!_isInitialized) await InitializeServices();

        var args = new Dictionary<string, object> { { "habitName", habitName } };

        try
        {
            string result = await CloudCodeService.Instance.CallEndpointAsync(
                "TrackHabit",
                args
            );

            if (result.Contains("ERROR"))
            {
                Debug.LogError($"[HabitClient] Ошибка сервера: {result}");
                return;
            }

            HabitResponse response = JsonUtility.FromJson<HabitResponse>(result);

            if (response != null && response.status == "OK")
            {
                Debug.Log($"[HabitClient] Успех! Привычку '{habitName}' отслеживают {response.count} чел.");
            }
        }
        catch (CloudCodeException e)
        {
            Debug.LogError($"[HabitClient] Ошибка Cloud Code: {e.Message}");
        }
    }
    [Serializable]
    private class HabitCountResponse
    {
        public int count;
    }

    public async void GetHabitCount(string habitName, Action<int> onSuccess)
    {
        if (!_isInitialized) await InitializeServices();

        var args = new Dictionary<string, object> { { "habitName", habitName } };

        try
        {
            string result = await CloudCodeService.Instance.CallEndpointAsync("GetHabitCount", args);
            HabitCountResponse response = JsonUtility.FromJson<HabitCountResponse>(result);

            if (response != null)
            {
                onSuccess?.Invoke(response.count);
            }
        }
        catch (CloudCodeException e)
        {
            Debug.LogError($"[HabitClient] Ошибка получения статистики: {e.Message}");
            onSuccess?.Invoke(0);
        }
    }
}