using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] GameObject grenadeExplosionVfxPrefab;
    [SerializeField] GameObject trailRenderer;
    [SerializeField] AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action onGrenadeActionComplete;
    private Vector3 positionXZ;
    private float totalDistance;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;

        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float targetReachedDistance = .2f;
        if (Vector3.Distance(positionXZ, targetPosition) < targetReachedDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;

            Instantiate(grenadeExplosionVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);

            onGrenadeActionComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeActionComplete)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.onGrenadeActionComplete = onGrenadeActionComplete;

        positionXZ = transform.position;
        positionXZ.y = 0f;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
