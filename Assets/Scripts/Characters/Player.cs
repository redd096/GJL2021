using UnityEngine.InputSystem;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{
    public PlayerInput playerInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        //get references
        playerInput = GetComponent<PlayerInput>();

        //add to level manager list
        if (GameManager.instance.levelManager)
            GameManager.instance.levelManager.Players.Add(this);
    }

    void OnDestroy()
    {
        //remove from level manager list
        if (GameManager.instance.levelManager)
            GameManager.instance.levelManager.Players.Remove(this);
    }
}
