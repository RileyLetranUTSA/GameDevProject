
using UnityEngine.UI;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    public int startLives = 3;
    private int currentLives;
    private bool isInvincible = false;

    public Renderer playerRenderer;
    public float flashDura = 0.1f;
    public int flashCount = 6;
    public int scoreInterval = 100000;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip LifeGainSound;

    public GameObject livesContainer;
    public UnityEngine.UI.Image lifeIconPrefab;
    public GameOverMenuScript GameOverScreen;

    public ThirdPersonMovementScript playerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLives = startLives;
        UpdateLivesUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit Something");

        if (other.CompareTag("Projectile") && !isInvincible)
        {
            Destroy(other.gameObject);
            TakeDamage();
            UpdateLivesUI();
        }
    }

    void TakeDamage()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        currentLives--;

        if (playerScript != null)
        {
            playerScript.currentCharms = playerScript.maxCharms;
            playerScript.UpdateCharmUI();
        }

        if (currentLives < 0)
        {
            Debug.Log("Player Lost All Lives");
            GameOverScreen.ScoreSetUp(playerScript.score);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(FlashInvincibility());
        }
    }

    System.Collections.IEnumerator FlashInvincibility()
    {
        isInvincible = true;

        Material mat = playerRenderer.material;

        for (int i = 0; i < flashCount; i++)
        {
            mat.SetFloat("_FlashAmount", 1f);
            yield return new WaitForSeconds(flashDura);

            mat.SetFloat("_FlashAmount", 0f);
            yield return new WaitForSeconds(flashDura);
        }

        isInvincible = false;
    }

    void UpdateLivesUI()
    {
        foreach (Transform child in livesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentLives; i++)
        {
            UnityEngine.UI.Image lifeIcon = Instantiate(lifeIconPrefab, livesContainer.transform);
            lifeIcon.gameObject.SetActive(true);
        }
    }

    public void AddLifeCheck(int currentScore)
    {
        while (currentScore >= scoreInterval)
        {
            AddLife();
            scoreInterval *= 3;
        }
    }

    public void AddLife()
    {
        if (audioSource != null && LifeGainSound != null)
        {
            audioSource.PlayOneShot(LifeGainSound);
        }

        currentLives++;
        UpdateLivesUI();
    }
}
