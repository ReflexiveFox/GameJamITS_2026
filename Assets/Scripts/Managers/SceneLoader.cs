using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadMainMenu()
        {
                SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
                Application.Quit();
        }
    }
}