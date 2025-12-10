using UnityEngine;
using System.Collections;
using UnityEngine.Audio; // ВАЖНО: Добавлено пространство имен для микшера

public class DynamicMusicManager : MonoBehaviour
{
    [Header("Audio Mixing")]
    // Сюда перетащи группу "Music" из твоего Audio Mixer
    public AudioMixerGroup musicOutputGroup;

    [Header("Target")]
    public PlayerCuteness playerCuteness;

    [Header("Thresholds (Hysteresis)")]
    [Tooltip("Если милота ВЫШЕ этого числа -> включаем Милую музыку")]
    public float switchToCuteThreshold = 60f;

    [Tooltip("Если милота НИЖЕ этого числа -> включаем Грубую музыку")]
    public float switchToRoughThreshold = 40f;

    [Header("Transition")]
    [Tooltip("Скорость плавного перехода (сек)")]
    public float fadeDuration = 2.0f;
    [Range(0f, 1f)] public float maxVolume = 0.5f;

    [Header("Music Library")]
    public AudioClip[] roughTracks; // Грубая музыка (для низкой милоты)
    public AudioClip[] cuteTracks;  // Милая музыка (для высокой милоты)

    // Два источника звука для кроссфейда (плавного перетекания)
    private AudioSource _sourceRough;
    private AudioSource _sourceCute;

    // Текущее состояние: true = играет милая, false = играет грубая
    private bool _isPlayingCute;

    // Чтобы не запускать корутину перехода много раз подряд
    private Coroutine _fadeCoroutine;

    void Start()
    {
        // 1. Если игрока не назначили вручную, ищем на сцене
        if (playerCuteness == null)
            playerCuteness = FindObjectOfType<PlayerCuteness>();

        // 2. Создаем два AudioSource программно
        _sourceRough = CreateSource("AudioSource_Rough");
        _sourceCute = CreateSource("AudioSource_Cute");

        // 3. Определяем начальное состояние
        if (playerCuteness != null)
        {
            float currentVal = playerCuteness.CurrentCuteness;
            _isPlayingCute = currentVal > switchToCuteThreshold;
        }
        else
        {
            _isPlayingCute = false;
        }

        // 4. Запускаем треки
        PlayRandomTrack(_sourceRough, roughTracks);
        PlayRandomTrack(_sourceCute, cuteTracks);

        // 5. Устанавливаем начальную громкость
        if (_isPlayingCute)
        {
            _sourceCute.volume = maxVolume;
            _sourceRough.volume = 0f;
        }
        else
        {
            _sourceCute.volume = 0f;
            _sourceRough.volume = maxVolume;
        }
    }

    void Update()
    {
        if (playerCuteness == null) return;

        float currentCuteness = playerCuteness.CurrentCuteness;

        // --- ЛОГИКА ГИСТЕРЕЗИСА ---
        if (!_isPlayingCute && currentCuteness > switchToCuteThreshold)
        {
            SwitchToCute();
        }
        else if (_isPlayingCute && currentCuteness < switchToRoughThreshold)
        {
            SwitchToRough();
        }

        CheckAndLoop(_sourceRough, roughTracks);
        CheckAndLoop(_sourceCute, cuteTracks);
    }

    private void SwitchToCute()
    {
        _isPlayingCute = true;
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(Crossfade(_sourceCute, _sourceRough));
    }

    private void SwitchToRough()
    {
        _isPlayingCute = false;
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(Crossfade(_sourceRough, _sourceCute));
    }

    private IEnumerator Crossfade(AudioSource toOn, AudioSource toOff)
    {
        float timer = 0f;
        float startVolOn = toOn.volume;
        float startVolOff = toOff.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            toOn.volume = Mathf.Lerp(startVolOn, maxVolume, t);
            toOff.volume = Mathf.Lerp(startVolOff, 0f, t);

            yield return null;
        }

        toOn.volume = maxVolume;
        toOff.volume = 0f;
    }

    // Создание AudioSource "на лету"
    private AudioSource CreateSource(string goName)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(this.transform);
        AudioSource src = go.AddComponent<AudioSource>();

        // --- ИСПРАВЛЕНИЕ ЗДЕСЬ ---
        // Назначаем Output группу микшера, если она задана в инспекторе
        if (musicOutputGroup != null)
        {
            src.outputAudioMixerGroup = musicOutputGroup;
        }
        // -------------------------

        src.loop = false;
        src.playOnAwake = false;
        return src;
    }

    private void PlayRandomTrack(AudioSource source, AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        source.Play();
    }

    private void CheckAndLoop(AudioSource source, AudioClip[] clips)
    {
        if (!source.isPlaying && Time.timeScale > 0)
        {
            PlayRandomTrack(source, clips);
        }
    }
}