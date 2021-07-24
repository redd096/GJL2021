using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Enemy : Character
{
    [Header("Patrol Area")]
    [SerializeField] float radiusPatrol = 2;

    [Header("Block Enemy Sight")]
    [SerializeField] LayerMask layerBlockSight = default;

    [Header("When Hit Player")]
    [SerializeField] bool knockBackPlayerOnHit = true;
    [CanShow("knockBackPlayerOnHit")] [SerializeField] float pushBackOnHitEnemy = 10;
    [CanShow("knockBackPlayerOnHit")] [SerializeField] float damageOnHitEnemy = 10;

    [Header("DEBUG ENEMY")]
    [ReadOnly] [SerializeField] protected Transform pointPatrol;
    [ReadOnly] public Player Target;
    [ReadOnly] public Vector3 LastTargetPosition;

    //event animations
    public System.Action onNextState { get; set; }
    public System.Action<bool> onTargetSetted { get; set; }

    protected virtual void OnDrawGizmos()
    {
        //draw patrol area
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pointPatrol != null ? pointPatrol.position : transform.position, radiusPatrol);

        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y) + DirectionAim * 0.5f, 0.3f);
    }

    protected override void Awake()
    {
        base.Awake();

        //instantiate static point for patrol
        pointPatrol = new GameObject("Patrol Point: " + name).transform;
        pointPatrol.position = transform.position;

        //if there is a weapon by inspector, set it
        PickWeapon(WeaponPrefab);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (knockBackPlayerOnHit == false)
            return;

        //if hit player
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player)
        {
            //pushback and damage
            player.PushBack((collision.transform.position - transform.position).normalized * pushBackOnHitEnemy);
            player.GetDamage(damageOnHitEnemy);
        }
    }

    #region public statemachine API

    /// <summary>
    /// Return path to a random point in patrol area
    /// </summary>
    /// <returns></returns>
    public List<Node> GetPatrolPath()
    {
        //get random point in patrol area
        Vector3 randomPoint = new Vector2(pointPatrol.position.x, pointPatrol.position.y) + Random.insideUnitCircle * radiusPatrol;
        Debug.DrawLine(transform.position, randomPoint, Color.blue, 0.5f);

        return AStar.instance.FindPath(transform.position, randomPoint);
    }

    /// <summary>
    /// Set if can knockback player on hit
    /// </summary>
    /// <param name="canKnockback"></param>
    public void SetKnobackPlayerOnHit(bool canKnockback)
    {
        knockBackPlayerOnHit = canKnockback;
    }

    /// <summary>
    /// Check if there are not obstacles between enemy and player
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerIsFound()
    {
        foreach (Player player in GameManager.instance.levelManager.Players)
        {
            //if there are not obstacles between enemy and player, set it as target
            if (Physics2D.Linecast(transform.position, player.transform.position, layerBlockSight) == false)
            {
                Target = player;

                return true;
            }

            Debug.DrawLine(transform.position, player.transform.position, Color.green);
        }

        return false;
    }

    /// <summary>
    /// Check target is still in vision area
    /// </summary>
    /// <returns></returns>
    public bool CheckTargetStillInVision(bool canSeeThroughWalls)
    {
        if (Target == null)
            return false;

        //save last target position
        LastTargetPosition = Target.transform.position;

        //if can see through walls OR there are not obstacles between enemy and player, return true
        if (canSeeThroughWalls || Physics2D.Linecast(transform.position, Target.transform.position, layerBlockSight) == false)
        {
            Debug.DrawLine(transform.position, Target.transform.position, Color.red);
            return true;
        }

        //else remove target
        Target = null;

        return false;
    }

    /// <summary>
    /// Set new state for state machine
    /// </summary>
    /// <param name="state"></param>
    public void SetState(string state)
    {
        //set new state
        stateMachine.SetTrigger(state);
    }

    /// <summary>
    /// Set in state machine if Target is setted
    /// </summary>
    /// <param name="isSetted"></param>
    public void SetTargetSetted(bool isSetted)
    {
        stateMachine.SetBool("Target Setted", isSetted);
    }

    /// <summary>
    /// Move patrol position to new transform position
    /// </summary>
    public void UpdatePatrolPosition()
    {
        pointPatrol.position = transform.position;
    }

    #endregion
}
