using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;
    public bool rotateWithPlayer = false; // ¬ращать ли миникарту?

    void LateUpdate()
    {
        if (player == null) return;

        //  опируем позицию игрока
        Vector3 newPosition = player.position;
        // Ќо высоту (Z) оставл€ем свою, чтобы камера "висела" над полем
        newPosition.z = transform.position.z;
        transform.position = newPosition;

        if (rotateWithPlayer)
        {
            // ≈сли нужно, поворачиваем камеру вслед за поворотом игрока
            transform.rotation = Quaternion.Euler(0, 0, player.eulerAngles.z);
        }
    }
}