using UnityEngine;

namespace Assets.Scripts.UIScripts
{
    public class MenuNavigation : MonoBehaviour
    {
        public void ExitGame()
        {
            Application.Quit();
        }

        public void NewGame()
        {
            SceneLoader.Load(SceneLoader.Scene.MainScene);
        }

        public void ResumeGame(GameObject pauseMenu)
        {
            pauseMenu.SetActive(false);
            GameState.GamePaused = false;
        }

        public void QuitToMainMenu()
        {
            GameState.GamePaused = false;
            SceneLoader.Load(SceneLoader.Scene.MainMenu);
        }
    }
}
