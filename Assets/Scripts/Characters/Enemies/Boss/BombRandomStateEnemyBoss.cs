using UnityEngine;
using DG.Tweening;
using redd096;

public class BombRandomStateEnemyBoss : StateMachineBehaviour
{
    [Header("Duration State")]
    [SerializeField] float durationState = 20;
    [SerializeField] float durationMovementOutOfScreen = 2;

    [Header("Bomb State")]
    [SerializeField] float damage = 30;
    [SerializeField] float durationBombExplosion = 1.5f;

    [Header("Delays")]
    [SerializeField] float delayFirstBomb = 4;
    [SerializeField] float delayBetweenBombs = 1;

    [Header("Event Animation On Enter State")]
    [SerializeField] bool callBombStateEvent = true;

    EnemyBoss enemy;
    float timeFinishState;
    float timeNextBomb;

    Pooling<GameObject> poolBombCircle = new Pooling<GameObject>();
    Pooling<Bullet> poolBombs = new Pooling<Bullet>();

    //on enter, move out of screen
    //every tot seconds spawn bombs at random point inside area
    //after few seconds, call "Next State"
    //
    //when enter state call also enemy.onIdleState event

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //get references
        enemy = animator.GetComponent<EnemyBoss>();

        //set vars
        timeFinishState = Time.time + durationState;
        timeNextBomb = Time.time + delayFirstBomb;

        //call state event
        if (callBombStateEvent)
            enemy.onBombState?.Invoke();

        //start moving out of screen
        MoveOutOfScreen();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //wait delay, then spawn bomb
        if (Time.time > timeNextBomb)
            SpawnBomb();

        //wait timer, then finish state
        if (Time.time > timeFinishState)
            FinishState();
    }

    void MoveOutOfScreen()
    {
        //move out of screen
        enemy.transform.DOMove(enemy.PositionOutScreen, durationMovementOutOfScreen);
    }

    void SpawnBomb()
    {
        //set new delay
        timeNextBomb = Time.time + delayBetweenBombs;


        //center is point patrol or center of map
        Vector2 center = enemy.PointPatrol != null ? new Vector2(enemy.PointPatrol.position.x, enemy.PointPatrol.position.y) : Vector2.zero;
        Vector2 size = enemy.RangeBossAttack;

        //calculate random point
        float x = Random.Range(center.x - size.x, center.x + size.x);
        float y = Random.Range(center.y - size.y, center.y + size.y);

        //instantiate bomb circle
        GameObject bombCircle = poolBombCircle.Instantiate(enemy.BombCircle);
        bombCircle.transform.position = new Vector3(x, y, 0);

        //instantiate bomb out of screen but on same X as circle
        Bullet bomb = poolBombs.Instantiate(enemy.BombPrefab);
        bombCircle.transform.position = new Vector2(x, enemy.PositionOutScreen.y);

        //set delay autodestruction and Init
        bomb.delayAutodestruction = durationBombExplosion;
        bomb.Init(null, Vector2.zero, damage, 0);

        //move bomb to position explosion
        bomb.transform.DOMove(new Vector3(x, y, 0), durationBombExplosion);
    }

    void FinishState()
    {
        //change to next state
        enemy.SetState("Next State");
    }
}
