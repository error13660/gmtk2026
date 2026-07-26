using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private Camera camera;
    private Vector3 basePosition;

    private void Awake()
    {
        basePosition = transform.position;
    }

    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.Equals(this.gameObject))
            {
                OnHover();
                if (Input.GetKeyDown(KeyCode.Mouse0)) OnCLick();
            }
        }
        else
        {
            transform.position = basePosition;
        }
    }

    void OnCLick()
    {
        SceneManager.LoadSceneAsync(1);
    }

    void OnHover()
    {
        transform.position = basePosition + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
    }
}
