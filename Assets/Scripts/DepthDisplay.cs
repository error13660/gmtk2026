using TMPro;
using UnityEngine;

public class DepthDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    void Update()
    {
        text.SetText("- " + (int)Player.Instance.depth + "m -");
    }
}
