using UnityEngine;

public class UpgradeGuiRaycaster : MonoBehaviour
{
    Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        UpgradeGui ugui;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (ugui = hit.collider.gameObject.GetComponent<UpgradeGui>())
            {
                ugui.OnHover();
                if(Input.GetKeyDown(KeyCode.Mouse0)) ugui.OnClick();
            }
        }
    }
}
