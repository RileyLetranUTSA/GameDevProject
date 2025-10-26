using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    public float lifetime = 7f; // set per projectile type

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
