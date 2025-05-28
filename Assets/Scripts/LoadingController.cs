using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public class LoadingController : MonoBehaviour
{
    public Slider loadingSlider;
    public string sceneToLoad = "AuthScene";
    public TextMeshProUGUI loadingText; 
    public float textUpdateSpeed = 0.5f;


    void Start()
    {
    
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(AnimateLoadingText());

    }

    IEnumerator LoadSceneAsync()
    {
    AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneToLoad);
    asyncOp.allowSceneActivation = false;

    float fakeProgress = 0f;

    while (fakeProgress < 1f)
    {
        // Echte Ladezeit bis 0.9
        float target = asyncOp.progress < 0.9f ? asyncOp.progress / 0.9f : 1f;

        // Glatt nach oben bewegen
        fakeProgress = Mathf.MoveTowards(fakeProgress, target, Time.deltaTime);
        loadingSlider.value = fakeProgress;

        // Wenn bei 1.0: kurz warten, dann aktivieren
        if (fakeProgress >= 1f)
        {
            yield return new WaitForSeconds(0.5f); // Optional für dramatischen Effekt
            asyncOp.allowSceneActivation = true;
        }

        yield return null;
        }
    }

    IEnumerator AnimateLoadingText()
    {
    string baseText = "Loading";
    int dotCount = 0;
    

    while (true)
        {
            dotCount = (dotCount + 1) % 4; // 0,1,2,3 → wieder 0
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(textUpdateSpeed);
        }
    }
}
