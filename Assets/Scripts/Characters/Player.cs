using UnityEngine.InputSystem;

public class Player : Character
{
    public PlayerInput playerInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        //get references
        playerInput = GetComponent<PlayerInput>();
    }
}
