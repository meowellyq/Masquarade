using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dialogue
{
    // Управляет показом катсцен в DialogueScene.
    // Скрывает портреты персонажей и показывает нужное изображение.
    // Yarn: cutscene show pond_bench
    // Yarn: cutscene hide
    public class КатсценаКонтроллер : MonoBehaviour
    {
        [Header("Катсцена UI")]
        [Tooltip("Image компонент для отображения картинки катсцены")]
        public Image cutsceneImage;

        [Tooltip("GameObject который содержит портреты персонажей (скрываем во время катсцены)")]
        public GameObject portraitsContainer;

        [Tooltip("Список изображений катсцен")]
        public List<ЗаписьКатсцены> cutscenes = new List<ЗаписьКатсцены>();

        void Start()
        {
            HideCutscene();
        }

        public void ShowCutscene(string imageName)
        {
            Sprite sprite = FindSprite(imageName);

            if (sprite == null)
                Debug.LogWarning($"[Катсцена] Спрайт '{imageName}' не найден. Показываем плейсхолдер.");

            if (portraitsContainer != null)
                portraitsContainer.SetActive(false);

            if (cutsceneImage != null)
            {
                cutsceneImage.sprite = sprite;
                cutsceneImage.gameObject.SetActive(true);
            }
        }

        public void HideCutscene()
        {
            if (portraitsContainer != null)
                portraitsContainer.SetActive(true);

            if (cutsceneImage != null)
                cutsceneImage.gameObject.SetActive(false);
        }

        Sprite FindSprite(string imageName)
        {
            foreach (var entry in cutscenes)
            {
                if (entry.name == imageName)
                    return entry.sprite;
            }
            return null;
        }
    }

    [System.Serializable]
    public class ЗаписьКатсцены
    {
        [Tooltip("Имя как в Yarn: cutscene show pond_bench")]
        public string name;
        public Sprite sprite;
    }
}