using UnityEngine;
using redd096;

public class IdleStateEnemy : StateMachineBehaviour
{
    [Header("Duration Idle")]
    [SerializeField] float durationIdle = 2;

    [Header("Aim at Target")]
    [SerializeField] bool aimAtTarget = true;
    [CanShow("aimAtTarget")] [SerializeField] bool canSeeThroughWalls = false;

    Enemy enemy;
    float timerIdle;

    //Stay still for few seconds
    //after few seconds, call "Next State"
    //
    //when call "Next State" call also enemy.onNextState event
    //
    //can look at last target position                              (try change target if null)
    //after few seconds, call Next State

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set vars
        timerIdle = Time.time + durationIdle;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(aimAtTarget)
        {
            //check if there is target and save its last position (change target if null)
            CheckTarget();

            //look at target last position
            LookAtTargetLastPosition();
        }

        //wait timer, then finish idle
        if (Time.time > timerIdle)
            FinishIdle();
    }

    #region private API

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

    void LookAtTargetLastPosition()
    {
        //aim at target
        enemy.AimWithCharacter(enemy.LastTargetPosition - enemy.transform.position);
    }

    #endregion

    void FinishIdle()
    {
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
