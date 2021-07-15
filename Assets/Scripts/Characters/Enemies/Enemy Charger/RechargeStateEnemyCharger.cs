using UnityEngine;

public class RechargeStateEnemyCharger : StateMachineBehaviour
{
    [Header("Duration Recharge")]
    [SerializeField] float durationRecharge = 1;

    Enemy enemy;
    float timerRecharge;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set timer
        timerRecharge = Time.time + durationRecharge;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //check if there is target and save its last position
        CheckTarget();

        //look at target last position
        LookAtTargetLastPosition();

        //wait, then charge target
        if (Time.time > timerRecharge)
            ChargeTarget();
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

    void ChargeTarget()
    {
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
