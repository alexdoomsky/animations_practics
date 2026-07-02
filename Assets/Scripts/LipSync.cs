using UnityEngine;

public class LipSync : MonoBehaviour
{
    public AudioSource voiceSource; // выход на AudioSource
    public AudioClip[] speakClips;  // массив клипов речи

    void Start()
    {
        Debug.Log("LipSync скрипт активирован на объекте: " + gameObject.name);

        // Проверяем, есть ли AudioSource
        if (voiceSource == null)
        {
            voiceSource = GetComponent<AudioSource>();
            if (voiceSource == null)
            {
                Debug.LogError("Нет AudioSource! Добавьте компонент AudioSource к объекту " + gameObject.name);
            }
            else
            {
                Debug.Log("AudioSource найден автоматически");
            }
        }
    }

    public void PlayRandomSpeakSound()
    {
        Debug.Log("Метод PlayRandomSpeakSound ВЫЗВАН на объекте " + gameObject.name);

        if (voiceSource == null)
        {
            Debug.LogError("voiceSource == null! Прикрепите AudioSource в инспекторе");
            return;
        }

        if (speakClips == null || speakClips.Length == 0)
        {
            Debug.LogError("Нет аудиоклипов! Добавьте звуки в массив speakClips");
            return;
        }

        if (!voiceSource.isPlaying)
        {
            AudioClip clip = speakClips[Random.Range(0, speakClips.Length)];
            Debug.Log("Пытаюсь проиграть клип: " + (clip != null ? clip.name : "null"));

            if (clip != null)
            {
                voiceSource.PlayOneShot(clip);
                Debug.Log("Звук запущен!");
            }
            else
            {
                Debug.LogError("Клип в массиве = null");
            }
        }
        else
        {
            Debug.Log("AudioSource уже играет, пропускаем");
        }
    }

    public void StopSpeakSound()
    {
        Debug.Log("Метод StopSpeakSound ВЫЗВАН");
        if (voiceSource != null)
            voiceSource.Stop();
    }
}

