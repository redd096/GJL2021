using UnityEngine;

public class CheckTargetIsSetted : StateMachineBehaviour
{
    [Header("Event Animation")]
    [SerializeField] bool callNextStateEvent = true;
    [SerializeField] bool callTargetSettedEvent = true;

    Enemy enemy;

    //Set in state machine "Target Setted" true or false
    //call "Next State"
    //
    //when set "Target Setted" call also enemy.onTargetSetted event
    //when call "Next State" call also enemy.onNextState event

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set if target is setted, then change state
        SetTargetSetted();
        FinishState();
    }

    void SetTargetSetted()
    {
        //call target setted event
        if(callTargetSettedEvent)
            enemy.onTargetSetted?.Invoke(enemy.Target != null);

        //set if target is setted
        enemy.SetTargetSetted(enemy.Target != null);
    }

    void FinishState()
    {
        //call next state event
        if (callNextStateEvent)
            enemy.onNextState?.Invoke();

        //change to next state
        enemy.SetState("Next State");
    }
}
