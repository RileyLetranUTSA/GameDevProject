using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle vsyncToggle;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject HowtoMenu;

    private void Start()
    {
        SetupResolutions();
        SetupVSync();
        SetupFullscreen();
    }
    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetupVSync()
    {
        vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
        vsyncToggle.onValueChanged.AddListener(SetVSync);
    }

    void SetupFullscreen()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    public void PlayButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Boss1");
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("QUITTING GAME...");
    }

    public void OpenOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void OpenHowtoMenu()
    {
        mainMenu.SetActive(false);
        HowtoMenu.SetActive(true);
    }

    public void CloseHowtoMenu()
    {
        HowtoMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    public void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }
}
