using UnityEngine;
using redd096;

public class NormalStatePlayer : StateMachineBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5;

    [Header("Dash")]
    [SerializeField] bool canDash = true;
    [CanShow("canDash")] [SerializeField] float dashForce = 20;
    [CanShow("canDash")] [SerializeField] float dashDelay = 1;
    [CanShow("canDash")] [SerializeField] float durationInvincible = 0.2f;

    Player player;
    bool attacking;
    float timeBeforeNextDash;

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
        Interact(InputRedd096.GetButtonDown("Interact"));

        //pause
        PauseGame(InputRedd096.GetButtonDown("Pause"));
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
            player.AimWithCharacter(mousePosition - new Vector2(player.transform.position.x, player.transform.position.y));
        }
        //or using analog
        else
        {
            //don't reset when release analog, keep last rotation
            if(inputAim != Vector2.zero)
                player.AimWithCharacter(inputAim);
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
        if(inputDash && Time.time > timeBeforeNextDash)
        {
            timeBeforeNextDash = Time.time + dashDelay;

            //add as push in Aim Direction
            if(player.DashToAimDirection)
            {
                player.PushBack(player.DirectionAim * dashForce);
            }
            //or push in Input Direction (if no input, dash right or left)
            else
            {
                if (player.DirectionInput != Vector2.zero)
                    player.PushBack(player.DirectionInput * dashForce);
                else
                    player.PushBack((player.transform.localScale.x > 0 ? Vector2.right : Vector2.left) * dashForce);
            }

            //set invincible
            player.SetInvincible(durationInvincible);

            //call event
            player.onDash?.Invoke();
        }
    }

    void Interact(bool inputInteract)
    {
        //on press, check nearest interactable
        if(inputInteract)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(player.transform.position, player.RadiusInteract);
            IInteractable interactable = FindNearest(cols, player.transform.position);

            //interact
            interactable?.Interact(player);
        }
    }

    /// <summary>
    /// Find nearest to position
    /// </summary>
    IInteractable FindNearest(Collider2D[] collection, Vector3 position)
    {
        IInteractable nearest = default;
        float distance = Mathf.Infinity;

        //foreach element in the collection
        foreach (Collider2D element in collection)
        {
            //only if there is element
            if (element == null)
                continue;

            //only if is interactable
            IInteractable interactable = element.GetComponentInParent<IInteractable>();
            if (interactable == null)
                continue;

            //check distance to find nearest
            float newDistance = Vector3.Distance(element.transform.position, position);
            if (newDistance < distance)
            {
                distance = newDistance;
                nearest = interactable;
            }
        }

        return nearest;
    }

    #endregion

    void PauseGame(bool inputPause)
    {
        //pause
        if (inputPause)
        {
            SceneLoader.instance.PauseGame();
        }
    }
}
