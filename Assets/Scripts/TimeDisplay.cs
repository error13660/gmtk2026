using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    void Update()
    {
        text.SetText("- " + (int)Player.Instance.timeRemaining + " -");
    }
}
