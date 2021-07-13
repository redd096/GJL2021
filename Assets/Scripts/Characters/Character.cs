using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Character")]
    [SerializeField] float health = 100;
    [SerializeField] float customDrag = 30;
    
    [Header("DEBUG")]
    [ReadOnly] public Vector2 DirectionAim;
    public WeaponBASE CurrentWeapon;
    [ReadOnly] public Vector2 MovementInput;
    [ReadOnly] [SerializeField] Vector2 pushForce;
    [ReadOnly] [SerializeField] float currentSpeed;

    public Rigidbody2D Rb { get; private set; }

    protected virtual void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();

        //if there is a weapon by inspector, set it
        CurrentWeapon?.PickWeapon(this);
    }

    protected virtual void Update()
    {
        //move rigidbody
        MoveCharacter();
    }

    /// <summary>
    /// Move character by velocity
    /// </summary>
    /// <param name="velocity"></param>
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
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    public void GetDamage(float damage)
    {
        health -= damage;

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
        //destroy character
        Destroy(gameObject);
    }

    /// <summary>
    /// Push back character
    /// </summary>
    /// <param name="push"></param>
    public void PushBack(Vector2 push)
    {
        pushForce += push;
    }
}
