using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float health = 100;

    [Header("Pick Weapon - use trigger or collision")]
    [SerializeField] bool useTrigger = true;

    public Rigidbody2D Rb { get; private set; }
    public PlayerInput playerInput { get; private set; }
    public Vector2 DirectionPlayer { get; set; }

    //list of picked weapons
    List<WeaponBASE> weapons = new List<WeaponBASE>();
    int currentWeapon = 0;

    void Awake()
    {
        //get references
        Rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (useTrigger == false)
            return;

        //if hit weapon, add to the list
        WeaponBASE weapon = collision.GetComponentInParent<WeaponBASE>();
        if(weapon)
        {
            weapons.Add(weapon);
            weapon.PickWeapon(this);    //pick
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger)
            return;

        //if hit weapon, add to the list
        WeaponBASE weapon = collision.gameObject.GetComponentInParent<WeaponBASE>();
        if (weapon)
        {
            weapons.Add(weapon);
            weapon.PickWeapon(this);    //pick
        }
    }
}
