using UnityEngine;
using redd096;
using DG.Tweening;

public class ShootStateEnemyBoss : StateMachineBehaviour
{
    [Header("Duration State")]
    [SerializeField] float durationState = 6;
    [SerializeField] float durationMovementInsideOfScreen = 2;

    [Header("Noise Accuracy")]
    [SerializeField] bool overwriteNoiseAccuracyFirstShoot = true;
    [CanShow("overwriteNoiseAccuracyFirstShoot")] [SerializeField] float noiseAccuracyFirstShoot = 30;

    [Header("Delays")]
    [SerializeField] float delayFirstShoot = 4;
    [SerializeField] float delayBetweenShoots = 1;

    [Header("Event Animation On Enter State")]
    [SerializeField] bool callShootStateEvent = true;

    EnemyBoss enemy;
    float timeFinishState;
    float timeNextShoot;
    int currentShoot;

    Pooling<Bullet> poolShoots = new Pooling<Bullet>();

    //on enter, move inside of screen
    //look at target
    //every tot seconds, shoots
    //after few seconds, call "Next State"
    //
    //when enter state call also enemy.onShootState event

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<EnemyBoss>();

        //set vars
        timeFinishState = Time.time + durationState;
        timeNextShoot = Time.time + delayFirstShoot;

        //reset vars
        currentShoot = 0;

        //call state event
        if (callShootStateEvent)
            enemy.onShootState?.Invoke();

        //start moving inside of screen
        MoveInsideOfScreen();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //look at nearest player
        LookAtPlayer();

        //wait delay, then shoot
        if (Time.time > timeNextShoot)
        {
            if (currentShoot <= 0)
                FirstShoot();
            else
                Shoot();
        }

        //wait timer, then finish state
        if (Time.time > timeFinishState)
            FinishState();
    }

    void MoveInsideOfScreen()
    {
        //center is point patrol or center of map
        Vector2 center = enemy.PointPatrol != null ? new Vector2(enemy.PointPatrol.position.x, enemy.PointPatrol.position.y) : Vector2.zero;
        Vector2 size = enemy.RangeWhereToStop /2;

        //calculate random point
        float x = Random.Range(center.x - size.x, center.x + size.x);
        float y = Random.Range(center.y - size.y, center.y + size.y);

        //move inside of screen
        enemy.transform.DOMove(new Vector3(x, y, 0), durationMovementInsideOfScreen);
    }

    void LookAtPlayer()
    {
        //find nearest enemy
        enemy.FindNearestEnemy();

        //aim at target
        if (enemy.Target)
        {
            enemy.AimWithCharacter(enemy.Target.transform.position - enemy.transform.position);
        }
    }

    void FirstShoot()
    {
        //overwrite noise accuracy if necessary
        if (overwriteNoiseAccuracyFirstShoot && enemy.CurrentWeapon is WeaponRange)
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
        //set delay between shots and increase counter
        timeNextShoot = Time.time + delayBetweenShoots;
        currentShoot++;

        //if reach limit, restart
        if (currentShoot >= enemy.BarrelsForEveryAttack.Length)
            currentShoot = 0;

        //if there are barrels in array for this shoot, change barrels
        if (enemy.BarrelsForEveryAttack.Length > currentShoot && enemy.CurrentWeapon is WeaponRange)
        {
            WeaponRange enemyWeapon = enemy.CurrentWeapon as WeaponRange;
            enemyWeapon.barrels = enemy.BarrelsForEveryAttack[currentShoot].barrels;
        }

        //shoot (not automatic)
        enemy.CurrentWeapon?.PressAttack();
        enemy.CurrentWeapon?.ReleaseAttack();
    }

    void FinishState()
    {
        //change to next state
        enemy.SetState("Next State");
    }
}
