using System.Collections.Generic;
using UnityEngine;

public class DamageTargetStateEnemyFollower : StateMachineBehaviour
{
    [Header("Look at Target")]
    [SerializeField] bool lookAtTarget = true;

    [Header("Damage In Front")]
    [SerializeField] float damage = 10;
    [SerializeField] float knockBack = 10;
    [SerializeField] float radiusCastToCheckWhatHit = 0.5f;

    [Header("Wait Before finish State")]
    [SerializeField] float timeToWait = 0.5f;

    Enemy enemy;
    float timerFinishState;

    //Damage in front (can look at target)
    //wait few seconds and call "Next State"

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set vars
        timerFinishState = Time.time + timeToWait;

        //look at target (if necessary)
        if (lookAtTarget)
            LookAtTarget();

        //damage in front
        DamageInFront();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //wait timer, then change state
        if (Time.time > timerFinishState)
            ChangeState();
    }

    #region private API

    void LookAtTarget()
    {
        if (enemy.Target == null)
            return;

        //aim at target
        enemy.AimWithCharacter(enemy.Target.transform.position - enemy.transform.position);
    }

    void DamageInFront()
    {
        //be sure to not hit again the same
        List<IDamageable> damageables = new List<IDamageable>();

        //be sure to not hit this enemy
        damageables.Add(enemy.GetComponent<IDamageable>());

        //find every object damageable in front
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(enemy.transform.position, radiusCastToCheckWhatHit, enemy.DirectionAim, 0.2f))
        {
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable != null && damageables.Contains(damageable) == false)
            {
                //add only one time in the list, and do damage and knockback
                damageables.Add(damageable);
                damageable.GetDamage(damage, false, enemy.transform.position);
                damageable.PushBack(enemy.DirectionAim * knockBack, enemy.transform.position);
            }
        }
    }

    #endregion

    void ChangeState()
    {
        //change to next state
        enemy.SetState("Next State");
    }
}
