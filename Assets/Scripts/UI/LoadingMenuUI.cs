using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private Image loadingImage;
    private AsyncOperation sceneLoading;

    private void Start()
    {
        LoadDeferredScene();
    }

    public void LoadSceneWhenReady()
    {
        if(sceneLoading == null) return;

        sceneLoading.allowSceneActivation = true;
    }

    private void LoadDeferredScene()
    {
        string scene = SceneLoadManagement.Instance.GetDeferredSceneToLoad();
        if(string.IsNullOrEmpty(scene)) return;

        sceneLoading = SceneManager.LoadSceneAsync(scene);
        sceneLoading.allowSceneActivation = false;
        StartCoroutine(WaitForSceneToLoad());
    }

    private IEnumerator WaitForSceneToLoad()
    {
        readyButton.SetActive(false);
        while(sceneLoading.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return StartCoroutine(FillLoadingBar());
        readyButton.SetActive(true);
    }

    private IEnumerator FillLoadingBar()
    {
        float fill = 0.0f;
        float fillIncrement = 0.009f;

        while(fill < 1.0f)
        {
            SetLoadingBar(fill);
            fill += fillIncrement;
            yield return new WaitForSeconds(fillIncrement);
        }
        SetLoadingBar(1.0f);
    }

    private void SetLoadingBar(float fill)
    {
        loadingImage.fillAmount = fill;
    }
}
