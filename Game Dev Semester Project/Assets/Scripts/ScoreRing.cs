using UnityEngine;
using System.Collections.Generic;

public class ScoreRing : MonoBehaviour
{
    public ThirdPersonMovementScript playerScript;
    public PlayerLives liveScript;
    public AudioSource audioSource;
    public AudioClip grazeSound;
    public int scorePerFrame = 10;
    private HashSet<GameObject> trackedProjectiles = new HashSet<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            trackedProjectiles.Add(other.gameObject);

            audioSource.PlayOneShot(grazeSound, 0.09f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            trackedProjectiles.Remove(other.gameObject);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        foreach (GameObject proj in trackedProjectiles)
        {
            if (proj != null)
            {
                playerScript.AddScore(scorePerFrame);
                playerScript.UpdateScoreUI();
                liveScript.AddLifeCheck(playerScript.score);
            }
        }
    }
}
