using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField] Transform destroyedCratePrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetWalkable(gridPosition, false);
    }

    public void Damage()
    {
        Transform crate = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(crate, 150f, transform.position, 10f);
        
        Destroy(gameObject);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

    private void OnDestroy()
    {
        LevelGrid.Instance.SetWalkable(gridPosition, true);
    }
}
