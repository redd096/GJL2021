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
    [ReadOnly] [SerializeField] Transform pointPatrol;
    [ReadOnly] public Player Target;
    [ReadOnly] public Vector3 LastTargetPosition;

    Animator stateMachine;

    //event animations
    public System.Action onNextState { get; set; }
    public System.Action onBackToPatrolState { get; set; }

    protected virtual void OnDrawGizmos()
    {
        //draw patrol area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointPatrol != null ? pointPatrol.position : transform.position, radiusPatrol);
    }

    protected override void Awake()
    {
        base.Awake();

        //get references
        stateMachine = GetComponent<Animator>();

        //instantiate static point for patrol
        pointPatrol = new GameObject("Patrol Point: " + name).transform;
        pointPatrol.position = transform.position;

        //TODO
        //SULL'ENEMY CONTROLLER ci saranno le variabili per sapere cosa droppa (con percentuale) e quanta vita ha (vita già c'è)
        //per il drop può droppare un solo oggetto, preso magari da una lista con percentuale
        //oppure può droppare da liste di drop?
        //oppure semplicemente viene generata una percentuale per ogni drop, quindi può essere che droppa tutto come può essere che ne droppa solo alcuni o nessuno?
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
    public List<Node> GetPath()
    {
        //get random point in patrol area
        Vector3 randomPoint = pointPatrol.position + Random.insideUnitSphere * radiusPatrol;

        //find path
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
    public bool CheckTargetStillInVision()
    {
        if (Target == null)
            return false;

        //save last target position
        LastTargetPosition = Target.transform.position;

        //if there are not obstacles between enemy and player, return true
        if (Physics2D.Linecast(transform.position, Target.transform.position, layerBlockSight) == false)
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

    #endregion
}
