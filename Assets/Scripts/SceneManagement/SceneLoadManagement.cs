using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManagement : MonoBehaviour
{
    private const string gameobjectName = "SceneManager";
    private static SceneLoadManagement _instance;
    private string deferredSceneName;

    private void Awake()
    {
        if(_instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static SceneLoadManagement Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameObject(gameobjectName).AddComponent<SceneLoadManagement>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    // LoadNewScene loads the loading scene and preps
    // the specified deferred scene for loading
    public void LoadNewScene(string sceneName)
    {
        deferredSceneName = sceneName;
        SceneManager.LoadScene(SceneNames.LoadingScene);
    }

    public string GetDeferredSceneToLoad()
    {
        return deferredSceneName;
    }
}
