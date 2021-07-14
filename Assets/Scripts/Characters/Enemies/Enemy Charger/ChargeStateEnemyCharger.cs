﻿using System.Collections;
using UnityEngine;
using redd096;

public class ChargeStateEnemyCharger : StateMachineBehaviour
{
    [Header("Charge Movement")]
    [SerializeField] float speed = 3;
    [SerializeField] bool followTarget = true;
    [CanShow("followTarget")] [SerializeField] float rotationSpeed = 0.05f;

    [Header("Charge Damage")]
    [SerializeField] float damage = 30;
    [SerializeField] float knockBack = 10;

    [Header("Check hit Something")]
    [SerializeField] float speedCheckIfHitWall = 2.5f;
    [SerializeField] float radiusCastToCheckWhatHit = 0.5f;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float calculatedSpeed = 0;

    Enemy enemy;
    Vector3 previousPosition;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //start coroutine
        enemy.StartCoroutine(CheckHitWallCoroutine());

        //call next state event
        enemy.onNextState?.Invoke();
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
            enemy.DirectionAim = Quaternion.AngleAxis(angle > 0 ? rotationSpeed : -rotationSpeed, Vector3.forward) * enemy.DirectionAim;
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
            enemy.CheckTargetStillInVision();
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
            //calculate speed (don't use rigidbody, to not glitch when hit walls), and save previous position
            calculatedSpeed = (enemy.transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
            previousPosition = enemy.transform.position;

            //hit wall or character
            if (calculatedSpeed <= speedCheckIfHitWall)
            {
                HitSomething();
                break;
            }

            //use fixed update
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    void HitSomething()
    {
        //circle cast to check what hit
        RaycastHit2D[] hits = Physics2D.CircleCastAll(enemy.transform.position, radiusCastToCheckWhatHit, enemy.DirectionAim, 0.2f);

        foreach (RaycastHit2D hit in hits)
        {
            //if hit something and isn't itself
            if (hit.transform && hit.transform.GetComponentInParent<Character>() != enemy)
            {
                //if hit something damageable, do damage and push back
                IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
                damageable?.GetDamage(damage, enemy.transform.position);
                damageable?.PushBack(enemy.DirectionAim * knockBack, enemy.transform.position);
            }
        }

        //move to next state
        enemy.SetState("Next State");
    }
}
