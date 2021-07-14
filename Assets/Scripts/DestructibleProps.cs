﻿using System.Collections.Generic;
using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public class DestructibleProps : MonoBehaviour, IDamageable
{
    [Header("Prop")]
    [SerializeField] float health = 100;
    [SerializeField] bool removeColliderOnDeath = true;

    [Header("Area Damage")]
    [SerializeField] bool doAreaDamage = false;
    [CanShow("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 1;     //damage characters in radius area
    [CanShow("doAreaDamage")] [SerializeField] float damage = 10;

    [Header("Push")]
    [SerializeField] bool canBePushed = false;
    [CanShow("canBePushed")] [SerializeField] float customDrag = 30;

    [Header("Destroy after few seconds is Dead")]
    [SerializeField] bool destroyAfterSeconds = false;
    [CanShow("destroyAfterSeconds")] [SerializeField] float secondsBeforeDestroy = 10;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 pushForce;          //used to push character (push by recoil, knockback, dash, etc...), will be decreased by customDrag in every frame
    [ReadOnly] [SerializeField] float currentSpeed;         //speed from MovementInput and pushForce

    public Rigidbody2D Rb { get; private set; }

    bool alreadyDead;

    //animation events
    public System.Action onGetDamage { get; set; }
    public System.Action onDie { get; set; }

    void OnDrawGizmos()
    {
        //draw area damage
        if (doAreaDamage && radiusAreaDamage > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusAreaDamage);
        }
    }

    protected virtual void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //move rigidbody
        if (canBePushed)
        {
            MoveRigidbody();
        }
    }

    /// <summary>
    /// On destroy, damage in area
    /// </summary>
    void DamageInArea()
    {
        //be sure to not hit itself
        List<IDamageable> damageables = new List<IDamageable>();

        //find every object damageable in area
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
        {
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null && damageables.Contains(damageable) == false)
            {
                //add only one time in the list, and do damage
                damageables.Add(damageable);
                damageable.GetDamage(damage, transform.position);
            }
        }
    }

    /// <summary>
    /// Move rigidbody using push
    /// </summary>
    void MoveRigidbody()
    {
        //set velocity
        Rb.velocity = pushForce;                    //push
        currentSpeed = Rb.velocity.magnitude;

        //remove push force
        Vector2 previousPush = pushForce;
        pushForce -= pushForce.normalized * (customDrag * Time.deltaTime);

        //clamp it
        if (previousPush.x >= 0 && pushForce.x < 0 || previousPush.x <= 0 && pushForce.x > 0)
            pushForce.x = 0;
        if (previousPush.y >= 0 && pushForce.y < 0 || previousPush.y <= 0 && pushForce.y > 0)
            pushForce.y = 0;
    }

    #region IDamageable

    /// <summary>
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    public virtual void GetDamage(float damage, Vector2 hitPosition = default)
    {
        if (alreadyDead)
            return;

        health -= damage;

        //call event
        onGetDamage?.Invoke();

        //check if dead
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Call it when health reach 0
    /// </summary>
    public virtual void Die()
    {
        alreadyDead = true;

        //call event
        onDie?.Invoke();

        //remove every collider if necessary
        if(removeColliderOnDeath)
        {
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
                col.enabled = false;
        }

        //damage in area too
        if (doAreaDamage && radiusAreaDamage > 0)
            DamageInArea();

        //if necessary, destroy after few seconds
        if (destroyAfterSeconds)
            Destroy(gameObject, secondsBeforeDestroy);
    }

    /// <summary>
    /// Push back
    /// </summary>
    /// <param name="push"></param>
    /// <param name="resetPreviousPush"></param>
    public virtual void PushBack(Vector2 push, Vector2 hitPosition = default, bool resetPreviousPush = false)
    {
        //reset previous push or add new one to it
        if (resetPreviousPush)
            pushForce = push;
        else
            pushForce += push;
    }

    #endregion
}