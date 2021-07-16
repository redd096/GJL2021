﻿using UnityEngine.InputSystem;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{
    [Header("Interact")]
    public float RadiusInteract = 1.5f;

    public PlayerInput playerInput { get; private set; }

    Animator stateMachine;

    //animation events
    public System.Action onDash { get; set; }

    void OnDrawGizmos()
    {
        //draw radius interact
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, RadiusInteract);
    }

    protected override void Awake()
    {
        base.Awake();

        //if there a weapon saved in game manager, set it (or use weapon prefab for first room)
        GameManager.instance.PickWeaponSaved(this, weaponPrefab);

        //get references
        playerInput = GetComponent<PlayerInput>();
        stateMachine = GetComponent<Animator>();

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        //check pick droppables
        CheckPickDroppables(collision);
    }

    #region overrides

    public override void GetDamage(float damage, Vector2 hitPosition = default)
    {
        base.GetDamage(damage, hitPosition);

        //update health UI
        GameManager.instance.uiManager.UpdateHealth(health, maxHealth);
    }

    public override void PickWeapon(WeaponBASE prefab)
    {
        base.PickWeapon(prefab);

        //save it and update UI
        GameManager.instance.CurrentWeapon = prefab;
        GameManager.instance.uiManager.UpdateWeaponImage(CurrentWeapon?.GetComponentInChildren<SpriteRenderer>().sprite);

        //save also in already seen
        GameManager.instance.WeaponsAlreadyUsed.Add(prefab);
    }

    public override void DropWeapon()
    {
        base.DropWeapon();

        //save it and update UI
        GameManager.instance.CurrentWeapon = null;
        GameManager.instance.uiManager.UpdateWeaponImage(null);
    }

    public override void Die()
    {
        base.Die();

        //call end game
        GameManager.instance.levelManager.EndGame();
    }

    #endregion

    #region private API

    void CheckPickDroppables(Collider2D collision)
    {
        //pick if is droppable
        collision.GetComponentInParent<IDroppable>()?.Pick();
    }

    #endregion

    /// <summary>
    /// Set state to pause or resume
    /// </summary>
    /// <param name="pause"></param>
    public void SetState(string state)
    {
        stateMachine.SetTrigger(state);
    }
}
