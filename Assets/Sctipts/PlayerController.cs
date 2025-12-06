using UnityEngine;
using UnityEngine.InputSystem;

// Добавляем требование компонента, чтобы избежать ошибок null reference
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;

    [Header("Audio Settings")]
    public AudioClip[] footstepSounds; // Массив звуков для разнообразия
    public float stepInterval = 0.5f;  // Как часто играть звук (чем меньше, тем чаще)

    [Header("References")]
    public Camera cam;
    public Rigidbody2D rb;

    private Vector2 movement;
    private Vector2 mousePos;
    private AudioSource _audioSource;
    private float _stepTimer;

    [HideInInspector] public float speedMultiplier = 1f;

    void Awake()
    {
        // Кэшируем компонент при инициализации
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 1. Считываем клавиатуру
        movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1;
            if (Keyboard.current.dKey.isPressed) movement.x += 1;
        }

        movement = movement.normalized;

        // 2. Считываем мышь
        if (Mouse.current != null)
        {
            mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // 3. Логика звука шагов
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed * speedMultiplier;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private void HandleFootsteps()
    {
        // Если игрок двигается (вектор движения не равен нулю)
        if (movement.sqrMagnitude > 0.1f)
        {
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0)
            {
                PlayRandomFootstep();
                _stepTimer = stepInterval; // Сброс таймера
            }
        }
        else
        {
            // Сбрасываем таймер, чтобы при начале движения звук был сразу
            _stepTimer = 0;
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepSounds.Length == 0) return;

        // Выбираем случайный звук из массива для естественности
        int index = Random.Range(0, footstepSounds.Length);

        // Изменяем высоту тона (pitch) немного, чтобы звуки не казались роботоподобными
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(footstepSounds[index]);
    }
}