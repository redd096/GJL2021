using UnityEngine;
using redd096;

public class Character : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] float health = 100;
    
    [Header("DEBUG")]
    [ReadOnly] public Vector2 DirectionAim;
    public WeaponBASE CurrentWeapon;

    public Rigidbody2D Rb { get; private set; }

    protected virtual void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();

        //if there is a weapon by inspector, set it
        CurrentWeapon?.PickWeapon(this);
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

    protected virtual void Die()
    {
        //destroy character
        Destroy(gameObject);
    }
}
