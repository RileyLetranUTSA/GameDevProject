using System;
using System.Collections;
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
    public Transform playerSpawnRef;

    [Header("Projectile Settings")]
    public float projectileSpeed = 10f;
    public float shootInterval = 2f;
    public float nextShootTime;

    [Header("Logic Reference")]
    public BossBase bossLogic;

    [Header("Boss Spawn Settings")]
    public GameObject[] bossPrefabs;
    public int currentBossIndex = 0;
    public float nextBossDelay = 5f;
    public Transform spawnPoint;
    private bool isDead = false;

    [Header("Invulnerability Settings")]
    public float invulnDuration = 1.5f;
    private bool isInvulnerable = false;

    public static BossController ActiveBoss { get; set; }



    void Start()
    {
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);

        if (bossLogic != null)
        {
            bossLogic.Init(this);
        }
        ActiveBoss = this;
        bossLogic.EnterBasePhase(currentPhase);
    }

    void Update()
    {
        if (!isDead)
        {
            HandlePhases();
            bossLogic.CheckToFire(ref nextShootTime);
        }

    }

    public void HandlePhases()
    {
        float hpPercent = currentHP / maxHP;

        if (hpPercent <= 0.25f && hasPhaseChanged == false && currentCharmPhase < maxPhases)
        {
            currentCharmPhase++;
            DestroyAllProjectiles();
            bossLogic.EnterCharmPhase(currentCharmPhase);

            StartCoroutine(InvulnerabilityCoroutine());
        }
        else if (hpPercent <= 0f && hasPhaseChanged == true && currentPhase < maxPhases)
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
        if (isDead || isInvulnerable) return;

        currentHP -= dmg;
        currentHP = Mathf.Max(currentHP, 0);
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
        Debug.Log($"Boss {currentBossIndex + 1} defeated!");

        isDead = true;

        transform.GetChild(0).gameObject.SetActive(false);
        healthBar.SetHealth(0);

        DestroyAllProjectiles();
        bossLogic.enabled = false;

        StartCoroutine(DestroyAndSpawn());
    }


    void DestroyAllProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in projectiles)
        {
            Destroy(proj);
        }
    }

    IEnumerator DestroyAndSpawn()
    {
        if (currentBossIndex + 1 < bossPrefabs.Length)
        {
            yield return new WaitForSeconds(nextBossDelay);

            int nextIndex = currentBossIndex + 1;

            GameObject nextBoss = Instantiate(
                bossPrefabs[nextIndex],
                spawnPoint.position,
                Quaternion.identity
            );

            BossController nextCtrl = nextBoss.GetComponent<BossController>();
            nextCtrl.currentBossIndex = nextIndex;
            nextCtrl.bossPrefabs = bossPrefabs;
            nextCtrl.spawnPoint = spawnPoint;
            nextCtrl.healthBar = healthBar;
            nextCtrl.playerSpawnRef = playerSpawnRef;

            nextCtrl.bossLogic = nextBoss.GetComponent<BossBase>();

            nextCtrl.currentHP = nextCtrl.maxHP;
            nextCtrl.healthBar.SetMaxHealth(nextCtrl.maxHP);
            nextCtrl.healthBar.SetHealth(nextCtrl.currentHP);

            BossController.ActiveBoss = nextCtrl;
        }

        Destroy(gameObject);
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnDuration);
        isInvulnerable = false;
    }


}
