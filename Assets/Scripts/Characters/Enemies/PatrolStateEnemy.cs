using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PatrolStateEnemy : StateMachineBehaviour
{
    [Header("Delay before start move")]
    [SerializeField] float delayFirstMovement = 0.3f;

    [Header("Patrol Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float approxReachPosition = 0.1f;

    [Header("Patrol Rules (stop at every node or when finish path")]
    [SerializeField] bool alwaysInMovement = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] bool stopAtEveryNode = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] float timeToWait = 1;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float remainingTime;
    [ReadOnly] [SerializeField] List<Node> path = new List<Node>();

    Enemy enemy;
    float timerBeforeMove;

    //Patrolling with pathfinding, inside area setted in enemy
    //when found player, call "Target Found"
    //
    //delay before first movement
    //can delay between every node or when finish path
    //if no path, try get new one, else stay still

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //reset vars
        path = null;
        timerBeforeMove = Time.time + delayFirstMovement;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //if not path, find new one
        if (path == null || path.Count <= 0)
        {
            path = enemy.GetPath();
            return;
        }

        //look at next point in path
        LookAtNextPoint();

        //debug
        remainingTime = timerBeforeMove - Time.time;

        //move if there is no timer to wait
        if (Time.time > timerBeforeMove)
            Movement();

        //check if found player
        CheckPlayerIsFound();
    }

    #region private API

    void Movement()
    {
        //if reached position, remove node
        if(Vector2.Distance(enemy.transform.position, path[0].worldPosition) <= approxReachPosition)
        {
            path.RemoveAt(0);

            //if need to stop
            if(alwaysInMovement == false)
            {
                //stop at every node or when finish path
                if (stopAtEveryNode || path.Count <= 0)
                {
                    timerBeforeMove = Time.time + timeToWait;
                }
            }

            return;
        }

        //else move to next node in the path
        enemy.MoveCharacter((path[0].worldPosition - enemy.transform.position).normalized, speed);
    }

    void LookAtNextPoint()
    {
        //aim at next point
        enemy.AimWithCharacter(path[0].worldPosition - enemy.transform.position);

        //debug draw path
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0)
                Debug.DrawLine(enemy.transform.position, path[0].worldPosition, Color.cyan);
            else
                Debug.DrawLine(path[i - 1].worldPosition, path[i].worldPosition, Color.cyan);
        }
    }

    #endregion

    void CheckPlayerIsFound()
    {
        //if player is found, change state
        if (enemy.CheckPlayerIsFound())
            enemy.SetState("Target Found");
    }
}
