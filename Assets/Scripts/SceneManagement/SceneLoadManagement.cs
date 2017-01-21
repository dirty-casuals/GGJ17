using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManagement : MonoBehaviour
{
    private const string gameobjectName = "SceneManager";
    private static SceneLoadManagement _instance;
    private string _deferredSceneName;
    private bool _loadImmediate;

    private void Awake()
    {
        if(_instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool LoadImmediate { get { return _loadImmediate; } }
    public string DeferredSceneName { get { return _deferredSceneName; } }

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
        _deferredSceneName = sceneName;
        SceneManager.LoadScene(SceneNames.LoadingScene);
    }

    // LoadNewSceneImmediate loads the loading scene
    // and loads new scene immediately
    public void LoadNewSceneImmediate(string sceneName)
    {
        _loadImmediate = true;
        _deferredSceneName = sceneName;
        SceneManager.LoadScene(SceneNames.LoadingScene);
    }

    public void ResetLoadingState()
    {
        _loadImmediate = false;
        _deferredSceneName = "";
    }
}
