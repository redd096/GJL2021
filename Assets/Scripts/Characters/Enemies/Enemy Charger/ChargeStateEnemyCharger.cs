using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class ChargeStateEnemyCharger : StateMachineBehaviour
{
    [Header("Keep Knockback players")]
    [SerializeField] bool keepKnockbackPlayers = false;

    [Header("Charge Movement")]
    [SerializeField] float speed = 3;
    [SerializeField] bool followTarget = true;
    [CanShow("followTarget")] [SerializeField] float rotationSpeed = 0.05f;
    [CanShow("followTarget")] [SerializeField] bool canSeeThroughWalls = false;

    [Header("Charge Damage")]
    [SerializeField] float damage = 10;
    [SerializeField] float knockBack = 10;
    [SerializeField] float radiusCastToCheckWhatHit = 0.5f;

    [Header("Check hit Something")]
    [SerializeField] float speedCheckIfHitWall = 2.5f;

    [Header("Event Animation")]
    [SerializeField] bool callNextStateEvent = true;

    [Header("Update Patrol Position")]
    [SerializeField] bool updatePatrolPosition = true;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float calculatedSpeed = 0;

    Enemy enemy;
    Vector3 previousPosition;

    //Move straight in aim direction
    //if follow target, rotate using rotation speed
    //when something stops rigidbody, call "Next State"
    //
    //when call "Next State" call also enemy.onNextState event
    //
    //knockback player on hit -> set to false
    //move to aim direction
    //if follow target, rotate to aim at last target position                                                   (try change target if null)
    //when rigidbody stops (hit something), damage and knockback everything in front, then call Next State
    //knockback player on hit -> set again to true

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //stop knockback players on hit (if necessary)
        if (keepKnockbackPlayers == false)
            enemy.SetKnobackPlayerOnHit(false);

        //start coroutine (to use fixed update)
        enemy.StartCoroutine(CheckHitWallCoroutine());
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //move to aim direction
        Movement();

        //rotate to follow target
        if (followTarget)
            FollowTarget();
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

    #region private API

    void Movement()
    {
        //move to aim direction
        enemy.MoveCharacter(enemy.DirectionAim, speed);
    }

    void FollowTarget()
    {
        if (followTarget == false)
            return;

        //check if there is target and save its last position
        CheckTarget();

        //rotate to look at target, using rotation speed
        float angle = Vector2.SignedAngle(enemy.DirectionAim, (enemy.LastTargetPosition - enemy.transform.position).normalized);
        if(angle != 0)
        {
            enemy.AimWithCharacter(Quaternion.AngleAxis(angle > 0 ? rotationSpeed : -rotationSpeed, Vector3.forward) * enemy.DirectionAim);
        }

        //debug
        Debug.DrawLine(enemy.transform.position, new Vector2(enemy.transform.position.x, enemy.transform.position.y) + enemy.DirectionAim * 2, Color.cyan);
        Debug.DrawLine(enemy.transform.position, enemy.transform.position + (enemy.LastTargetPosition - enemy.transform.position).normalized * 2, Color.cyan);
    }

    void CheckTarget()
    {
        //be sure target still in vision area (and update last position)
        if (enemy.Target)
        {
            enemy.CheckTargetStillInVision(canSeeThroughWalls);
        }
        //else try find new target
        else
        {
            enemy.CheckPlayerIsFound();
        }
    }

    IEnumerator CheckHitWallCoroutine()
    {
        //save previous position and wait to not stop immediatly
        previousPosition = enemy.transform.position;
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (enemy == null)
                break;

            //if hit wall or character
            if (CheckHit())
            {
                //damage and change state
                DamageInFront();
                ChangeState();
                break;
            }

            //use fixed update
            yield return new WaitForFixedUpdate();
        }
    }

    bool CheckHit()
    {
        //calculate speed (don't use rigidbody, to not glitch when hit walls), and save previous position
        calculatedSpeed = (enemy.transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
        previousPosition = enemy.transform.position;

        //hit wall or character
        if (calculatedSpeed <= speedCheckIfHitWall)
        {
            return true;
        }

        return false;
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
        //call next state event
        if(callNextStateEvent)
            enemy.onNextState?.Invoke();

        //move to next state
        enemy.SetState("Next State");
    }
}
