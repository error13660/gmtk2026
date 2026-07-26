using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData Instance { get; private set; }

    [Header("Bejelentkezett felhasználó")]

    [SerializeField]
    private long userId;

    [SerializeField]
    private string userName = string.Empty;

    [SerializeField]
    private bool isLoggedIn;

    public long UserId => userId;
    public string UserName => userName;
    public bool IsLoggedIn => isLoggedIn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        RefreshLoginState();
    }

    public void SetUser(
        long newUserId,
        string newUserName
    )
    {
        userId = newUserId;
        userName = newUserName;

        RefreshLoginState();
    }

    public void ClearUser()
    {
        userId = 0;
        userName = string.Empty;

        RefreshLoginState();
    }

    private void RefreshLoginState()
    {
        isLoggedIn =
            userId > 0 &&
            !string.IsNullOrWhiteSpace(userName);
    }
}