using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameOverMenuScript : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public void ScoreSetUp(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = "Score: " + score.ToString();
    }

    public void AgainButton()
    {
        SceneManager.LoadScene("Boss1");
        Time.timeScale = 1f;
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1f;
    }
}
