using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    public float lifetime = 99f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
