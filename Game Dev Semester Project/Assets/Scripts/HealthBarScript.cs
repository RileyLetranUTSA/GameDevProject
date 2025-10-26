using UnityEngine;
using UnityEngine.UI;
public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Image fillBar;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(float health)
    {
        slider.value = health;

        float healthPercent = health / slider.maxValue;

        if (healthPercent <= 0.25)
        {
            fillBar.color = Color.red;
        }
        else
        {
            fillBar.color = Color.white;
        }
    }
}
