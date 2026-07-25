using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float showTime;
    [SerializeField] private float animationTime = 1f;
    private float startTime;
    private float boxHeight;
    private RectTransform rt;
    private static bool shortenedShowtime = false;
    [SerializeField] private bool useShortenedShowTIme = true;

    void OnEnable()
    {
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        if (shortenedShowtime && useShortenedShowTIme) showTime = 7f;

        text.gameObject.SetActive(false);
        startTime = Time.time;
        rt = GetComponent<RectTransform>();
        boxHeight = rt.rect.height;

        while (Time.time - startTime < animationTime)
        {
            float t = (Time.time - startTime) / animationTime;
            t = Mathf.Clamp01(t);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, t * boxHeight);
            yield return null;
        }
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boxHeight);
        text.gameObject.SetActive(true);

        while (Time.time - startTime < showTime - animationTime)
        {
            yield return null;
        }
        text.gameObject.SetActive(false);

        while ((Time.time - startTime) > (showTime - animationTime))
        {
            float t = (showTime - (Time.time - startTime)) / animationTime;
            t = Mathf.Clamp01(t);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, t * boxHeight);
            if (Time.time - showTime >= animationTime) break;
            yield return null;
        }
        shortenedShowtime = true;
        gameObject.SetActive(false); //disable this display
    }

}
