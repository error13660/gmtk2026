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
            request.SetRequestHeader(
                "Accept",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (!IsSuccessful(request))
            {
                onError?.Invoke(
                    CreateErrorMessage(request)
                );

                yield break;
            }

            LeaderboardListResponse response;

            try
            {
                response =
                    JsonUtility.FromJson<LeaderboardListResponse>(
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
                onError?.Invoke(
                    "A szerver üres vagy hibás választ adott."
                );

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
        string encodedPlayerName =
            UnityWebRequest.EscapeURL(playerName);

        string url =
            apiUrl +
            "?player_name=" +
            encodedPlayerName;

        using (UnityWebRequest request =
               UnityWebRequest.Get(url))
        {
            request.SetRequestHeader(
                "Accept",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (!IsSuccessful(request))
            {
                onError?.Invoke(
                    CreateErrorMessage(request)
                );

                yield break;
            }

            LeaderboardEntryResponse response;

            try
            {
                response =
                    JsonUtility.FromJson<LeaderboardEntryResponse>(
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
                onError?.Invoke(
                    "A szerver üres vagy hibás választ adott."
                );

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
        int depth,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    )
    {
        Debug.Log(
            "[LeaderboardApiService] SavePlayer elindult. " +
            $"Depth: {depth}"
        );

        if (UserData.Instance == null)
        {
            const string error =
                "A UserData singleton nem található.";

            Debug.LogError(
                "[LeaderboardApiService] " + error
            );

            onError?.Invoke(error);
            yield break;
        }

        Debug.Log(
            "[LeaderboardApiService] UserData megtalálva. " +
            $"IsLoggedIn: {UserData.Instance.IsLoggedIn}, " +
            $"UserId: {UserData.Instance.UserId}, " +
            $"UserName: '{UserData.Instance.UserName}'"
        );

        if (!UserData.Instance.IsLoggedIn)
        {
            const string error =
                "A játékos nincs bejelentkezve.";

            Debug.LogError(
                "[LeaderboardApiService] " + error
            );

            onError?.Invoke(error);
            yield break;
        }

        if (UserData.Instance.UserId <= 0)
        {
            const string error =
                "A bejelentkezett játékos UserId értéke hibás.";

            Debug.LogError(
                "[LeaderboardApiService] " + error
            );

            onError?.Invoke(error);
            yield break;
        }

        if (string.IsNullOrWhiteSpace(
                UserData.Instance.UserName
            ))
        {
            const string error =
                "A bejelentkezett játékos neve üres.";

            Debug.LogError(
                "[LeaderboardApiService] " + error
            );

            onError?.Invoke(error);
            yield break;
        }

        if (depth < 0)
        {
            const string error =
                "A depth nem lehet negatív.";

            Debug.LogError(
                "[LeaderboardApiService] " + error
            );

            onError?.Invoke(error);
            yield break;
        }

        LeaderboardEntryRequest payload =
            new LeaderboardEntryRequest(
                UserData.Instance.UserId,
                UserData.Instance.UserName,
                depth
            );

        string json = JsonUtility.ToJson(
            payload,
            true
        );

        Debug.Log(
            "[LeaderboardApiService] PUT kérés indítása.\n" +
            $"URL: {apiUrl}\n" +
            $"JSON:\n{json}"
        );

        byte[] body =
            Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                apiUrl,
                UnityWebRequest.kHttpVerbPUT
            );

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json; charset=utf-8"
        );

        request.SetRequestHeader(
            "Accept",
            "application/json"
        );

        yield return request.SendWebRequest();

        string responseBody =
            request.downloadHandler != null
                ? request.downloadHandler.text
                : string.Empty;

        Debug.Log(
            "[LeaderboardApiService] PUT kérés befejezõdött.\n" +
            $"HTTP státusz: {request.responseCode}\n" +
            $"Request result: {request.result}\n" +
            $"Request error: {request.error}\n" +
            $"Szerver válasza:\n{responseBody}"
        );

        if (!IsSuccessful(request))
        {
            string error =
                CreateErrorMessage(request);

            Debug.LogError(
                "[LeaderboardApiService] SavePlayer HTTP hiba:\n" +
                error
            );

            onError?.Invoke(error);
            yield break;
        }

        LeaderboardEntryResponse response;

        try
        {
            response =
                JsonUtility.FromJson<LeaderboardEntryResponse>(
                    responseBody
                );
        }
        catch (Exception exception)
        {
            string error =
                "Nem sikerült feldolgozni a szerver válaszát: " +
                exception.Message;

            Debug.LogError(
                "[LeaderboardApiService] JSON feldolgozási hiba.\n" +
                $"Válasz:\n{responseBody}\n" +
                $"Exception: {exception}"
            );

            onError?.Invoke(error);
            yield break;
        }

        if (response == null)
        {
            const string error =
                "A szerver üres vagy hibás választ adott.";

            Debug.LogError(
                "[LeaderboardApiService] " + error +
                "\nVálasz:\n" +
                responseBody
            );

            onError?.Invoke(error);
            yield break;
        }

        Debug.Log(
            "[LeaderboardApiService] Feldolgozott API válasz. " +
            $"Success: {response.success}, " +
            $"Message: '{response.message}'"
        );

        if (!response.success)
        {
            string error =
                string.IsNullOrWhiteSpace(response.message)
                    ? "A szerver sikertelen mentést jelzett."
                    : response.message;

            Debug.LogError(
                "[LeaderboardApiService] API hiba: " +
                error
            );

            onError?.Invoke(error);
            yield break;
        }

        if (response.data != null)
        {
            Debug.Log(
                "[LeaderboardApiService] Mentett játékos:\n" +
                $"Id: {response.data.id}\n" +
                $"UserId: {response.data.player_id}\n" +
                $"PlayerName: {response.data.player_name}\n" +
                $"Depth: {response.data.depth}\n" +
                $"UpdatedAt: {response.data.updated_at}"
            );
        }
        else
        {
            Debug.LogWarning(
                "[LeaderboardApiService] A mentés sikeres volt, " +
                "de a válasz data mezõje üres."
            );
        }

        Debug.Log(
            "[LeaderboardApiService] SavePlayer sikeresen befejezõdött."
        );

        onSuccess?.Invoke(response);
    }

    private static bool IsSuccessful(
        UnityWebRequest request
    )
    {
#if UNITY_2020_1_OR_NEWER
        return request.result ==
               UnityWebRequest.Result.Success;
#else
        return !request.isNetworkError &&
               !request.isHttpError;
#endif
    }

    private static string CreateErrorMessage(
        UnityWebRequest request
    )
    {
        string responseBody =
            request.downloadHandler != null
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