using UnityEngine;
using redd096;

[System.Serializable]
public struct BossBarrelsStruct
{
    public Transform[] barrels;
}

public class EnemyBoss : Enemy
{
    [Header("Boss Bomb State (cyan)")]
    public Vector2 PositionOutScreen = Vector2.up * 10;
    public Vector2 RangeBossAttack = Vector2.one;
    public Bullet BombPrefab = default;
    public GameObject BombCircle = default;

    [Header("Boss Shoot State (green)")]
    public Vector2 RangeWhereToStop = Vector2.one * 2;
    public BossBarrelsStruct[] BarrelsForEveryAttack = default;

    public float Health => health;
    public Transform PointPatrol => pointPatrol;

    public System.Action onSpawnState { get; set; }
    public System.Action onBombState { get; set; }
    public System.Action onShootState { get; set; }
    public System.Action onIdleState { get; set; }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //line to position out screen
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, PositionOutScreen);

        //bomb area
        Gizmos.DrawWireCube(pointPatrol != null ? pointPatrol.position : transform.position, RangeBossAttack);

        //range shoot
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pointPatrol != null ? pointPatrol.position : transform.position, RangeWhereToStop);
    }

    protected override void Update()
    {
        //don't push character
        Rb.velocity = Vector2.zero;
    }

    public override void PickWeapon(WeaponBASE prefab)
    {
        //override, to equip current weapon instead of instantiate prefab
        CurrentWeapon?.PickWeapon(this);
    }

    public void FindNearestEnemy()
    {
        Player nearestPlayer = null;
        float distance = Mathf.Infinity;

        //fine nearest
        foreach (Player player in GameManager.instance.levelManager.Players)
        {
            float newDistance = Vector2.Distance(transform.position, player.transform.position);
            if(newDistance < distance)
            {
                nearestPlayer = player;
                distance = newDistance;
            }
        }

        //set target
        Target = nearestPlayer;
    }
}
