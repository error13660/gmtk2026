using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardApiService : ILeaderboardApiService
{
    private readonly string apiUrl;

    public LeaderboardApiService(string apiUrl)
    {
        this.apiUrl = apiUrl.TrimEnd('/');
    }

    public IEnumerator GetLeaderboard(
        Action<LeaderboardListResponse> onSuccess,
        Action<string> onError
    )
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (!IsSuccessful(request))
            {
                onError?.Invoke(CreateErrorMessage(request));
                yield break;
            }

            LeaderboardListResponse response;

            try
            {
                response = JsonUtility.FromJson<LeaderboardListResponse>(
                    request.downloadHandler.text
                );
            }
            catch (Exception exception)
            {
                onError?.Invoke(
                    "Nem sikerült feldolgozni a szerver válaszát: " +
                    exception.Message
                );

                yield break;
            }

            if (response == null)
            {
                onError?.Invoke("A szerver üres vagy hibás választ adott.");
                yield break;
            }

            if (!response.success)
            {
                onError?.Invoke(response.message);
                yield break;
            }

            onSuccess?.Invoke(response);
        }
    }

    public IEnumerator GetPlayer(
        string playerName,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    )
    {
        string encodedPlayerName = UnityWebRequest.EscapeURL(playerName);
        string url = apiUrl + "?player_name=" + encodedPlayerName;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (!IsSuccessful(request))
            {
                onError?.Invoke(CreateErrorMessage(request));
                yield break;
            }

            LeaderboardEntryResponse response;

            try
            {
                response = JsonUtility.FromJson<LeaderboardEntryResponse>(
                    request.downloadHandler.text
                );
            }
            catch (Exception exception)
            {
                onError?.Invoke(
                    "Nem sikerült feldolgozni a szerver válaszát: " +
                    exception.Message
                );

                yield break;
            }

            if (response == null)
            {
                onError?.Invoke("A szerver üres vagy hibás választ adott.");
                yield break;
            }

            if (!response.success)
            {
                onError?.Invoke(response.message);
                yield break;
            }

            onSuccess?.Invoke(response);
        }
    }

    public IEnumerator SavePlayer(
        string playerName,
        int score,
        int depth,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    )
    {
        LeaderboardEntryRequest payload = new LeaderboardEntryRequest(
            playerName,
            score,
            depth
        );

        string json = JsonUtility.ToJson(payload);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            request.SetRequestHeader(
                "Accept",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (!IsSuccessful(request))
            {
                onError?.Invoke(CreateErrorMessage(request));
                yield break;
            }

            LeaderboardEntryResponse response;

            try
            {
                response = JsonUtility.FromJson<LeaderboardEntryResponse>(
                    request.downloadHandler.text
                );
            }
            catch (Exception exception)
            {
                onError?.Invoke(
                    "Nem sikerült feldolgozni a szerver válaszát: " +
                    exception.Message
                );

                yield break;
            }

            if (response == null)
            {
                onError?.Invoke("A szerver üres vagy hibás választ adott.");
                yield break;
            }

            if (!response.success)
            {
                onError?.Invoke(response.message);
                yield break;
            }

            onSuccess?.Invoke(response);
        }
    }

    private static bool IsSuccessful(UnityWebRequest request)
    {
#if UNITY_2020_1_OR_NEWER
        return request.result == UnityWebRequest.Result.Success;
#else
        return !request.isNetworkError && !request.isHttpError;
#endif
    }

    private static string CreateErrorMessage(UnityWebRequest request)
    {
        string responseBody = request.downloadHandler != null
            ? request.downloadHandler.text
            : string.Empty;

        return
            "API hiba. HTTP státusz: " +
            request.responseCode +
            ", hiba: " +
            request.error +
            ", válasz: " +
            responseBody;
    }
}