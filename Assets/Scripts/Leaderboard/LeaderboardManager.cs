using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardManager : MonoBehaviour
{

    [Header("Leaderboard UI")]
    [SerializeField] private LeaderboardRowUI rowPrefab;
    [SerializeField] private Transform tableContent;

    [Header("JSON")]
    [SerializeField] private string jsonFileName = "leaderboard.json";

    private readonly List<LeaderboardRowUI> createdRows = new();

    private IEnumerator Start()
    {
        yield return LoadLeaderboardFromJson();
    }

    private IEnumerator LoadLeaderboardFromJson()
    {
        string filePath = Path.Combine(
            Application.dataPath,
            "Scripts",
            "Leaderboard",
            jsonFileName
        );

        string json;

        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            using UnityWebRequest request = UnityWebRequest.Get(filePath);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"Nem sikerült betölteni a JSON fájlt: {request.error}"
                );

                yield break;
            }

            json = request.downloadHandler.text;
        }
        else
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError(
                    $"Nem található a JSON fájl: {filePath}"
                );

                yield break;
            }

            json = File.ReadAllText(filePath);
        }

        LeaderboardData leaderboardData =
            JsonUtility.FromJson<LeaderboardData>(json);

        if (
            leaderboardData == null ||
            leaderboardData.players == null
        )
        {
            Debug.LogError(
                "A leaderboard JSON formátuma nem megfelelõ."
            );

            yield break;
        }

        ShowLeaderboard(leaderboardData.players);
    }

    private void ShowLeaderboard(
        List<LeaderboardPlayerData> players
    )
    {
        ClearLeaderboard();

        players.Sort(
            (firstPlayer, secondPlayer) =>
                secondPlayer.score.CompareTo(firstPlayer.score)
        );

        for (int index = 0; index < players.Count; index++)
        {
            LeaderboardRowUI newRow = Instantiate(
                rowPrefab,
                tableContent
            );

            int position = index + 1;

            newRow.SetData(position, players[index]);

            createdRows.Add(newRow);
        }
    }

    private void ClearLeaderboard()
    {
        foreach (LeaderboardRowUI row in createdRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }

        createdRows.Clear();
    }

}
