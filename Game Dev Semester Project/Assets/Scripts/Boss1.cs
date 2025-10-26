using UnityEngine;

public class Boss1 : BossBase
{
    public GameObject projectile1Prefab;
    public Transform projectileSpawnCenter;
    private float rotationOffset = 0f;

    void Start()
    {
        if (controller != null)
        {
            Init(controller);

        }
    }

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
                controller.shootInterval = 1f;
                controller.projectileSpeed = 8f;
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
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount,projectile1Prefab, rotationOffset);
    }
    void CharmPhase2Attack()
    {
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount,projectile1Prefab, rotationOffset);
    }
    void CharmPhase3Attack()
    {
        int projectileCount = 2;
        rotationOffset = (rotationOffset + 20f) % 360f;
        FireCircle(projectileCount,projectile1Prefab, rotationOffset);
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

            Destroy(proj, 10f); // lifetime
        }
    }
    void FireCurvedCircle(int projectileCount, GameObject prefab, float angleOffset = 0f, float curveStrength = 50f)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i + angleOffset;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(prefab, projectileSpawnCenter.position, Quaternion.identity);

            // Start moving the projectile with a curve
            StartCoroutine(MoveCurvedProjectile(proj, dir, controller.projectileSpeed, curveStrength));

            Destroy(proj, 10f); // lifetime
        }
    }
    System.Collections.IEnumerator MoveCurvedProjectile(GameObject proj, Vector3 direction, float speed, float curveStrength)
    {
        float angle = 0f;

        while (proj != null)
        {
            // Curve the direction over time
            angle += curveStrength * Time.deltaTime;
            Vector3 curvedDir = Quaternion.Euler(0, angle, 0) * direction;

            // Move in the new direction
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

            // Start moving the projectile with a curve
            StartCoroutine(ReversedMoveCurvedProjectile(proj, dir, controller.projectileSpeed, curveStrength));

            Destroy(proj, 10f); // lifetime
        }
    }
    System.Collections.IEnumerator ReversedMoveCurvedProjectile(GameObject proj, Vector3 direction, float speed, float curveStrength)
    {
        float angle = 0f;

        while (proj != null)
        {
            // Curve the direction over time
            angle -= curveStrength * Time.deltaTime;
            Vector3 curvedDir = Quaternion.Euler(0, angle, 0) * direction;

            // Move in the new direction
            proj.transform.position += curvedDir * speed * Time.deltaTime;

            yield return null;
        }
    }
}
