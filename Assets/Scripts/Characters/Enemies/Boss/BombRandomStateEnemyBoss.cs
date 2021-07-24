﻿using System.Collections.Generic;
using UnityEngine;
using redd096;

public class BombRandomStateEnemyBoss : StateMachineBehaviour
{
    [Header("Duration State")]
    [SerializeField] float durationState = 20;
    [SerializeField] float durationMovementOutOfScreen = 2;

    [Header("Bomb State")]
    [SerializeField] float damage = 30;
    [SerializeField] float durationBombExplosion = 1.5f;
    [SerializeField] bool bombOnPlayer = true;

    [Header("Delays")]
    [SerializeField] float delayFirstBomb = 4;
    [SerializeField] int minNumberRandomBombs = 4;
    [SerializeField] int maxNumberRandomBombs = 7;
    [SerializeField] float delayBetweenBombs = 1;

    [Header("Event Animation On Enter State")]
    [SerializeField] bool callBombStateEvent = true;

    EnemyBoss enemy;
    float timeFinishState;
    float timeNextBomb;

    Pooling<GameObject> poolBombCircle = new Pooling<GameObject>();
    Pooling<Bullet> poolBombs = new Pooling<Bullet>();
    Coroutine movementCoroutine;
    List<Coroutine> bombCoroutines = new List<Coroutine>();

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
            GetPositionBombs();

        //wait timer, then finish state
        if (Time.time > timeFinishState)
            FinishState();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        //be sure to stop coroutines
        if (movementCoroutine != null)
            enemy.StopCoroutine(movementCoroutine);

        foreach(Coroutine bombCoroutine in bombCoroutines)      //every coroutine in the list
            if (bombCoroutine != null)
                enemy.StopCoroutine(bombCoroutine);

        bombCoroutines.Clear();                                 //and clear the list
    }

    #region private API

    void MoveOutOfScreen()
    {
        //move out of screen
        movementCoroutine = enemy.StartCoroutine(enemy.MovementCoroutine(enemy.transform, enemy.PositionOutScreen, durationMovementOutOfScreen));
    }

    void GetPositionBombs()
    {
        //set new delay
        timeNextBomb = Time.time + delayBetweenBombs;

        //find nearest enemy and spawn bomb on it
        enemy.FindNearestEnemy();
        if(enemy.Target && bombOnPlayer)
        {
            SpawnBomb(enemy.Target.transform.position);
        }

        //center is point patrol or center of map
        Vector2 center = enemy.PointPatrol != null ? new Vector2(enemy.PointPatrol.position.x, enemy.PointPatrol.position.y) : Vector2.zero;
        Vector2 size = enemy.RangeBossAttack / 2;

        //get random number of bombs
        int randomBombsNumber = Random.Range(minNumberRandomBombs, maxNumberRandomBombs);

        //then spawn bombs in random points
        for (int i = 0; i < randomBombsNumber; i++)
        {
            //calculate random point
            float x = Random.Range(center.x - size.x, center.x + size.x);
            float y = Random.Range(center.y - size.y, center.y + size.y);

            SpawnBomb(new Vector3(x, y, 0));
        }
    }

    void SpawnBomb(Vector3 position)
    {
        //instantiate bomb circle and bomb
        GameObject bombCircle = poolBombCircle.Instantiate(enemy.BombCircle);
        Bullet bomb = poolBombs.Instantiate(enemy.BombPrefab);

        //circle and bomb position
        bombCircle.transform.position = position;
        bomb.transform.position = new Vector3(position.x, enemy.PositionOutScreen.y, 0);    //move bomb from out of screen to position explosion

        //set delay autodestruction and Init
        bomb.delayAutodestruction = durationBombExplosion;
        bomb.Init(null, Vector2.zero, damage, 0);

        //move bomb to destination
        bombCoroutines.Add(enemy.StartCoroutine(enemy.MovementCoroutine(bomb.transform, position, durationBombExplosion)));
    }

    #endregion

    void FinishState()
    {
        //change to next state
        enemy.SetState("Next State");
    }
}
