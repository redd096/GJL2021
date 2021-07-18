using UnityEngine;
using redd096;

public class IdleStateEnemyBoss : StateMachineBehaviour
{
    [Header("Duration State")]
    [SerializeField] float durationState = 2;

    [Header("Aim at Target")]
    [SerializeField] bool aimAtTarget = true;
    [CanShow("aimAtTarget")] [SerializeField] bool canSeeThroughWalls = false;

    [Header("Event Animation On Enter State")]
    [SerializeField] bool callIdleStateEvent = true;

    EnemyBoss enemy;
    float timeFinishState;

    //Stay still for few seconds
    //after few seconds, call "Next State"
    //
    //when enter state call also enemy.onIdleState event
    //
    //can look at last target position                              (try change target if null)
    //after few seconds, call Next State

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<EnemyBoss>();

        //set vars
        timeFinishState = Time.time + durationState;

        //call state event
        if (callIdleStateEvent)
            enemy.onIdleState?.Invoke();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (aimAtTarget)
        {
            //check if there is target and save its last position (change target if null)
            CheckTarget();

            //look at target last position
            LookAtTargetLastPosition();
        }

        //wait timer, then finish state
        if (Time.time > timeFinishState)
            FinishState();
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

    void FinishState()
    {
        //change to next state
        enemy.SetState("Next State");
    }
}
