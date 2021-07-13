using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Enemy : Character
{
    [Header("Patrol Area")]
    [SerializeField] float radiusPatrol = 2;

    [Header("DEBUG ENEMY")]
    [ReadOnly] [SerializeField] Transform pointPatrol;

    void OnDrawGizmos()
    {
        //draw patrol area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointPatrol != null ? pointPatrol.position : transform.position, radiusPatrol);
    }

    protected override void Awake()
    {
        base.Awake();

        //instantiate static point for patrol
        pointPatrol = new GameObject("Patrol Point: " + name).transform;
        pointPatrol.position = transform.position;
    }

    /// <summary>
    /// Return path to a random point in patrol area
    /// </summary>
    /// <returns></returns>
    public List<Node> GetPath()
    {
        //get random point in patrol area
        Vector3 randomPoint = pointPatrol.position + Random.insideUnitSphere * radiusPatrol;

        //find path
        return AStar.instance.FindPath(transform.position, randomPoint);
    }
}
