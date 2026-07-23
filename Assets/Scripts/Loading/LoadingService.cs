using UnityEngine;

public class LoadingService : MonoBehaviour
{
    public static LoadingService Instance { get; private set; }

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingContainer;

    private int activeLoadingRequests;

    public bool IsLoading => activeLoadingRequests > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeLoadingRequests = 0;

        if (loadingContainer != null)
        {
            loadingContainer.SetActive(false);
        }
    }

    public void Show()
    {
        activeLoadingRequests++;
        UpdateLoadingVisibility();
    }

    public void Hide()
    {
        activeLoadingRequests--;

        if (activeLoadingRequests < 0)
        {
            activeLoadingRequests = 0;
        }

        UpdateLoadingVisibility();
    }

    public void ForceHide()
    {
        activeLoadingRequests = 0;
        UpdateLoadingVisibility();
    }

    private void UpdateLoadingVisibility()
    {
        if (loadingContainer == null)
        {
            Debug.LogWarning(
                "A LoadingService loadingContainer mezõje nincs beállítva."
            );

            return;
        }

        loadingContainer.SetActive(IsLoading);
    }
}