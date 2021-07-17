using UnityEngine;

public class WarningStateEnemy : StateMachineBehaviour
{
    [Header("Duration Warning")]
    [SerializeField] float durationWarning = 0.5f;

    Enemy enemy;
    float timeFinishWarning;

    //Stay still for few seconds looking at target
    //when player is lost, call "Target Lost"       (check first this)
    //after few seconds, call "Next State"          (check only if Target is NOT lost)
    //
    //when call "Next State" call also enemy.onNextState event
    //
    //look at target
    //if target is no more in vision area, call target Lost
    //else after few seconds, call Next State

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set timer
        timeFinishWarning = Time.time + durationWarning;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //look at target
        LookAtTarget();

        //check if target still in vision - else back to patrol
        if (CheckTargetStillInVision())
            return;

        //if finish timer, call it
        if(Time.time > timeFinishWarning)
            FinishTimer();
    }

    #region private API

    void LookAtTarget()
    {
        if (enemy.Target == null)
            return;

        //aim at target
        enemy.AimWithCharacter(enemy.Target.transform.position - enemy.transform.position);
    }

    #endregion

    bool CheckTargetStillInVision()
    {
        //if player is found, change state
        if (enemy.CheckTargetStillInVision() == false)
        {
            enemy.SetState("Target Lost");
            return true;
        }

        return false;
    }

    void FinishTimer()
    {
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
