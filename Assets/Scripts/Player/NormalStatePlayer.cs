using UnityEngine;
using redd096;

public class NormalStatePlayer : StateMachineBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5;

    Player player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        player = animator.GetComponent<Player>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //move
        Movement(InputRedd096.GetValue<Vector2>("Move"));
    }

    void Movement(Vector2 inputMovement)
    {
        //set velocity
        player.Rb.velocity = inputMovement.normalized * speed;
    }
}
