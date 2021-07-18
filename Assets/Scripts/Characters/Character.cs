using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Character")]
    [SerializeField] protected float health = 100;
    [SerializeField] float customDrag = 30;
    [SerializeField] protected WeaponBASE weaponPrefab = default;

    [Header("DEBUG")]
    [ReadOnly] public float MaxHealth = 0;
    [ReadOnly] public Vector2 DirectionInput;               //when character moves, set it with only direction (used to know last movement direction)
    [ReadOnly] public Vector2 DirectionAim;                 //when character aim, set it (used to know where to shoot for example)
    [ReadOnly] public Vector2 AimPositionNotNormalized;     //when character aim, set it without normalize (used to set crosshair on screen)
    [ReadOnly] public WeaponBASE CurrentWeapon;             //current equipped weapon
    [ReadOnly] [SerializeField] Vector2 MovementInput;      //when character moves, set it as direction * speed (used to move character, will be reset in every frame)
    [ReadOnly] [SerializeField] Vector2 pushForce;          //used to push character (push by recoil, knockback, dash, etc...), will be decreased by customDrag in every frame
    [ReadOnly] [SerializeField] float currentSpeed;         //speed from MovementInput and pushForce

    public Rigidbody2D Rb { get; private set; }
    Shield shield;
    protected Animator stateMachine;

    bool alreadyDead;

    //animation events
    public System.Action onGetDamage { get; set; }
    public System.Action onDie { get; set; }

    protected virtual void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();
        shield = GetComponentInChildren<Shield>();
        stateMachine = GetComponent<Animator>();

        //max health
        MaxHealth = health;
    }

    protected virtual void Update()
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

    #region public API

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

    /// <summary>
    /// Aim to set character's direction
    /// </summary>
    /// <param name="aim"></param>
    public void AimWithCharacter(Vector2 aim)
    {
        //save last input aim + set direction aim
        AimPositionNotNormalized = aim;
        DirectionAim = aim.normalized;
    }

    /// <summary>
    /// Instantiate and Equip Weapon
    /// </summary>
    /// <param name="weaponPrefab"></param>
    public virtual void PickWeapon(WeaponBASE prefab)
    {
        //instantiate and equip weapon
        CurrentWeapon = prefab != null ? Instantiate(prefab) : null;
        CurrentWeapon?.PickWeapon(this);
    }

    /// <summary>
    /// Drop and Destroy Weapon
    /// </summary>
    public virtual void DropWeapon()
    {
        if (CurrentWeapon == null)
            return;

        //drop and destroy weapon
        CurrentWeapon.DropWeapon();
        CurrentWeapon = null;
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

        //do nothing if hit shield (at least ignoreShield is true)
        if (shield && shield.HitShield(hitPosition) && ignoreShield == false)
            return;

        //set health and update UI
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
        if (alreadyDead)
            return;

        alreadyDead = true;

        //call event
        onDie?.Invoke();

        //destroy object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get health
    /// </summary>
    /// <param name="healthGiven"></param>
    public virtual void GetHealth(float healthGiven)
    {
        if (alreadyDead)
            return;

        //add health
        health += healthGiven;
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
