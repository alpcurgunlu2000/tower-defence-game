using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
      public void SaveAndCloseOptions()
    {
        Debug.Log("Settings saved!");
        optionsPanel.SetActive(false);
    }
}
