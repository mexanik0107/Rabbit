using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;

    [Header("Audio Settings")]
    // Перетащи сюда группу SFX
    public AudioMixerGroup sfxGroup;
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.5f;

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
        _audioSource = GetComponent<AudioSource>();

        // Привязка к микшеру
        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }
    }

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

        movement = movement.normalized;

        if (Mouse.current != null)
        {
            mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

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
            _stepTimer = 0;
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(footstepSounds[index]);
    }
}