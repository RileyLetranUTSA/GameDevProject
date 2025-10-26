using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 100;
    private float currentHP;

    public HealthBarScript healthBar;

    [Header("Phases")]
    public int currentPhase = 1;
    public int currentCharmPhase = 0;
    public int maxPhases = 3;
    public int bossNum = 1;
    public Boolean hasPhaseChanged = false;

    [Header("Projectile Settings")]
    public float projectileSpeed = 10f;
    public float shootInterval = 2f;
    public float nextShootTime;

    [Header("Logic Reference")]
    public BossBase bossLogic;


    void Start()
    {
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);

        if (bossLogic != null)
        {
            bossLogic.Init(this);
        }
        bossLogic.EnterBasePhase(currentPhase);
    }

    void Update()
    {
        HandlePhases();
        bossLogic.CheckToFire(ref nextShootTime);

    }

    public void HandlePhases()
    {
        float hpPercent = currentHP / maxHP;

        if (hpPercent <= 0.25f && hasPhaseChanged == false)
        {
            currentCharmPhase++;
            DestroyAllProjectiles();
            bossLogic.EnterCharmPhase(currentCharmPhase);
        }
        else if (hpPercent <= 0f && hasPhaseChanged == true)
        {
            currentPhase++;
            DestroyAllProjectiles();
            bossLogic.EnterBasePhase(currentPhase);
            currentHP = maxHP;
            healthBar.SetHealth(currentHP);
            hasPhaseChanged = false;
        }
    }



    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        healthBar.SetHealth(currentHP);

        if (currentHP <= 0)
        {
            if (currentPhase >= 3 && currentCharmPhase >= 3)
            {
                currentHP = 0;
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
    void DestroyAllProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in projectiles)
        {
            Destroy(proj);
        }
    }
}
