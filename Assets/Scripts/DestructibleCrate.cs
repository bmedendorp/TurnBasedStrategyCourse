using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : DynamicBlocker
{
    [SerializeField] Transform destroyedCratePrefab;

    public void Damage()
    {
        Transform crate = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
        ApplyExplosionToCrate(crate, 150f, transform.position, 15f);
        
        Destroy(gameObject);
    }

    private void ApplyExplosionToCrate(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToCrate(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
