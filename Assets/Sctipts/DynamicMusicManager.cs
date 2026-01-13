using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class DynamicMusicManager : MonoBehaviour
{
    [Header("Audio Mixing")]
    public AudioMixerGroup musicOutputGroup; // Группа Music в микшере

    [Header("Target")]
    public PlayerCuteness playerCuteness; // Ссылка на скрипт состояния игрока

    [Header("Пороги (Гистерезис)")]
    // Гистерезис нужен, чтобы музыка не "дребезжала", если значение колеблется около 50.
    // Переключаемся на "Милую", только если > 60. Обратно на "Грубую", только если < 40.
    public float switchToCuteThreshold = 60f;
    public float switchToRoughThreshold = 40f;

    [Header("Переход")]
    public float fadeDuration = 2.0f; // Время кроссфейда
    [Range(0f, 1f)] public float maxVolume = 0.5f;

    [Header("Библиотека музыки")]
    public AudioClip[] roughTracks; // Треки для низкой милоты
    public AudioClip[] cuteTracks;  // Треки для высокой милоты

    // Два источника звука для плавного перетекания одного в другой
    private AudioSource _sourceRough;
    private AudioSource _sourceCute;

    // Флаг текущего состояния
    private bool _isPlayingCute;

    // Ссылка на активную корутину перехода (чтобы можно было прервать)
    private Coroutine _fadeCoroutine;

    void Start()
    {
        if (playerCuteness == null)
            playerCuteness = FindObjectOfType<PlayerCuteness>();

        // Программно создаем два AudioSource
        _sourceRough = CreateSource("AudioSource_Rough");
        _sourceCute = CreateSource("AudioSource_Cute");

        // Определяем, какую музыку играть на старте
        if (playerCuteness != null)
        {
            float currentVal = playerCuteness.CurrentCuteness;
            _isPlayingCute = currentVal > switchToCuteThreshold;
        }
        else
        {
            _isPlayingCute = false;
        }

        // Запускаем воспроизведение
        PlayRandomTrack(_sourceRough, roughTracks);
        PlayRandomTrack(_sourceCute, cuteTracks);

        // Выставляем начальную громкость (один играет, другой заглушен)
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

        // Логика переключения с учетом гистерезиса
        if (!_isPlayingCute && currentCuteness > switchToCuteThreshold)
        {
            SwitchToCute();
        }
        else if (_isPlayingCute && currentCuteness < switchToRoughThreshold)
        {
            SwitchToRough();
        }

        // Если трек закончился, запускаем следующий случайный
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

    // Корутина плавного изменения громкости (кроссфейд)
    private IEnumerator Crossfade(AudioSource toOn, AudioSource toOff)
    {
        float timer = 0f;
        float startVolOn = toOn.volume;
        float startVolOff = toOff.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Lerp интерполирует значение от start до target
            toOn.volume = Mathf.Lerp(startVolOn, maxVolume, t);
            toOff.volume = Mathf.Lerp(startVolOff, 0f, t);

            yield return null; // Ждем следующий кадр
        }

        // Гарантируем финальные значения
        toOn.volume = maxVolume;
        toOff.volume = 0f;
    }

    private AudioSource CreateSource(string goName)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(this.transform);
        AudioSource src = go.AddComponent<AudioSource>();

        if (musicOutputGroup != null)
        {
            src.outputAudioMixerGroup = musicOutputGroup;
        }

        src.loop = false; // Мы сами контролируем луп через код
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
        // Если музыка не играет и игра не на паузе (TimeScale > 0)
        if (!source.isPlaying && Time.timeScale > 0)
        {
            PlayRandomTrack(source, clips);
        }
    }
}