using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitLogItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private Image _photoImage;
    [SerializeField] private Button _photoButton; // Новая кнопка для вызова камеры

    private HabitLogData _log;
    private Sprite _loadedSprite;

    private void Awake()
    {
        if (_photoButton != null)
            _photoButton.onClick.AddListener(OnPhotoButtonClicked);
    }

    public void Bind(HabitLogData log)
    {
        _log = log;

        if (log.RelapseCount == 0)
        {
            _dayText.text = $"День {log.DayNumber}: Без срывов";
            _dayText.color = Color.white;
        }
        else
        {
            string word = log.RelapseCount == 1 ? "срыв" : (log.RelapseCount < 5 ? "срыва" : "срывов");
            _dayText.text = $"День {log.DayNumber}: {log.RelapseCount} {word}";
            _dayText.color = Color.red;
        }

        // Загрузка фото, если оно уже есть
        if (_photoImage != null)
        {
            if (!string.IsNullOrEmpty(log.PhotoPath) && System.IO.File.Exists(log.PhotoPath))
            {
                _photoImage.gameObject.SetActive(true);
                LoadPhoto(log.PhotoPath);
            }
        }
    }

    private void OnPhotoButtonClicked()
    {
        if (_log == null) return;

        // Проверка, занята ли камера, чтобы избежать конфликтов
        if (NativeCamera.IsCameraBusy()) return;

        // Вызов камеры. maxSize = 1024 (ограничение размера для экономии памяти)
        NativeCamera.TakePicture((path) =>
        {
            // Если пользователь отменил съемку, path будет null
            if (path == null) return;

            // Сохраняем путь к фото в нашу модель
            _log.PhotoPath = path;

            // Обновляем запись в базе данных SQLite
            HabitRepository.Instance.UpdateLog(_log);

            // Обновляем UI (загружаем только что сделанное фото)
            if (_photoImage != null)
            {
                _photoImage.gameObject.SetActive(true);
                LoadPhoto(path);
            }
        }, maxSize: 1024);
    }

    private void LoadPhoto(string path)
    {
        // Загружаем текстуру из файла
        Texture2D texture = NativeCamera.LoadImageAtPath(path, 1024);
        if (texture != null)
        {
            // Конвертируем текстуру в Sprite для отображения в UI Image
            _loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _photoImage.sprite = _loadedSprite;
            _photoImage.preserveAspect = true; // Сохраняем пропорции фото
        }
    }

    private void OnDestroy()
    {
        // Очищаем память при удалении объекта списка
        if (_loadedSprite != null) Destroy(_loadedSprite);
    }
}