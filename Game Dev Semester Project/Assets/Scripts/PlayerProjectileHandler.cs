using UnityEngine;

public class PlayerProjectileHandler : MonoBehaviour
{
    public float damage = 10f;  // how much damage this projectile does

    private void OnTriggerEnter(Collider other)
    {
        BossController boss = other.GetComponent<BossController>();
        // Check if we hit the boss
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;

        }
    }
}
