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

    [Header("Shoot (change state after shoot or when lose target?)")]
    [SerializeField] float durationShoot = 0.3f;
    [SerializeField] bool changeStateAfterShoot = true;

    Enemy enemy;
    float timerBeforeShoot;
    bool firstShoot;
    Coroutine stopShootCoroutine;

    //Look at Target and shoot
    //when player is lost, call "Target Lost"   (only if setted to NOT change state after shoot)
    //when finish shoot, call "Next State"      (only if setted to change state after shoot)
    //
    //continue look at target last position
    //delay before first shoot. Can also overwrite noise weapon for first shoot
    //delay between every shoot.
    //can be changed state after shoot or when lose target
    //can be set duration shoot, for automatic weapon or to delay the changing of state after shoot

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
        enemy.CheckTargetStillInVision(canSeeThroughWalls: false);
        LookAtTargetLastPosition();

        //shoot if there is no timer to wait
        if (Time.time > timerBeforeShoot)
        {
            if (firstShoot)
                FirstShoot();
            else
                Shoot();
        }

        //check target still in vision area or finish state
        CheckIfFinishState();
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

    void LookAtTargetLastPosition()
    {
        //aim at target
        enemy.AimWithCharacter(enemy.LastTargetPosition - enemy.transform.position);
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

            //call function finish shoot
            OnFinishShoot();
        }
    }

    #endregion

    void OnFinishShoot()
    {
        if (changeStateAfterShoot == false)
            return;

        //change state if necessary after shot
        enemy.SetState("Next State");
    }

    void CheckIfFinishState()
    {
        //do only if not change at every shot
        if (changeStateAfterShoot)
            return;

        //if player is lost, change state
        if (enemy.Target == null)
            enemy.SetState("Target Lost");
    }
}
