using System.Collections;
using UnityEngine;

public class Boss2 : BossBase
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
                controller.shootInterval = 1.5f;
                controller.projectileSpeed = 10f;
                break;
            case 2:
                controller.shootInterval = 1f;
                controller.projectileSpeed = 15f;
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

    //Base Phase Code
    //Phase 1
    void Phase1Attack()
    {
        int projectileCount = 16;
        FireExplodingProjectileAtPlayer(projectile1Prefab, projectileCount, 120f, 0.6f);
    }

    //Phase 2
    void Phase2Attack()
    {
        int projectileCount = 15;
        FireExpandingRingTowardPlayer(projectileCount, projectile1Prefab, 8f, 12f, 5f);
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
        rotationOffset = (rotationOffset + 20f) % 360f;
        ExplodingRingAttack(20, 6, 8f, 2f);
    }
    void CharmPhase2Attack()
    {
        int projectileCount = 21;
        RingHomingAttack(projectileCount, 7f, 10f, 3.5f);
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
    public void FireExpandingRingTowardPlayer(int projectileCount, GameObject prefab, float expandSpeed, float travelSpeed, float expandRadius)
    {
        StartCoroutine(ExpandingRingRoutine(
            projectileCount, prefab, expandSpeed, travelSpeed, expandRadius));
    }

    IEnumerator ExpandingRingRoutine(int projectileCount, GameObject prefab, float expandSpeed, float travelSpeed, float expandRadius)
    {
        GameObject[] projectiles = new GameObject[projectileCount];
        Vector3 bossPos = projectileSpawnCenter.position;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(prefab, bossPos, Quaternion.identity);
            projectiles[i] = proj;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb)
                rb.linearVelocity = dir * expandSpeed;
        }

        float timer = 0;
        while (timer < 3f)
        {
            bool expanded = true;

            for (int i = 0; i < projectileCount; i++)
            {
                if (projectiles[i] == null) continue;
                float dist = Vector3.Distance(projectiles[i].transform.position, bossPos);
                if (dist < expandRadius)
                {
                    expanded = false;
                    break;
                }
            }

            if (expanded) break;

            timer += Time.deltaTime;
            yield return null;
        }

        Vector3 lockedDirection = (
            controller.playerSpawnRef.position - bossPos
        ).normalized;

        foreach (var proj in projectiles)
        {
            if (proj == null) continue;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb)
                rb.linearVelocity = lockedDirection * travelSpeed;
        }

        foreach (var proj in projectiles)
            if (proj != null)
                Destroy(proj, 10f);
    }

    void ExplodingRingAttack(int initialCount, int explosionCount, float travelSpeed, float explodeDelay)
    {
        StartCoroutine(ExplodingRingRoutine(initialCount, explosionCount, travelSpeed, explodeDelay));
    }

    IEnumerator ExplodingRingRoutine(int initialCount, int explosionCount, float travelSpeed, float explodeDelay)
    {
        GameObject[] initialProjectiles = new GameObject[initialCount];

        Vector3 bossPos = projectileSpawnCenter.position;

        for (int i = 0; i < initialCount; i++)
        {
            float angle = (360f / initialCount) * i + rotationOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(projectile1Prefab, bossPos, Quaternion.identity);
            initialProjectiles[i] = proj;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * travelSpeed;
        }

        yield return new WaitForSeconds(explodeDelay);

        for (int i = 0; i < initialProjectiles.Length; i++)
        {
            GameObject proj = initialProjectiles[i];

            if (proj == null)
                continue;

            Vector3 explodePos = proj.transform.position;

            Destroy(proj);

            SpawnExplosionRing(explodePos, explosionCount, controller.projectileSpeed);
        }
    }

    void SpawnExplosionRing(Vector3 center, int projectileCount, float speed)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i + rotationOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(projectile1Prefab, center, Quaternion.identity);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * speed;

            Destroy(proj, 6f);
        }
    }

    void RingHomingAttack(int projectileCount, float ringRadius, float travelSpeed, float delayBeforeRelease)
    {
        StartCoroutine(RingHomingAttackRoutine(projectileCount, ringRadius, travelSpeed, delayBeforeRelease));
    }

    IEnumerator RingHomingAttackRoutine(int projectileCount, float ringRadius, float travelSpeed, float delayBeforeRelease)
    {
        GameObject[] ring = new GameObject[projectileCount];

        Vector3 bossPos = projectileSpawnCenter.position;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 startPos = bossPos;
            Vector3 targetOffset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * ringRadius;

            GameObject proj = Instantiate(projectile1Prefab, startPos, Quaternion.identity);
            ring[i] = proj;

            StartCoroutine(MoveProjectileToPosition(proj, startPos + targetOffset, 0.25f));
        }

        yield return new WaitForSeconds(0.3f);

        Transform player = controller.playerSpawnRef;
        Vector3 ringMoveDir = (player.position - bossPos).normalized;

        float timer = 0f;

        while (timer < delayBeforeRelease)
        {
            foreach (GameObject proj in ring)
            {
                if (proj != null)
                    proj.transform.position += ringMoveDir * travelSpeed * Time.deltaTime;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject proj in ring)
        {
            if (proj != null)
            {
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (player.position - proj.transform.position).normalized;
                    rb.linearVelocity = dir * controller.projectileSpeed;
                }
            }
        }

        foreach (GameObject proj in ring)
        {
            if (proj != null)
                Destroy(proj, 8f);
        }
    }
    IEnumerator MoveProjectileToPosition(GameObject proj, Vector3 target, float duration)
    {
        if (proj == null) yield break;

        Vector3 start = proj.transform.position;
        float t = 0f;

        while (t < duration)
        {
            if (proj == null) break;

            proj.transform.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;

            yield return null;
        }

        if (proj != null)
            proj.transform.position = target;
    }
    void FireExplodingProjectileAtPlayer(GameObject prefab, int explosionCount, float curveStrength = 0f, float explodeDistanceThreshold = 0.5f)
    {
        Vector3 targetPos = controller.playerSpawnRef.position;
        Vector3 dir = (targetPos - projectileSpawnCenter.position).normalized;

        GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * controller.projectileSpeed;
        StartCoroutine(ExplodeWhenReachingTarget(proj, targetPos, explosionCount, curveStrength, explodeDistanceThreshold));
    }
    IEnumerator ExplodeWhenReachingTarget(GameObject proj, Vector3 targetPos, int explosionCount, float curveStrength, float threshold)
    {
        while (proj != null)
        {
            float dist = Vector3.Distance(proj.transform.position, targetPos);

            if (dist <= threshold)
            {
                Vector3 explosionPos = proj.transform.position;
                Destroy(proj);

                for (int i = 0; i < explosionCount; i++)
                {
                    float angle = (360f / explosionCount) * i;

                    Vector3 dir = new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        0f,
                        Mathf.Sin(angle * Mathf.Deg2Rad)
                    );

                    GameObject child = Instantiate(projectile1Prefab, explosionPos, Quaternion.identity);
                    StartCoroutine(MoveCurvedProjectile(child, dir, controller.projectileSpeed, curveStrength));

                    Destroy(child, 3f);
                }
                yield break;
            }

            yield return null;
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

}
