using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject sword;

    private void Start()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnMoveStart += MoveAction_OnMoveStart;
            moveAction.OnMoveStop += MoveAction_OnMoveStop;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSlashStart += SwordAction_OnSlashStart;
            swordAction.OnSlashComplete += SwordAction_OnSlashComplete;
        }

        sword.SetActive(false);
        rifle.SetActive(true);
    }

    private void MoveAction_OnMoveStart(object sender, EventArgs e) 
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnMoveStop(object sender, EventArgs e) 
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e) 
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = 
            Instantiate(bulletProjectilePrefab, shootPointTransform.transform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = bulletProjectile.transform.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void SwordAction_OnSlashStart(object sender, EventArgs e)
    {
        rifle.SetActive(false);
        sword.SetActive(true);
        animator.SetTrigger("SwordSlash");
    }

    private void SwordAction_OnSlashComplete(object sender, EventArgs e)
    {
        sword.SetActive(false);
        rifle.SetActive(true);
    }
}
