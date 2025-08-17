using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMain : MonoBehaviour
{
    public void LoadNewGame()
    {
        // Load the main scene for a new game
        SceneManager.LoadScene("MenuScene");
    }
}
