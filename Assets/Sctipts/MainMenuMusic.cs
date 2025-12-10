using UnityEngine;
using UnityEngine.Audio; // Нужно для работы с микшером

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Список треков для случайного воспроизведения")]
    public AudioClip[] playlist;

    [Header("Audio Mixing")]
    [Tooltip("Перетащи сюда группу 'Music' из Audio Mixer")]
    public AudioMixerGroup musicOutputGroup;

    private AudioSource _audioSource;
    private int _lastTrackIndex = -1;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // Сразу настраиваем AudioSource
        _audioSource.playOnAwake = false;
        _audioSource.loop = false; // Мы будем менять треки сами, поэтому авто-повтор выключаем

        // Автоматически назначаем группу микшера, если она указана в инспекторе
        if (musicOutputGroup != null)
        {
            _audioSource.outputAudioMixerGroup = musicOutputGroup;
        }
    }

    void Start()
    {
        PlayRandomTrack();
    }

    void Update()
    {
        // Если музыка закончилась (и игра не на паузе), включаем следующую
        if (!_audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (playlist == null || playlist.Length == 0) return;

        int newIndex;

        // Если у нас больше 1 трека, стараемся не играть один и тот же два раза подряд
        if (playlist.Length > 1)
        {
            do
            {
                newIndex = Random.Range(0, playlist.Length);
            }
            while (newIndex == _lastTrackIndex);
        }
        else
        {
            newIndex = 0;
        }

        _lastTrackIndex = newIndex;

        _audioSource.clip = playlist[newIndex];
        _audioSource.Play();
    }
}