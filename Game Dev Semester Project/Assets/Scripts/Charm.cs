using System.Collections;
using UnityEngine;

public class Charm : MonoBehaviour
{
    public float initialRadius = 3f;
    public float finalRadius = 6f;
    public float expandDuration = 2f;
    public float lifetime = 2.5f;
    public Transform charmVisual;
    private SphereCollider charmCollider;

    void Awake()
    {
        charmCollider = gameObject.AddComponent<SphereCollider>();
        charmCollider.isTrigger = true;
        charmCollider.radius = initialRadius;
    }

    public void StartExpansion()
    {
        StartCoroutine(ExpandCoroutine());
        StartCoroutine(LifetimeCoroutine());
    }

    private IEnumerator ExpandCoroutine()
    {
        float timer = 0f;
        while (timer < expandDuration)
        {
            timer += Time.deltaTime;
            charmCollider.radius = Mathf.Lerp(initialRadius, finalRadius, timer / expandDuration);

            if (charmVisual != null)
                charmVisual.localScale = Vector3.one * charmCollider.radius * 2f;

            yield return null;
        }
        charmCollider.radius = finalRadius;
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
            Destroy(other.gameObject);
    }
}
