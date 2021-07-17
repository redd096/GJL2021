﻿using System.Collections.Generic;
using UnityEngine;

public class ExplodeStateEnemyKamiCATze : StateMachineBehaviour
{
    [Header("Wait Before Explode")]
    [SerializeField] float delayBeforeExplode = 1;

    [Header("Explosion")]
    [SerializeField] [Min(0)] float radiusAreaDamage = 1;     //damage characters in radius area
    [SerializeField] bool ignoreShield = false;
    [SerializeField] float damage = 10;
    [SerializeField] float knockBack = 10;

    [Header("Reduce health when explode")]
    [SerializeField] float selfDamage = 200;

    Enemy enemy;
    float timeBeforeExplode;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set vars
        timeBeforeExplode = Time.time + delayBeforeExplode;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //wait timer, then explode
        if (Time.time > timeBeforeExplode)
        {
            if(radiusAreaDamage > 0)
                DamageInArea();

            Explosion();
        }
    }

    /// <summary>
    /// Explosion, damage in area
    /// </summary>
    void DamageInArea()
    {
        //be sure to not hit again the same
        List<IDamageable> damageables = new List<IDamageable>();

        //find every object damageable in area
        foreach (Collider2D col in Physics2D.OverlapCircleAll(enemy.transform.position, radiusAreaDamage))
        {
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null && damageables.Contains(damageable) == false)
            {
                //add only one time in the list, and do damage and knockback
                damageables.Add(damageable);
                damageable.GetDamage(damage, ignoreShield, enemy.transform.position);
                damageable.PushBack((col.transform.position - enemy.transform.position).normalized * knockBack, enemy.transform.position);
            }
        }
    }

    void Explosion()
    {
        //set if target still in vision, then change state
        //enemy.SetTargetSetted(enemy.CheckTargetStillInVision());
        enemy.SetState("Next State");

        //call back to patrol state (if go to charge will be called next state again)
        enemy.onBackToPatrolState?.Invoke();

        //explosion self damage
        enemy.GetDamage(selfDamage);
    }
}
