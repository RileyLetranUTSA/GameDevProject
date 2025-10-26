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
    public float shootCooldown = 0.3f;
    private float nextShootTime = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
{
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");
    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    float currentSpeed = speed;
    if (Input.GetKey(KeyCode.LeftShift))
        currentSpeed = 6f;

    // Only rotate according to movement if NOT shooting
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
        // still allow movement in any direction while shooting, without changing facing
        Vector3 moveDir = Cam.transform.TransformDirection(direction);
        moveDir.y = 0f;
        moveDir.Normalize();
        controller.Move(moveDir * currentSpeed * Time.deltaTime);
    }
}
    void HandleShooting()
{
    if (Input.GetMouseButton(0) && Time.time >= nextShootTime)
    {
        nextShootTime = Time.time + shootCooldown;

        // Rotate the player **only for shooting**
        Vector3 camForward = new Vector3(Cam.forward.x, 0f, Cam.forward.z).normalized;
        if (camForward.sqrMagnitude > 0f)
            transform.rotation = Quaternion.LookRotation(camForward);

        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = camForward * projectileSpeed;

        Destroy(projectile, 5f);
    }
}

}
