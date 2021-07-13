using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PatrolStateEnemy : StateMachineBehaviour
{
    [Header("Patrol Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float approxReachPosition = 0.1f;

    [Header("Patrol Rules (stop at every node or when finish path")]
    [SerializeField] bool alwaysInMovement = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] bool stopAtEveryNode = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] float timeToWait = 1;

    Enemy enemy;
    List<Node> path;
    float timerBeforeMove;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //reset vars
        path = null;
        timerBeforeMove = 0;
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

        //move if there is no timer to wait
        if(Time.time > timerBeforeMove)
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
                    timerBeforeMove = Time.time + timeToWait;
            }

            return;
        }

        //else move to next node in the path
        enemy.MoveCharacter((path[0].worldPosition - enemy.transform.position).normalized, speed);
    }

    #endregion

    void CheckPlayerIsFound()
    {
        //if player is found, change state
        if (enemy.CheckPlayerIsFound())
            enemy.SetState("Target Found");
    }
}
