using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // ѕеретащи сюда игрока

    // ≈сли хочешь, чтобы миникарта вращалась вместе с игроком, поставь true
    public bool rotateWithPlayer = false;

    void LateUpdate()
    {
        if (player == null) return;

        // —ледим за позицией игрока, но высоту (z) оставл€ем свою (-10 по стандарту)
        Vector3 newPosition = player.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;

        if (rotateWithPlayer)
        {
            // ¬ращаем камеру так же, как игрока (вокруг оси Z)
            transform.rotation = Quaternion.Euler(0, 0, player.eulerAngles.z);
        }
    }
}