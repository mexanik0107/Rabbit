using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;       // Ссылка на игрока

    [Header("Settings")]
    public float smoothSpeed = 5f; // Насколько плавно камера догоняет игрока (чем больше, тем резче)

    [Header("Limits")]
    // Координаты границ, за которые центр камеры не может выехать
    public Vector2 minPosition;    // Левый нижний угол
    public Vector2 maxPosition;    // Правый верхний угол

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Определяем, куда камера ХОЧЕТ попасть (позиция игрока + сохраняем Z камеры)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);

        // 2. Ограничиваем эту позицию рамками (Clamp)
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

        // 3. Плавно перемещаем камеру к этой точке
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}