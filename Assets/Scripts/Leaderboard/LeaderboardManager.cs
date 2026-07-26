using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    [SerializeField] private LeaderboardRowUI rowPrefab;
    [SerializeField] private Transform tableContent;

    [Header("API")]
    [SerializeField]
    private string apiUrl =
        "https://dzsepetto.hu/gmtk_api/gmtk_api.php";

    private readonly List<LeaderboardRowUI> createdRows = new();

    private IEnumerator Start()
    {
        yield return LoadLeaderboardFromApi();
    }

    public void RefreshLeaderboard()
    {
        StartCoroutine(LoadLeaderboardFromApi());
    }

    private IEnumerator LoadLeaderboardFromApi()
    {
        yield return new WaitForSecondsRealtime(2f);

        LoadingService.Instance.Show();

        yield return new WaitForSecondsRealtime(5f);

        using UnityWebRequest request =
            UnityWebRequest.Get(apiUrl);

        request.SetRequestHeader(
            "Accept",
            "application/json"
        );

        yield return request.SendWebRequest();

        try
        {
            if (request.result !=
                UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"Nem sikerült lekérni a leaderboardot. " +
                    $"HTTP: {request.responseCode}, " +
                    $"hiba: {request.error}, " +
                    $"válasz: {request.downloadHandler.text}"
                );

                yield break;
            }

            string json =
                request.downloadHandler.text;

            LeaderboardListResponse response =
                JsonUtility.FromJson<LeaderboardListResponse>(
                    json
                );

            if (response == null)
            {
                Debug.LogError(
                    "Az API válasza üres vagy hibás."
                );

                yield break;
            }

            if (!response.success)
            {
                Debug.LogError(
                    $"Az API hibát adott vissza: " +
                    $"{response.message}"
                );

                yield break;
            }

            if (response.data == null)
            {
                ClearLeaderboard();
                yield break;
            }

            ShowLeaderboard(response.data);
        }
        finally
        {
            LoadingService.Instance.Hide();
        }
    }

    private void ShowLeaderboard(
        LeaderboardEntry[] players
    )
    {
        ClearLeaderboard();

        List<LeaderboardEntry> sortedPlayers =
            new List<LeaderboardEntry>(players);

        sortedPlayers.Sort(
            (firstPlayer, secondPlayer) =>
                secondPlayer.depth.CompareTo(
                    firstPlayer.depth
                )
        );

        for (
            int index = 0;
            index < sortedPlayers.Count;
            index++
        )
        {
            LeaderboardRowUI newRow =
                Instantiate(
                    rowPrefab,
                    tableContent
                );

            int position = index + 1;

            newRow.SetData(
                position,
                sortedPlayers[index]
            );

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