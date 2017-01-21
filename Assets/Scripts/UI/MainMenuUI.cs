using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoadManagement.Instance.LoadNewScene(SceneNames.GameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
