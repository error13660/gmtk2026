using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private Camera camera;

    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.Equals(this.gameObject))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0)) OnCLick();
            }
        }
    }

    void OnCLick()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
