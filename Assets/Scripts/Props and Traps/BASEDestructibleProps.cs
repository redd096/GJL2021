using System.Collections.Generic;
using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public class BASEDestructibleProps : MonoBehaviour, IDamageable
{
    [Header("Prop")]
    [SerializeField] bool isDestructible = true;
    [CanShow("isDestructible")] [SerializeField] float health = 100;
    [SerializeField] bool removeColliderOnDeath = true;

    [Header("Area Damage on Destroy")]
    [SerializeField] bool doAreaDamage = false;
    [CanShow("doAreaDamage")] [SerializeField] bool ignoreShield = false;
    [CanShow("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 1;     //damage characters in radius area
    [CanShow("doAreaDamage")] [SerializeField] float damage = 10;
    [CanShow("doAreaDamage")] [SerializeField] float knockBack = 0;

    [Header("Push")]
    [SerializeField] bool canBePushed = false;
    [CanShow("canBePushed")] [SerializeField] float customDrag = 30;

    [Header("Remove after few seconds is Dead")]
    [SerializeField] bool removeAfterSecondsIsDead = false;
    [CanShow("removeAfterSecondsIsDead")] [SerializeField] float secondsBeforeRemove = 10;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 pushForce;          //used to push character (push by recoil, knockback, dash, etc...), will be decreased by customDrag in every frame
    [ReadOnly] [SerializeField] float currentSpeed;         //speed from MovementInput and pushForce

    public Rigidbody2D Rb { get; private set; }

    protected bool alreadyDead;

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

            //update grid when move
            if (Rb.velocity.magnitude > 0)
                AStar.instance.UpdateGrid();
        }
    }

    #region private API

    /// <summary>
    /// On destroy, damage in area
    /// </summary>
    void DamageInArea()
    {
        //be sure to not hit again the same
        List<IDamageable> damageables = new List<IDamageable>();

        //find every object damageable in area
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
        {
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null && damageables.Contains(damageable) == false)
            {
                //add only one time in the list, and do damage and knockback
                damageables.Add(damageable);
                damageable.GetDamage(damage, ignoreShield, transform.position);
                damageable.PushBack((col.transform.position - transform.position).normalized * knockBack, transform.position);
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

    /// <summary>
    /// Remove collider
    /// </summary>
    void RemoveCollider()
    {
        //remove every collider
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        //update grid
        AStar.instance.UpdateGrid();
    }

    #endregion

    #region IDamageable

    /// <summary>
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    public virtual void GetDamage(float damage, bool ignoreShield = true, Vector2 hitPosition = default)
    {
        if (alreadyDead)
            return;

        //be sure is destructible
        if (isDestructible == false)
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
        if (alreadyDead)
            return;

        alreadyDead = true;

        //call event
        onDie?.Invoke();

        //remove every collider if necessary
        if(removeColliderOnDeath)
        {
            Invoke("RemoveCollider", 0.5f);
        }

        //damage in area too
        if (doAreaDamage && radiusAreaDamage > 0)
            DamageInArea();

        //if necessary, destroy after few seconds
        if (removeAfterSecondsIsDead)
            Destroy(gameObject, secondsBeforeRemove);
    }

    /// <summary>
    /// Get health
    /// </summary>
    /// <param name="healthGiven"></param>
    public void GetHealth(float healthGiven)
    {
        if (alreadyDead)
            return;

        //add health
        health += healthGiven;
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
