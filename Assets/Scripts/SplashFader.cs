using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashFader : MonoBehaviour
{
    public Image logoImage;
    public float fadeDuration = 2f;
    public float waitTime = 2f;
    public string nextSceneName = "LoadingScene";

    void Start()
    {
        StartCoroutine(PlaySplash());
    }

    IEnumerator PlaySplash()
    {
    
        Color c = logoImage.color;
        c.a = 0f;
        logoImage.color = c;

        // Fade-In
        yield return StartCoroutine(FadeToAlpha(1f));

        // Warten
        yield return new WaitForSeconds(waitTime);

        // Fade-Out
        yield return StartCoroutine(FadeToAlpha(0f));

        // Szene laden
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        float t = 0f;
        Color startColor = logoImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            logoImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
    }
}
