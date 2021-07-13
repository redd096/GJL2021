using UnityEngine;
using redd096;

public class NormalStatePlayer : StateMachineBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5;

    [Header("Dash")]
    [SerializeField] bool canDash = true;
    [CanShow("canDash")] [SerializeField] bool dashToAimDirection = true;
    [CanShow("canDash")] [SerializeField] float dashForce = 20;
    [CanShow("canDash")] [SerializeField] float dashDelay = 1;

    Player player;
    bool attacking;
    float lastDash;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        player = animator.GetComponent<Player>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //inputs
        Movement(InputRedd096.GetValue<Vector2>("Move"));
        Aim(InputRedd096.GetValue<Vector2>("Aim"));
        Attack(InputRedd096.GetButton("Attack"));
        Dash(InputRedd096.GetButtonDown("Dash"));
    }

    #region private API

    void Movement(Vector2 inputMovement)
    {
        //set velocity
        player.MoveCharacter(inputMovement.normalized, speed);
    }

    void Aim(Vector2 inputAim)
    {
        //set direction player using mouse position
        if (InputRedd096.IsCurrentControlScheme(player.playerInput, "KeyboardAndMouse"))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(inputAim);
            player.DirectionAim = (mousePosition - new Vector2(player.transform.position.x, player.transform.position.y)).normalized;
        }
        //or using analog
        else
        {
            player.DirectionAim = inputAim.normalized;
        }
    }

    void Attack(bool inputAttack)
    {
        //when press, attack
        if(inputAttack && attacking == false)
        {
            attacking = true;
            player.CurrentWeapon?.PressAttack();
        }
        //on release, stop it if automatic shoot
        else if(inputAttack == false && attacking)
        {
            attacking = false;
            player.CurrentWeapon?.ReleaseAttack();
        }
    }

    void Dash(bool inputDash)
    {
        if (canDash == false)
            return;

        //on press, check delay between dash
        if(inputDash && Time.time > lastDash + dashDelay)
        {
            lastDash = Time.time;

            //add as push
            player.PushBack((dashToAimDirection ? player.DirectionAim : player.DirectionInput) * dashForce);
        }
    }

    #endregion
}
