using System.Collections;
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

    [Header("Shoot")]
    [SerializeField] float durationShoot = 0.3f;
    [SerializeField] bool changeStateAfterShot = true;

    Enemy enemy;
    float timerBeforeShoot;
    bool firstShoot;
    Coroutine stopShootCoroutine;

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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        //if coroutine is running, stop it and stop shooting
        if (stopShootCoroutine != null)
        {
            enemy.StopCoroutine(stopShootCoroutine);
            enemy.CurrentWeapon?.ReleaseAttack();
        }
    }

    #region private API

    void LookAtTarget()
    {
        if (enemy.Target == null)
            return;

        //aim at target
        enemy.AimWithCharacter(enemy.Target.transform.position - enemy.transform.position);
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

        //shoot
        enemy.CurrentWeapon?.PressAttack();

        //start coroutine
        if (stopShootCoroutine != null)
            enemy.StopCoroutine(stopShootCoroutine);

        stopShootCoroutine = enemy.StartCoroutine(StopShootCoroutine());
    }

    IEnumerator StopShootCoroutine()
    {
        //wait
        yield return new WaitForSeconds(durationShoot);

        //stop shoot
        if (enemy)
        {
            enemy.CurrentWeapon?.ReleaseAttack();

            if(changeStateAfterShot)
                enemy.SetState("Next State");
        }
    }

    #endregion

    void CheckTargetStillInVision()
    {
        //if player is found, change state
        if (enemy.CheckTargetStillInVision() == false)
            enemy.SetState("Target Lost");
    }
}
