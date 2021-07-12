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

        //aim
        Aim(InputRedd096.GetValue<Vector2>("Aim"));
    }

    #region private API

    void Movement(Vector2 inputMovement)
    {
        //set velocity
        player.Rb.velocity = inputMovement.normalized * speed;
    }

    void Aim(Vector2 inputAim)
    {
        //set direction player using mouse position
        if (InputRedd096.IsCurrentControlScheme(player.playerInput, "KeyboardAndMouse"))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(inputAim);
            player.DirectionPlayer = (mousePosition - new Vector2(player.transform.position.x, player.transform.position.y)).normalized;
        }
        //or using analog
        else
        {
            player.DirectionPlayer = inputAim.normalized;
        }
    }

    #endregion
}
