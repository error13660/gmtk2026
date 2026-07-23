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
        LeaderboardPlayerData playerData
    )
    {
        if (playerData == null)
        {
            Debug.LogError("A játékos adata nem lehet null.");
            return;
        }

        positionText.text = position.ToString();
        playerNameText.text = playerData.playerName;
        scoreText.text = playerData.score.ToString("N0");
        depthText.text = $"{playerData.depth} m";
    }

}
