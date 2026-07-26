using UnityEngine;

public class LeaderboardLoginLayout : MonoBehaviour
{
    [Header("UI elemek")]

    [SerializeField]
    private GameObject loginButton;

    [SerializeField]
    private RectTransform scrollView;

    [Header("Scroll View alsó margó")]

    [Tooltip("Ennyi hely marad a login gombnak kijelentkezve.")]
    [SerializeField]
    private float loggedOutBottomOffset = 40f;

    [Tooltip("A Scroll View alsó margója bejelentkezve.")]
    [SerializeField]
    private float loggedInBottomOffset = 0f;

    private bool previousLoginState;

    private void Start()
    {
        RefreshLayout();

        previousLoginState = IsUserLoggedIn();
    }

    private void Update()
    {
        bool currentLoginState = IsUserLoggedIn();

        if (currentLoginState == previousLoginState)
        {
            return;
        }

        previousLoginState = currentLoginState;

        RefreshLayout();
    }

    public void RefreshLayout()
    {
        bool isLoggedIn = IsUserLoggedIn();

        if (loginButton != null)
        {
            loginButton.SetActive(!isLoggedIn);
        }

        if (scrollView != null)
        {
            SetScrollViewBottomOffset(
                isLoggedIn
                    ? loggedInBottomOffset
                    : loggedOutBottomOffset
            );
        }

        Canvas.ForceUpdateCanvases();
    }

    private bool IsUserLoggedIn()
    {
        return UserData.Instance != null &&
               UserData.Instance.IsLoggedIn;
    }

    private void SetScrollViewBottomOffset(
        float bottomOffset
    )
    {
        Vector2 offsetMin = scrollView.offsetMin;

        offsetMin.y = bottomOffset;

        scrollView.offsetMin = offsetMin;
    }
}