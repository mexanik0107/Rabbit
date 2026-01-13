using UnityEngine;
using UnityEngine.InputSystem; // Используем новую систему ввода
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 10f;

    [Header("Звук")]
    public AudioMixerGroup sfxGroup;
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.5f; // Как часто играть звук шага

    [Header("Ссылки")]
    public Camera cam;
    public Rigidbody2D rb;

    private Vector2 movement;
    private Vector2 mousePos;
    private AudioSource _audioSource;
    private float _stepTimer;

    // Скрыт в инспекторе, но доступен для других скриптов (например, для бонусов скорости)
    [HideInInspector] public float speedMultiplier = 1f;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }
    }

    // Update используется для чтения ввода (Input)
    void Update()
    {
        movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1;
            if (Keyboard.current.dKey.isPressed) movement.x += 1;
        }

        // Нормализация вектора, чтобы движение по диагонали не было быстрее
        movement = movement.normalized;

        if (Mouse.current != null)
        {
            // Переводим координаты мыши из экранных (пиксели) в мировые
            mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        HandleFootsteps();
    }

    // FixedUpdate используется для физики (движение Rigidbody)
    void FixedUpdate()
    {
        float currentSpeed = moveSpeed * speedMultiplier;

        // MovePosition перемещает физическое тело с учетом коллизий
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);

        // Поворот игрока в сторону мыши
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private void HandleFootsteps()
    {
        // sqrMagnitude — быстрая проверка длины вектора (быстрее, чем magnitude)
        // Если мы двигаемся
        if (movement.sqrMagnitude > 0.1f)
        {
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0)
            {
                PlayRandomFootstep();
                _stepTimer = stepInterval;
            }
        }
        else
        {
            // Сбрасываем таймер, если остановились
            _stepTimer = 0;
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);

        // Немного меняем питч (высоту тона), чтобы звуки не казались пулеметной очередью
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(footstepSounds[index]);
    }
}