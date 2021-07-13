using UnityEngine;

public class IdleStateEnemyCharger : StateMachineBehaviour
{
    [Header("Duration Idle")]
    [SerializeField] float durationIdle = 3;

    Enemy enemy;
    float timerIdle;

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

        //wait timer, then finish idle
        if (Time.time > timerIdle)
            FinishIdle();
    }

    void FinishIdle()
    {
        //set if target still in vision, then change state
        enemy.SetTargetSetted(enemy.CheckTargetStillInVision());
        enemy.SetState("Next State");
    }
}
