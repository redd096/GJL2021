using UnityEngine.InputSystem;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{
    [Header("When Hit an Enemy")]
    [SerializeField] float pushBackOnHitEnemy = 10;
    [SerializeField] float damageOnHitEnemy = 10;

    public PlayerInput playerInput { get; private set; }

    //animation events
    public System.Action onDash { get; set; }

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if hit an enemy (not enemy charger)
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy && enemy is EnemyCharger == false)
        {
            //pushback and damage
            PushBack((transform.position - collision.transform.position).normalized * pushBackOnHitEnemy);
            GetDamage(damageOnHitEnemy);
        }
    }
}
