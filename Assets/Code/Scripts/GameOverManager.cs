using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    public void TriggerGameOver()
    {
        Debug.Log("Game Over triggered!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("GameOverPanel is not assigned!");
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}