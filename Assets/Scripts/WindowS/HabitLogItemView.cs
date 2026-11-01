using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitLogItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private Image _photoImage;

    public void Bind(HabitLogData log)
    {
        if (log.RelapseCount == 0)
        {
            _dayText.text = $"День {log.DayNumber}: Без срывов";
            _dayText.color = Color.green; 
        }
        else
        {
            string word = log.RelapseCount == 1 ? "срыв" : (log.RelapseCount < 5 ? "срыва" : "срывов");
            _dayText.text = $"День {log.DayNumber}: {log.RelapseCount} {word}";
            _dayText.color = Color.red;
        }

        // Логика для будущего фото
        if (_photoImage != null)
        {
            if (!string.IsNullOrEmpty(log.PhotoPath))
            {
                _photoImage.gameObject.SetActive(true);
            }
            else
            {
                _photoImage.gameObject.SetActive(false);
            }
        }
    }
}