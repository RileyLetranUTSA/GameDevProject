using TMPro;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public Transform Cam;
    public float speed = 12f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 20f;
    public float shootCooldown = 0.2f;
    private float nextShootTime = 0f;
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Charms")]
    public Transform playerSpawnRef;
    public GameObject charmPrefab;
    public int maxCharms = 2;
    [HideInInspector] public int currentCharms;
    public GameObject charmContainer;
    public UnityEngine.UI.Image charmIconPrefab;
    public AudioClip charmSound;

    [Header("Score")]
    public int score = 0;
    public TextMeshProUGUI inGameScore;

    [Header("Damage Switching for Testing")]
    public float lowDamage = 1f;
    public float highDamage = 75f;
    private float currentDamage = 1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentCharms = maxCharms;

        UpdateCharmUI();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleCharms();

        HandleDamage();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = 6f;

        if (direction.magnitude >= 0.1f && !Input.GetMouseButton(0))
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }
        else if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = Cam.transform.TransformDirection(direction);
            moveDir.y = 0f;
            moveDir.Normalize();
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }
    }
    void HandleShooting()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextShootTime)
        {
            audioSource.PlayOneShot(shootSound,0.09f);

            nextShootTime = Time.time + shootCooldown;

            Vector3 camForward = new Vector3(Cam.forward.x, 0f, Cam.forward.z).normalized;
            if (camForward.sqrMagnitude > 0f)
                transform.rotation = Quaternion.LookRotation(camForward);

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            PlayerProjectileHandler ph = projectile.GetComponent<PlayerProjectileHandler>();
            if (ph != null)
            {
                ph.damage = currentDamage;
            }

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = camForward * projectileSpeed;

            Destroy(projectile, 5f);
        }
    }

    void HandleDamage()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentDamage = (currentDamage == lowDamage) ? highDamage : lowDamage;

            Debug.Log("Projectile Damage switched to: " + currentDamage);
        }
    }

    void HandleCharms()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SpawnCharm();
        }
    }


    void SpawnCharm()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (currentCharms <= 0 || charmPrefab == null)
        {
            return;
        }

        audioSource.PlayOneShot(charmSound);
        
        GameObject newCharm = Instantiate(charmPrefab, playerSpawnRef.position, Quaternion.identity);
        Charm charmComponent = newCharm.GetComponent<Charm>();
        if (charmComponent != null)
            charmComponent.StartExpansion();

        currentCharms--;
        UpdateCharmUI();
    }

    public void UpdateCharmUI()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        
        foreach (Transform child in charmContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentCharms; i++)
        {
            UnityEngine.UI.Image charmIcon = Instantiate(charmIconPrefab, charmContainer.transform);
            charmIcon.gameObject.SetActive(true);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void UpdateScoreUI()
    {
        inGameScore.text = "Score: " + score.ToString();
    }

}
