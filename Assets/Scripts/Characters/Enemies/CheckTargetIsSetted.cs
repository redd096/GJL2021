using UnityEngine;

public class CheckTargetIsSetted : StateMachineBehaviour
{
    [Header("Check Min Distance")]
    [SerializeField] bool checkMinDistance = true;
    [SerializeField] float minDistance = 1;

    [Header("Event Animation")]
    [SerializeField] bool callNextStateEvent = true;
    [SerializeField] bool callTargetSettedEvent = true;

    Enemy enemy;
    bool isTargetSetted;

    //Set in state machine "Target Setted" true or false
    //call "Next State"
    //
    //when set "Target Setted" call also enemy.onTargetSetted event
    //when call "Next State" call also enemy.onNextState event
    //
    //check if target is setted
    //if necessary check also distance from the target

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //check if target is setted
        CheckTarget();

        //set target setted, then change state
        SetTargetSetted();
        FinishState();
    }

    void CheckTarget()
    {
        //check target is setted
        isTargetSetted = enemy.Target != null;

        //check min distance (if necessary)
        if (isTargetSetted && checkMinDistance)
            isTargetSetted = Vector2.Distance(enemy.transform.position, enemy.Target.transform.position) < minDistance;
    }

    void SetTargetSetted()
    {
        //call target setted event
        if(callTargetSettedEvent)
            enemy.onTargetSetted?.Invoke(isTargetSetted);

        //set if target is setted
        enemy.SetTargetSetted(isTargetSetted);
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
