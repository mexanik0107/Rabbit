using UnityEngine;
using UnityEngine.InputSystem; // ВАЖНО: Подключаем новую систему

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;

    [Header("References")]
    public Camera cam;
    public Rigidbody2D rb;

    private Vector2 movement;
    private Vector2 mousePos;

    [HideInInspector] public float speedMultiplier = 1f;

    void Update()
    {
        // === ИСПРАВЛЕНИЕ ПОД NEW INPUT SYSTEM ===

        // 1. Считываем клавиатуру напрямую
        movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1;
            if (Keyboard.current.dKey.isPressed) movement.x += 1;
        }

        // Нормализуем
        movement = movement.normalized;

        // 2. Считываем мышь напрямую
        if (Mouse.current != null)
        {
            mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }

    void FixedUpdate()
    {
        // Здесь всё без изменений
        float currentSpeed = moveSpeed * speedMultiplier;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}