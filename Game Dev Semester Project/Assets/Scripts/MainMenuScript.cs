using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Boss1");
        Time.timeScale = 1f;
    }
}
