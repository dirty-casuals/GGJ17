using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private GameObject pauseMenu;

    public void ShowPauseMenu()
    {
        PauseGame();
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        UnPauseGame();
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void LoadMainMenu()
    {
        UnPauseGame();
        SceneLoadManagement.Instance.LoadNewSceneImmediate(SceneNames.MainMenuScene);
    }

    public void RestartGame()
    {
        UnPauseGame();
        SceneLoadManagement.Instance.LoadNewSceneImmediate(SceneNames.LevelOneScene);
    }

    private void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    private void UnPauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
