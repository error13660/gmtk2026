using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float attraction = 1;

    private void Update()
    {
        Vector2 playerV2 = player.position;
        Vector2 cameraV2 = transform.position;
        float distance = Vector2.Distance(playerV2, cameraV2);
        if (distance < .5) return;

        Vector2 direction = (playerV2 - cameraV2).normalized;
        Vector2 offset = direction * (attraction * distance * Time.deltaTime);
        transform.position = transform.position + (Vector3)offset;
    }

}
