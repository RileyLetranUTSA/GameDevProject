using UnityEngine;

public class PlayerProjectileHandler : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        BossController boss = other.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;

        }
    }
}
