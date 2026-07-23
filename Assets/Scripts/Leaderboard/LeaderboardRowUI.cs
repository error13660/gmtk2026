using TMPro;
using UnityEngine;

public class LeaderboardRowUI : MonoBehaviour
{
    [Header("Szöveges mezõk")]
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text depthText;

    public void SetData(
    int position,
    LeaderboardEntry player
)
    {
        positionText.text = position.ToString();
        playerNameText.text = player.player_name;
        scoreText.text = player.score.ToString();
        depthText.text = player.depth.ToString();
    }

}
