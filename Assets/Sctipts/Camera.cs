using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    public Transform player; // За кем следим

    [Header("Настройки")]
    public float smoothSpeed = 5f; // Скорость сглаживания (Lerp)

    [Header("Границы")]
    public Vector2 minPosition;    // Левый нижний угол карты
    public Vector2 maxPosition;    // Правый верхний угол карты

    // LateUpdate вызывается ПОСЛЕ всех Update. Это важно для камеры, 
    // чтобы она двигалась после того, как игрок уже закончил движение в этом кадре.
    void LateUpdate()
    {
        if (player == null) return;

        // Целевая позиция: X и Y игрока, но Z камеры оставляем прежним (чтобы не провалиться сквозь фон)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);

        // Clamp ограничивает значения, не давая камере выйти за min/max координаты
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

        // Vector3.Lerp плавно интерполирует текущую позицию к целевой
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}