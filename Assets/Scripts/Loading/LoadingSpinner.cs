using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 250f;

    private void Update()
    {
        transform.Rotate(
            0f,
            0f,
            -rotationSpeed * Time.unscaledDeltaTime
        );
    }
}