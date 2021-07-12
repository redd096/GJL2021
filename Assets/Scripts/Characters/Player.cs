using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
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
