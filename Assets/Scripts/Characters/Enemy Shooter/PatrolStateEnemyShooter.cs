using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PatrolStateEnemyShooter : StateMachineBehaviour
{
    [Header("Patrol Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float approxReachPosition = 0.1f;

    [Header("Patrol Rules (stop at every node or when finish path")]
    [SerializeField] bool alwaysInMovement = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] bool stopAtEveryNode = false;
    [CanShow("alwaysInMovement", NOT = true)] [SerializeField] float timeToWait = 1;

    Enemy enemy;
    List<Node> path;
    float timerBeforeMove;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //SULL'ENEMY CONTROLLER ci saranno le variabili per sapere cosa droppa (con percentuale) e quanta vita ha (vita già c'è)
        //per il drop può droppare un solo oggetto, preso magari da una lista con percentuale
        //oppure può droppare da liste di drop?
        //oppure semplicemente viene generata una percentuale per ogni drop, quindi può essere che droppa tutto come può essere che ne droppa solo alcuni o nessuno?

        //lo shooter avrà:
        //normalmente si muove in giro random (o verso il player?)
        //quando il player entra nella linea di tiro, cambia stato e inizia a sparargli (si muove pure mentre spara??)
        //p.s. lo shooter avrà un delay tra uno sparo e l'altro + una variabile WEAPON e quindi ci sarà anchje il suo fire rate

        //il charger avrà:
        //normalmente si muove in giro random (o verso il player?)
        //quando il player entra nella linea di tiro, cambia stato e inizia a CARICARE
        //quando finisce di caricare, cambia ancora stato e inizia a CORRERE verso il player (o ultima posizione nota?)
        //quando sbatte contro qualcosa (player o muri), cambia ancora stato e resta in IDLE per qualche secondo
        //poi tornerà allo stato normale, che passerà subito al caricare se il nemico è ancora nella linea di tiro
        //p.s. il charger avrà 2 variabili quando colpisce il player: danno e knockback
        //p.p.s. il charger non può essere danneggiato da davanti perché ha uno scudo
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //if not path, find new one
        if (path == null || path.Count <= 0)
        {
            path = enemy.GetPath();
            return;
        }

        //move if there is no timer to wait
        if(Time.time > timerBeforeMove)
            Movement();


    }

    #region private API

    void Movement()
    {
        //if reached position, remove node
        if(Vector2.Distance(enemy.transform.position, path[0].worldPosition) <= approxReachPosition)
        {
            path.RemoveAt(0);

            //if need to stop
            if(alwaysInMovement == false)
            {
                //stop at every node or when finish path
                if (stopAtEveryNode || path.Count <= 0)
                    timerBeforeMove = Time.time + timeToWait;
            }

            return;
        }

        //else move to next node in the path
        enemy.MoveCharacter((path[0].worldPosition - enemy.transform.position).normalized, speed);
    }

    void CheckPlayerIsFound()
    {
        //if(Physics2D.Raycast(enemy.transform.position))
    }

    #endregion
}
