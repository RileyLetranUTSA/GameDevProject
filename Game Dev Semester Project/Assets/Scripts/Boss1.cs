using System.Collections;
using UnityEngine;

public class Boss1 : BossBase
{
    public GameObject projectile1Prefab;
    public Transform projectileSpawnCenter;
    private float rotationOffset = 0f;

    public float basePlayerRadius = 2f;

    public override void CheckToFire(ref float nextShootTime)
    {
        if (Time.time >= controller.nextShootTime)
        {
            if (controller.hasPhaseChanged == false)
            {
                switch (controller.currentPhase)
                {
                    case 1:
                        Phase1Attack();
                        break;
                    case 2:
                        Phase2Attack();
                        break;
                    case 3:
                        Phase3Attack();
                        break;
                }
            }
            else if (controller.hasPhaseChanged == true)
                switch (controller.currentCharmPhase)
                {
                    case 1:
                        CharmPhase1Attack();
                        break;
                    case 2:
                        CharmPhase2Attack();
                        break;
                    case 3:
                        CharmPhase3Attack();
                        break;

                }
            nextShootTime = Time.time + controller.shootInterval;
        }
    }


    public override void EnterBasePhase(int phaseNumber)
    {
        //controller.currentPhase = phaseNumber;
        Debug.Log($"Boss entered Phase {phaseNumber}");

        switch (phaseNumber)
        {
            case 1:
                controller.shootInterval = 2f;
                controller.projectileSpeed = 8f;
                break;
            case 2:
                controller.shootInterval = 2f;
                controller.projectileSpeed = 10f;
                break;
            case 3:
                //*****NOT FINSIHED*****
                controller.shootInterval = .1f;
                controller.projectileSpeed = 8f;
                break;
        }
    }
    public override void EnterCharmPhase(int phaseNumber)
    {
        controller.hasPhaseChanged = true;
        Debug.Log($"Boss entered Charm Phase {phaseNumber}");

        switch (phaseNumber)
        {
            case 1:
                controller.shootInterval = 3f;
                controller.projectileSpeed = 8f;
                break;
            case 2:
                //*****NOT FINSIHED*****
                controller.shootInterval = .1f;
                controller.projectileSpeed = 8f;
                break;
            case 3:
                //*****NOT FINSIHED*****
                controller.shootInterval = .1f;
                controller.projectileSpeed = 8f;
                break;
        }
    }

    //Base Phase Code
    //Phase 1
    void Phase1Attack()
    {
        int projectileCount = 32;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount, projectile1Prefab, rotationOffset);
    }

    //Phase 2
    void Phase2Attack()
    {
        int projectileCount = 10;
        DelayedTargetedRingAttack(projectileCount, 5f, 12f, 1.5f);
    }

    //Phase 3

    void Phase3Attack()
    {
        //*****NOT FINSIHED*****
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount, projectile1Prefab, rotationOffset);
    }
    //End Base Phase Code

    //Charm Phase Code
    void CharmPhase1Attack()
    {
        FireSequentialLineShapes(5, 4, 5, 0.5f);
    }
    void CharmPhase2Attack()
    {
        //*****NOT FINSIHED*****
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount, projectile1Prefab, rotationOffset);

    }
    void CharmPhase3Attack()
    {
        //*****NOT FINSIHED*****
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount, projectile1Prefab, rotationOffset);
    }

    //End Charm Phase Code

    //Shared firing logic
    void FireCircle(int projectileCount, GameObject prefab, float angleOffset = 0f)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i + angleOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * controller.projectileSpeed;

            Destroy(proj, 7f);
        }
    }

    public void FireSequentialLineShapes(int row1Count, int row2Count, int row3Count, float delayBetweenRows = 0.1f)
    {
        Vector3 initialPos = (controller.playerSpawnRef.position - projectileSpawnCenter.position).normalized;
        StartCoroutine(FireSequentialLineShapesRoutine(row1Count, row2Count, row3Count, delayBetweenRows, initialPos));
    }

    IEnumerator FireSequentialLineShapesRoutine(int row1Count, int row2Count, int row3Count, float delay, Vector3 initialPos)
    {
        FireLineRow(row1Count, initialPos);
        yield return new WaitForSeconds(delay);

        FireLineRow(row2Count, initialPos);
        yield return new WaitForSeconds(delay);

        FireLineRow(row3Count, initialPos);
    }

    void FireLineRow(int projectileCount, Vector3 initialPos)
    {
        Vector3 perpendicular = Vector3.Cross(initialPos, Vector3.up).normalized;

        float spacing = 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float offset = (i - (projectileCount - 1) / 2f) * spacing;

            Vector3 spawnPos = projectileSpawnCenter.position + perpendicular * offset;

            GameObject proj = Instantiate(projectile1Prefab, spawnPos, Quaternion.identity);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = initialPos * controller.projectileSpeed;

            Destroy(proj, 7f);
        }
    }

    void DelayedTargetedRingAttack(int projectileCount, float outwardSpeed, float travelSpeed, float delay)
    {
        StartCoroutine(DelayedTargetedRingRoutine(projectileCount, outwardSpeed, travelSpeed, delay));
    }

    IEnumerator DelayedTargetedRingRoutine(int projectileCount, float outwardSpeed, float travelSpeed, float delay)
    {
        GameObject[] projectiles = new GameObject[projectileCount];
        Vector3 bossPos = projectileSpawnCenter.position;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));

            GameObject proj = Instantiate(projectile1Prefab, bossPos, Quaternion.identity);
            projectiles[i] = proj;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * outwardSpeed;
        }

        yield return new WaitForSeconds(delay);

        Vector3 currentPlayerPos = controller.playerSpawnRef.position;

        foreach (GameObject proj in projectiles)
        {
            if (proj == null) continue;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 dir = (currentPlayerPos - proj.transform.position).normalized;
            rb.linearVelocity = dir * travelSpeed;
        }

        foreach (GameObject proj in projectiles)
            if (proj != null)
                Destroy(proj, 8f);
    }


}
