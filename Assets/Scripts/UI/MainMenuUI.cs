using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoadManagement.Instance.LoadNewScene(SceneNames.LevelOneScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
