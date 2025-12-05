using System.Collections;
using UnityEngine;

public class Boss3 : BossBase
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
                        //*****NOT FINSIHED*****
                        CharmPhase2Attack();
                        break;
                    case 3:
                        //*****NOT FINSIHED*****
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
                controller.shootInterval = 0.5f;
                controller.projectileSpeed = 7f;
                break;
            case 2:
                controller.shootInterval = .5f;
                controller.projectileSpeed = 10f;
                break;
            case 3:
                controller.shootInterval = .6f;
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
                controller.shootInterval = 0.7f;
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
        rotationOffset = (rotationOffset + 40f) % 360f;
        FireCircleWithGaps(30, 7, projectile1Prefab, rotationOffset);
    }

    //Phase 2
    void Phase2Attack()
    {
        int projectileCount = 16;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCurvedCircle(projectileCount, projectile1Prefab, rotationOffset);
    }

    //Phase 3

    void Phase3Attack()
    {
        int projectileCount = 32;
        rotationOffset = (rotationOffset + 10f) % 360f;
        FireCurvedCircle(projectileCount, projectile1Prefab, rotationOffset, 10f);
        ReverseCurvedCircle(projectileCount, projectile1Prefab, rotationOffset, 10f);
    }
    //End Base Phase Code

    //Charm Phase Code
    void CharmPhase1Attack()
    {
        int projectileCount = 20;
        FireProjectilesAroundPlayer(projectileCount, 18);
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

            Destroy(proj, 10f);
        }
    }
    void FireCurvedCircle(int projectileCount, GameObject prefab, float angleOffset = 0f, float curveStrength = 30f)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i + angleOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);

            StartCoroutine(MoveCurvedProjectile(proj, dir, controller.projectileSpeed, curveStrength));

            Destroy(proj, 12f);
        }
    }
    IEnumerator MoveCurvedProjectile(GameObject proj, Vector3 direction, float speed, float curveStrength)
    {
        float angle = 0f;

        while (proj != null)
        {
            angle += curveStrength * Time.deltaTime;
            Vector3 curvedDir = Quaternion.Euler(0, angle, 0) * direction;

            proj.transform.position += curvedDir * speed * Time.deltaTime;

            yield return null;
        }
    }

    void ReverseCurvedCircle(int projectileCount, GameObject prefab, float angleOffset = 0f, float curveStrength = 50f)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i - angleOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);

            StartCoroutine(ReversedMoveCurvedProjectile(proj, dir, controller.projectileSpeed, curveStrength));

            Destroy(proj, 10f);
        }
    }
    IEnumerator ReversedMoveCurvedProjectile(GameObject proj, Vector3 direction, float speed, float curveStrength)
    {
        float angle = 0f;

        while (proj != null)
        {
            angle -= curveStrength * Time.deltaTime;
            Vector3 curvedDir = Quaternion.Euler(0, angle, 0) * direction;

            proj.transform.position += curvedDir * speed * Time.deltaTime;

            yield return null;
        }
    }
    void FireProjectilesAroundPlayer(int projectileCount, float extraRadius)
    {
        float totalRadius = basePlayerRadius + extraRadius;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float radians = angle * Mathf.Deg2Rad;

            Vector3 spawnCenter = controller.playerSpawnRef.position;
            Vector3 spawnPos = spawnCenter + new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * totalRadius;

            Vector3 dir = (controller.playerSpawnRef.position - spawnPos).normalized;

            GameObject proj = Instantiate(projectile1Prefab, spawnPos, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * controller.projectileSpeed;

            Destroy(proj, 25f);
        }
    }

    void FireCircleWithGaps(int rows, int bulletsPerRow, GameObject prefab, float angleOffset)
    {
        float sliceAngle = 360f / rows;

        for (int i = 0; i < rows; i++)
        {
            bool isGap = (i % 2 == 0);

            if (isGap)
                continue;

            float baseAngle = sliceAngle * i + angleOffset;

            for (int b = 0; b < bulletsPerRow; b++)
            {
                float angle = baseAngle + (sliceAngle / bulletsPerRow) * b;

                Vector3 dir = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);

                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.linearVelocity = dir * controller.projectileSpeed;

                Destroy(proj, 5f);
            }
        }
    }

}
