using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ItchOAuthLogin : MonoBehaviour
{
    [Header("itch.io OAuth")]

    [SerializeField]
    private string clientId = "IDE_JON_A_CLIENT_ID";

    [SerializeField]
    private string callbackUrl =
        "https://dzsepetto.hu/gmtk_api/callback.html";

    [Header("Saját szerver")]

    [SerializeField]
    private string loginStatusUrl =
        "https://dzsepetto.hu/gmtk_api/itch_login_status.php";

    [Header("Polling beállítások")]

    [SerializeField]
    private float checkIntervalSeconds = 2f;

    [SerializeField]
    private float loginTimeoutSeconds = 300f;

    [Header("Bejelentkezett itch.io felhasználó")]

    [SerializeField]
    private long itchUserId;

    [SerializeField]
    private string itchUsername;

    [SerializeField]
    private string itchDisplayName;

    [SerializeField]
    private bool isLoggedIn;

    [SerializeField]
    private bool isLoginInProgress;

    private Coroutine pollingCoroutine;

    public long ItchUserId => itchUserId;
    public string ItchUsername => itchUsername;
    public string ItchDisplayName => itchDisplayName;
    public bool IsLoggedIn => isLoggedIn;

    public void LoginWithItch()
    {
        Debug.Log("LoginWithItch meghívva.");

        if (isLoginInProgress)
        {
            Debug.LogWarning(
                "Már folyamatban van egy itch.io bejelentkezés."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(clientId) ||
            clientId == "IDE_JON_A_CLIENT_ID")
        {
            Debug.LogError(
                "Nincs megfelelõ itch.io Client ID megadva."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            Debug.LogError(
                "Nincs callback URL megadva."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(loginStatusUrl))
        {
            Debug.LogError(
                "Nincs login status API URL megadva."
            );

            return;
        }

        ResetLoginData();

        string state = Guid.NewGuid().ToString("N");

        PlayerPrefs.SetString(
            "itch_oauth_state",
            state
        );

        PlayerPrefs.Save();

        string authorizationUrl =
            "https://itch.io/user/oauth" +
            "?client_id=" +
            UnityWebRequest.EscapeURL(clientId) +
            "&scope=" +
            UnityWebRequest.EscapeURL("profile:me") +
            "&redirect_uri=" +
            UnityWebRequest.EscapeURL(callbackUrl) +
            "&response_type=token" +
            "&state=" +
            UnityWebRequest.EscapeURL(state);

        Debug.Log(
            "OAuth URL elkészült:\n" +
            authorizationUrl
        );

        isLoginInProgress = true;

        /*
         * Elindítjuk a pollingot még az OAuth oldal
         * megnyitása elõtt.
         */
        pollingCoroutine = StartCoroutine(
            CheckLoginResult(state)
        );

        Application.OpenURL(authorizationUrl);

        Debug.Log(
            "Az itch.io bejelentkezési oldal megnyitva."
        );
    }

    private IEnumerator CheckLoginResult(string state)
    {
        float elapsedTime = 0f;

        Debug.Log(
            "Várakozás az itch.io bejelentkezés eredményére..."
        );

        while (elapsedTime < loginTimeoutSeconds)
        {
            yield return new WaitForSecondsRealtime(
                checkIntervalSeconds
            );

            elapsedTime += checkIntervalSeconds;

            string requestUrl =
                loginStatusUrl +
                "?state=" +
                UnityWebRequest.EscapeURL(state);

            using UnityWebRequest request =
                UnityWebRequest.Get(requestUrl);

            /*
             * Megakadályozzuk, hogy a böngészõ egy korábbi
             * választ cache-bõl adjon vissza.
             */
            request.SetRequestHeader(
                "Cache-Control",
                "no-cache"
            );

            yield return request.SendWebRequest();

            string responseBody =
                request.downloadHandler != null
                    ? request.downloadHandler.text
                    : string.Empty;

            if (request.responseCode == 410)
            {
                Debug.LogError(
                    "Az itch.io bejelentkezés lejárt."
                );

                FinishLoginAttempt();
                yield break;
            }

            if (request.result !=
                UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    "Login státusz lekérési hiba.\n" +
                    "HTTP: " + request.responseCode + "\n" +
                    "Hiba: " + request.error + "\n" +
                    "Válasz: " + responseBody
                );

                /*
                 * Átmeneti hálózati hiba esetén nem állunk le,
                 * hanem a következõ körben újrapróbáljuk.
                 */
                continue;
            }

            ItchLoginStatusResponse response;

            try
            {
                response =
                    JsonUtility.FromJson<ItchLoginStatusResponse>(
                        responseBody
                    );
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "A status API válasza nem dolgozható fel:\n" +
                    responseBody + "\n" +
                    exception.Message
                );

                continue;
            }

            if (response == null)
            {
                Debug.LogWarning(
                    "A status API üres választ adott."
                );

                continue;
            }

            if (!response.success)
            {
                Debug.LogWarning(
                    "A status API hibát adott: " +
                    response.message
                );

                continue;
            }

            if (response.data == null)
            {
                Debug.LogWarning(
                    "A status API válaszából hiányzik a data mezõ."
                );

                continue;
            }

            if (response.data.status == "pending")
            {
                Debug.Log(
                    "Az itch.io bejelentkezés még folyamatban van..."
                );

                continue;
            }

            if (response.data.status == "completed")
            {
                HandleSuccessfulLogin(
                    state,
                    response.data
                );

                yield break;
            }

            Debug.LogWarning(
                "Ismeretlen login státusz: " +
                response.data.status
            );
        }

        Debug.LogError(
            "Az itch.io bejelentkezés idõtúllépés miatt megszakadt."
        );

        FinishLoginAttempt();
    }

    private void HandleSuccessfulLogin(
        string state,
        ItchLoginStatusData loginData
    )
    {
        string expectedState = PlayerPrefs.GetString(
            "itch_oauth_state",
            string.Empty
        );

        if (string.IsNullOrWhiteSpace(expectedState) ||
            expectedState != state)
        {
            Debug.LogError(
                "Az OAuth state nem egyezik."
            );

            FinishLoginAttempt();
            return;
        }

        if (loginData.id <= 0)
        {
            Debug.LogError(
                "Érvénytelen itch.io user ID érkezett."
            );

            FinishLoginAttempt();
            return;
        }

        if (string.IsNullOrWhiteSpace(loginData.username))
        {
            Debug.LogError(
                "Nem érkezett itch.io felhasználónév."
            );

            FinishLoginAttempt();
            return;
        }

        itchUserId = loginData.id;
        itchUsername = loginData.username;
        itchDisplayName = loginData.display_name;
        isLoggedIn = true;

        PlayerPrefs.DeleteKey(
            "itch_oauth_state"
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Sikeres itch.io bejelentkezés!\n" +
            "ID: " + itchUserId + "\n" +
            "Username: " + itchUsername + "\n" +
            "Display name: " + itchDisplayName
        );

        FinishLoginAttempt();

        OnItchLoginSuccessful();
    }

    private void OnItchLoginSuccessful()
    {
        string playerName =
            string.IsNullOrWhiteSpace(itchDisplayName)
                ? itchUsername
                : itchDisplayName;

        Debug.Log(
            "Leaderboard játékosnév: " + playerName
        );

        /*
         * Itt tudod frissíteni a UI-t vagy meghívni
         * a leaderboard rendszeredet.
         *
         * Például:
         *
         * loginStatusText.text =
         *     "Belépve: " + playerName;
         */
    }

    private void FinishLoginAttempt()
    {
        isLoginInProgress = false;
        pollingCoroutine = null;
    }

    private void ResetLoginData()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }

        itchUserId = 0;
        itchUsername = string.Empty;
        itchDisplayName = string.Empty;
        isLoggedIn = false;
        isLoginInProgress = false;
    }

    private void OnDestroy()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }
}

[Serializable]
public class ItchLoginStatusResponse
{
    public bool success;
    public string message;
    public ItchLoginStatusData data;
}

[Serializable]
public class ItchLoginStatusData
{
    public string status;
    public long id;
    public string username;
    public string display_name;
}