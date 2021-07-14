using UnityEngine;

public class WarningStateEnemy : StateMachineBehaviour
{
    [Header("Duration Warning")]
    [SerializeField] float durationWarning = 0.5f;

    Enemy enemy;
    float timerBeforeCharge;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set timer
        timerBeforeCharge = Time.time + durationWarning;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //look at target
        LookAtTarget();

        //check if target still in vision - else back to patrol
        if (CheckTargetStillInVision())
            return;

        //if finish timer, start to charge
        if(Time.time > timerBeforeCharge)
            ChargeTarget();
    }

    #region private API

    void LookAtTarget()
    {
        if (enemy.Target == null)
            return;

        //aim at target
        enemy.DirectionAim = (enemy.Target.transform.position - enemy.transform.position).normalized;
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

    void ChargeTarget()
    {
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
