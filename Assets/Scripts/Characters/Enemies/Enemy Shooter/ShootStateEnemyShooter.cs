using UnityEngine;

public class ShootStateEnemyShooter : StateMachineBehaviour
{
    [Header("Shoot")]
    [SerializeField] float delayBeforeFirstShot = 1;
    [SerializeField] float delayBetweenShots = 1;

    Enemy enemy;
    float timerBeforeShoot;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set delay before first shot
        timerBeforeShoot = Time.time + delayBeforeFirstShot;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //look at target
        LookAtTarget();

        //shoot if there is no timer to wait
        if (Time.time > timerBeforeShoot)
            Shoot();

        //check target still in vision area
        CheckTargetStillInVision();
    }

    #region private API

    void LookAtTarget()
    {
        //aim at target
        enemy.DirectionAim = (enemy.Target.transform.position - enemy.transform.position).normalized;
    }

    void Shoot()
    {
        //set delay between shots
        timerBeforeShoot = Time.time + delayBetweenShots;

        //shoot but not automatic
        enemy.CurrentWeapon?.PressAttack();
        enemy.CurrentWeapon?.ReleaseAttack();
    }

    #endregion

    void CheckTargetStillInVision()
    {
        //if player is found, change state
        if (enemy.CheckTargetStillInVision() == false)
            enemy.SetState("Target Lost");
    }
}
