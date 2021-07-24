﻿using System.Collections.Generic;
using UnityEngine;
using redd096;

public class MoveToLastTargetPositionStateEnemy : StateMachineBehaviour
{
    [Header("Keep Knockback players")]
    [SerializeField] bool keepKnockbackPlayers = true;

    [Header("Delay before start move")]
    [SerializeField] float delayBeforeMove = 0;

    [Header("Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float approxReachNode = 0.05f;

    [Header("change state After Seconds or when Reach Target ??")]
    [SerializeField] bool finishAfterTime = true;
    [CanShow("finishAfterTime")] [SerializeField] float secondsBeforeStop = 1;
    [CanShow("finishAfterTime", NOT = true)] [SerializeField] float distanceFromTargetPosition = 1f;

    [Header("Can See Through Walls")]
    [SerializeField] bool canSeeThroughWalls = false;

    [Header("Update Patrol Position")]
    [SerializeField] bool updatePatrolPosition = true;

    Enemy enemy;
    List<Node> path;
    float timerBeforeMove;

    float timerToChangeState;

    //Move to last target position using pathfinding
    //when reach last target position, call "Next State"    (only if setted to NOT finish after seconds -so reach target-)
    //after few seconds, call "Next State"                  (only if setted to finish after seconds)
    //
    //delay before move
    //update path to last target position at every frame                                (try change target if null)
    //if no path, if setted to reach position will call Next State, else stay still

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //reset vars
        path = null;
        timerBeforeMove = Time.time + delayBeforeMove;

        //set time to finish this state (only if setted finishAfterTime)
        timerToChangeState = Time.time + secondsBeforeStop;

        //stop knockback players on hit (if necessary)
        if (keepKnockbackPlayers == false)
            enemy.SetKnobackPlayerOnHit(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //update path (change target if null)
        GetPathToLastTargetPosition();

        //check finish movement
        if ((finishAfterTime && CheckFinishSeconds())                               //after seconds
            || (finishAfterTime == false && CheckReachedLastTargetPosition()))      //when reach target (or no path)
        {
            return;
        }

        //if no path, stop
        if (path == null || path.Count <= 0)
            return;

        //look at next point in path
        LookAtNextPoint();

        //move if there is no timer to wait
        if (Time.time > timerBeforeMove)
            Movement();
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

    void GetPathToLastTargetPosition()
    {
        //check target still in vision, and update Last Target Position (or change target)
        if(enemy.Target)
            enemy.CheckTargetStillInVision(canSeeThroughWalls);
        else
            enemy.CheckPlayerIsFound();

        //update path to it
        path = AStar.instance.FindPath(enemy.transform.position, enemy.LastTargetPosition);
    }

    void LookAtNextPoint()
    {
        //aim at next point
        enemy.AimWithCharacter(path[0].worldPosition - enemy.transform.position);
    }

    void Movement()
    {
        //if reached position, remove node
        if (Vector2.Distance(enemy.transform.position, path[0].worldPosition) <= approxReachNode)
        {
            path.RemoveAt(0);

            return;
        }

        //else move to next node in the path
        enemy.MoveCharacter((path[0].worldPosition - enemy.transform.position).normalized, speed);
    }

    #endregion

    bool CheckFinishSeconds()
    {
        //check if finished time
        if(Time.time > timerToChangeState)
        {
            enemy.SetState("Next State");
            return true;
        }

        return false;
    }

    bool CheckReachedLastTargetPosition()
    {
        //check if reached last target position
        if (path == null || path.Count <= 0                                                                             //no path (so can't move)
            || Vector2.Distance(enemy.transform.position, enemy.LastTargetPosition) <= distanceFromTargetPosition)      //or reached position
        {
            enemy.SetState("Next State");
            return true;
        }

        return false;
    }
}
