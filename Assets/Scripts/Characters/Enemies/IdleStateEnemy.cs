using UnityEngine;

public class IdleStateEnemy : StateMachineBehaviour
{
    [Header("Duration Idle")]
    [SerializeField] float durationIdle = 2;

    Enemy enemy;
    float timerIdle;

    //Stay still for few seconds
    //after few seconds, call "Next State"
    //
    //when call "Next State" call also enemy.onNextState event

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
        //call next state event
        enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
