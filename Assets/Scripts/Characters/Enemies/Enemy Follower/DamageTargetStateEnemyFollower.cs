using System.Collections.Generic;
using UnityEngine;
using redd096;

public class DamageTargetStateEnemyFollower : StateMachineBehaviour
{
    [Header("Keep Knockback players")]
    [SerializeField] bool keepKnockbackPlayers = false;

    [Header("Look at Target")]
    [SerializeField] bool lookAtTarget = true;
    [SerializeField] bool canSeeThroughWalls = false;

    [Header("Move to Target before do damage?")]
    [SerializeField] bool moveToTarget = true;
    [CanShow("moveToTarget")] [SerializeField] float speed = 5;

    [Header("Damage In Front")]
    [SerializeField] float damage = 10;
    [SerializeField] float knockBack = 10;
    [SerializeField] float radiusCastToCheckWhatHit = 0.5f;

    [Header("Wait Before finish State")]
    [SerializeField] float timeToWait = 0.5f;

    [Header("Update Patrol Position")]
    [SerializeField] bool updatePatrolPosition = true;

    Enemy enemy;
    float timerFinishState;

    //can set to move to target until hit before all
    //
    //Damage in front (can look at target)
    //wait few seconds and call "Next State"

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //stop knockback players on hit (if necessary)
        if (keepKnockbackPlayers == false)
            enemy.SetKnobackPlayerOnHit(false);

        //if no move to target, set timer and do damage
        if (moveToTarget == false)
        {
            timerFinishState = Time.time + timeToWait;

            //look at target (if necessary)
            if (lookAtTarget)
                LookAtTarget();

            //damage in front
            DamageInFront();
        }
        //else set timer at 0 so in update move to target
        else
        {
            timerFinishState = 0;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(timerFinishState <= 0)
        {
            //if lose target or hit it, set timer and do damage
            if(enemy.CheckTargetStillInVision(canSeeThroughWalls) == false || CheckHit())
            {
                timerFinishState = Time.time + timeToWait;
                DamageInFront();
                return;
            }

            //else look at target and move to it
            if (lookAtTarget)
                LookAtTarget();

            MoveToTarget();
        }

        //wait timer, then change state
        if (timerFinishState > 0 && Time.time > timerFinishState)
            ChangeState();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        //reset knockback players on hit (if necessary)
        if (keepKnockbackPlayers == false)
            enemy.SetKnobackPlayerOnHit(true);

        //update patrol position
        if (updatePatrolPosition)
            enemy.UpdatePatrolPosition();
    }

    #region move to target

    void MoveToTarget()
    {
        //move to target
        enemy.MoveCharacter((enemy.Target.transform.position - enemy.transform.position).normalized, speed);
    }

    bool CheckHit()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();

        //foreach collider, overlap
        foreach (Collider2D collider in enemy.GetComponentsInChildren<Collider2D>())
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(collider, contactFilter2D, cols);

            //check hit player
            foreach (Collider2D col in cols)
                if (col.GetComponentInParent<Player>())
                    return true;
        }

        return false;
    }

    #endregion

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
