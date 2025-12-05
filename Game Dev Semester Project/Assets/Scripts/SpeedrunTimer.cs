using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float runTime = 0f;
    private bool isRunning = true;

    void Start()
    {
        runTime = 0f;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) 
            return;

        runTime += Time.deltaTime;

        UpdateTimerUI();
    }

    public void ResetTimer()
    {
        runTime = 0f;
        UpdateTimerUI();
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(runTime / 60f);
        int seconds = Mathf.FloorToInt(runTime % 60f);
        int milliseconds = Mathf.FloorToInt((runTime * 1000f) % 1000f);

        timerText.text = 
            $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
