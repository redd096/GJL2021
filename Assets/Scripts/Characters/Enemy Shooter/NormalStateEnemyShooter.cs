using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStateEnemyShooter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

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
}
