using System.Collections;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float damage;
    public float duration = 0.05f;

    public GameObject effectPrefab;

    private DestroyScript destroyScript;

    public Transform transformToFollow;

    private void Awake()
    {
        destroyScript = GetComponentInParent<DestroyScript>();

        if (destroyScript == null)
        {
            destroyScript = GetComponent<DestroyScript>();
        }
    }

    private void OnEnable()
    {
        destroyScript.DestroyAfter(duration);

        if (effectPrefab != null)
        {
            if (transformToFollow != null)
            {

               // Instantiate the effect at the position of the transform to follow
                GameObject effectInstance = Instantiate(effectPrefab, transformToFollow.position, transformToFollow.rotation);
            }
            else
            {
                // Instantiate the effect at the position of this object
                GameObject effectInstance2 = Instantiate(effectPrefab, transform.position, transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
                // Optionally, you can destroy the explosion object after dealing damage
            }
        }
    }
}
