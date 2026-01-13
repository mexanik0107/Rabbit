using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    [Header("Settings")]
    public AudioClip[] playlist; // Список треков

    [Header("Audio Mixing")]
    public AudioMixerGroup musicOutputGroup;

    private AudioSource _audioSource;
    private int _lastTrackIndex = -1; // Запоминаем последний трек, чтобы не повторяться

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

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
        if (!_audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (playlist == null || playlist.Length == 0) return;

        int newIndex;

        // Алгоритм выбора трека (избегает повторений подряд)
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