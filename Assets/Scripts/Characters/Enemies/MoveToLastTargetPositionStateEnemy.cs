using System.Collections.Generic;
using UnityEngine;
using redd096;

public class MoveToLastTargetPositionStateEnemy : StateMachineBehaviour
{
    [Header("Delay before start move")]
    [SerializeField] float delayBeforeMove = 0;

    [Header("Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float approxReachPosition = 0.1f;

    [Header("change state After Seconds or when Reach Target ??")]
    [SerializeField] bool finishAfterTime = true;
    [CanShow("finishAfterTime")] [SerializeField] float secondsBeforeStop = 1;
    [CanShow("finishAfterTime", NOT = true)] [SerializeField] float distanceFromLastTargetPosition = 0.5f;

    Enemy enemy;
    List<Node> path;
    float timerBeforeMove;

    float timerToChangeState;

    //Move to last target position
    //when reach last target position, call "Next State"    (only if setted to NOT finish after seconds -so reach target-)
    //after few seconds, call "Next State"                  (only if setted to finish after seconds)
    //
    //delay before move
    //update path to last target position at every frame (change target if null)
    //if no path, if setted to reach position will call Next State, else stay still

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //reset vars
        path = null;
        timerBeforeMove = Time.time + delayBeforeMove;

        timerToChangeState = Time.time + secondsBeforeStop;
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

    #region private API

    void GetPathToLastTargetPosition()
    {
        //check target still in vision, and update Last Target Position (or change target)
        if(enemy.Target)
            enemy.CheckTargetStillInVision();
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
        if (Vector2.Distance(enemy.transform.position, path[0].worldPosition) <= approxReachPosition)
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
        if (path == null || path.Count <= 0                                                                                 //no path (so can't move)
            || Vector2.Distance(enemy.transform.position, enemy.LastTargetPosition) <= distanceFromLastTargetPosition)      //or reached position
        {
            enemy.SetState("Next State");
            return true;
        }

        return false;
    }
}
