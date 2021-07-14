using UnityEngine;
using redd096;

public class ShootStateEnemyShooter : StateMachineBehaviour
{
    [Header("Noise Accuracy")]
    [SerializeField] bool overwriteNoiseAccuracyFirstShoot = true;
    [CanShow("overwriteNoiseAccuracyFirstShoot")] [SerializeField] float noiseAccuracyFirstShoot = 30;

    [Header("Delay Shoots")]
    [SerializeField] float delayBeforeFirstShot = 1;
    [SerializeField] float delayBetweenShots = 1;

    Enemy enemy;
    float timerBeforeShoot;
    bool firstShoot;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<Enemy>();

        //set delay before first shot
        timerBeforeShoot = Time.time + delayBeforeFirstShot;

        //reset vars
        firstShoot = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //look at target
        LookAtTarget();

        //shoot if there is no timer to wait
        if (Time.time > timerBeforeShoot)
        {
            if (firstShoot)
                FirstShoot();
            else
                Shoot();
        }

        //check target still in vision area
        CheckTargetStillInVision();
    }

    #region private API

    void LookAtTarget()
    {
        if (enemy.Target == null)
            return;

        //aim at target
        enemy.DirectionAim = (enemy.Target.transform.position - enemy.transform.position).normalized;
    }

    void FirstShoot()
    {
        firstShoot = false;

        //overwrite noise accuracy if necessary
        if(overwriteNoiseAccuracyFirstShoot && enemy.CurrentWeapon is WeaponRange)
        {
            //save previous noise accuracy
            WeaponRange enemyWeapon = enemy.CurrentWeapon as WeaponRange;
            float defaultNoiseAccuracy = enemyWeapon.NoiseAccuracy;

            //shoot with new noise
            enemyWeapon.NoiseAccuracy = noiseAccuracyFirstShoot;
            Shoot();

            //reset noise
            enemyWeapon.NoiseAccuracy = defaultNoiseAccuracy;

            return;
        }

        //else normal shoot
        Shoot();
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
