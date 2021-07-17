using UnityEngine;

public class LookLastTargetPositionStateEnemy : StateMachineBehaviour
{
    [Header("Time to wait")]
    [SerializeField] float timeToWait = 1;

    Enemy enemy;
    float timeFinishState;

    //(It's the same as IdleState, but look at last target position)
    //Stay still for few seconds looking at last target position
    //after few seconds, call "Next State"
    //
    //when call "Next State" call also enemy.onNextState event
    //
    //look at last target position                                  (try change target if null)
    //after few seconds, call Next State

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set timer
        timeFinishState = Time.time + timeToWait;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //check if there is target and save its last position (change target if null)
        CheckTarget();

        //look at target last position
        LookAtTargetLastPosition();

        //wait, then finish state
        if (Time.time > timeFinishState)
            FinishState();
    }

    #region private API

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

    void LookAtTargetLastPosition()
    {
        //aim at target
        enemy.AimWithCharacter(enemy.LastTargetPosition - enemy.transform.position);
    }

    #endregion

    void FinishState()
    {
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
