using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Character")]
    [SerializeField] float health = 100;
    [SerializeField] float customDrag = 30;

    [Header("DEBUG")]
    [ReadOnly] public Vector2 DirectionInput;               //when character moves, set it with only direction (used to know last movement direction)
    [ReadOnly] public Vector2 DirectionAim;                 //when character aim, set it (used to know where to shoot for example)
    public WeaponBASE CurrentWeapon;                        //current equipped weapon
    [ReadOnly] [SerializeField] Vector2 MovementInput;      //when character moves, set it as direction * speed (used to move character, will be reset in every frame)
    [ReadOnly] [SerializeField] Vector2 pushForce;          //used to push character (push by recoil, knockback, dash, etc...), will be decreased by customDrag in every frame
    [ReadOnly] [SerializeField] float currentSpeed;         //speed from MovementInput and pushForce

    public Rigidbody2D Rb { get; private set; }
    Shield shield;

    bool alreadyDead;

    //animation events
    public System.Action onGetDamage { get; set; }
    public System.Action onDie { get; set; }

    protected virtual void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();
        shield = GetComponentInChildren<Shield>();

        //if there is a weapon by inspector, set it
        CurrentWeapon?.PickWeapon(this);
    }

    void Update()
    {
        //move rigidbody
        MoveCharacter();
    }

    /// <summary>
    /// Move character using movement and push
    /// </summary>
    void MoveCharacter()
    {
        //set velocity
        Rb.velocity = MovementInput + pushForce;    //input + push
        currentSpeed = Rb.velocity.magnitude;

        //reset movement input
        MovementInput = Vector2.zero;

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
    /// Set character movement
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void MoveCharacter(Vector2 direction, float speed)
    {
        //save last input direction + set movement
        DirectionInput = direction;
        MovementInput = direction * speed;
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

        //do nothing if hit shield
        if (shield && shield.HitShield(hitPosition))
            return;

        health -= damage;

        //call event
        onGetDamage?.Invoke();

        //check if dead
        if(health <= 0)
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

        //destroy object
        Destroy(gameObject);
    }

    /// <summary>
    /// Push back character
    /// </summary>
    /// <param name="push"></param>
    /// <param name="resetPreviousPush"></param>
    public virtual void PushBack(Vector2 push, Vector2 hitPosition = default, bool resetPreviousPush = false)
    {
        //do nothing if hit shield
        if (shield && shield.HitShield(hitPosition))
            return;

        //reset previous push or add new one to it
        if (resetPreviousPush)
            pushForce = push;
        else
            pushForce += push;
    }

    #endregion
}
