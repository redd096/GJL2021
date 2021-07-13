using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Enemy : Character
{
    [Header("Patrol Area")]
    [SerializeField] float radiusPatrol = 2;

    [Header("DEBUG ENEMY")]
    [ReadOnly] [SerializeField] Transform pointPatrol;
    [ReadOnly] public Player Target;

    Animator stateMachine;

    void OnDrawGizmos()
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

        //SULL'ENEMY CONTROLLER ci saranno le variabili per sapere cosa droppa (con percentuale) e quanta vita ha (vita già c'è)
        //per il drop può droppare un solo oggetto, preso magari da una lista con percentuale
        //oppure può droppare da liste di drop?
        //oppure semplicemente viene generata una percentuale per ogni drop, quindi può essere che droppa tutto come può essere che ne droppa solo alcuni o nessuno?

        //il charger avrà:
        //V normalmente si muove in giro random (o verso il player?)
        //quando il player entra nella linea di tiro, cambia stato e inizia a CARICARE
        //quando finisce di caricare, cambia ancora stato e inizia a CORRERE verso il player (o ultima posizione nota?)
        //quando sbatte contro qualcosa (player o muri), cambia ancora stato e resta in IDLE per qualche secondo
        //poi tornerà allo stato normale, che passerà subito al caricare se il nemico è ancora nella linea di tiro
        //p.s. il charger avrà 2 variabili quando colpisce il player: danno e knockback
        //p.p.s. il charger non può essere danneggiato da davanti perché ha uno scudo
    }

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
    /// Check if there are not obstacles between enemy and player
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerIsFound()
    {
        foreach (Player player in GameManager.instance.levelManager.Players)
        {
            //if there are not obstacles between enemy and player, set it as target
            if (Physics2D.Linecast(transform.position, player.transform.position, CreateLayer.LayerOnly("Not Walkable")) == false)
            {
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
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
        //if there are not obstacles between enemy and player, return true
        if (Physics2D.Linecast(transform.position, Target.transform.position, CreateLayer.LayerOnly("Not Walkable")) == false)
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
}
