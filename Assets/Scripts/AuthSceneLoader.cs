using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthSceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
