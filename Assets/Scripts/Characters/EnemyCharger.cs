using UnityEngine;

public class EnemyCharger : Enemy
{
    [Header("Shield")]
    [SerializeField] float angleDefense = 90;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //draw shield angle
        Gizmos.color = Color.green;
        Vector3 up = Quaternion.AngleAxis(angleDefense, transform.localScale.x > 0 ? Vector3.forward : Vector3.back) * Vector3.right;
        Vector3 down = Quaternion.AngleAxis(-angleDefense, transform.localScale.x > 0 ? Vector3.forward : Vector3.back) * Vector3.right;
        Gizmos.DrawLine(transform.position, transform.position + up);
        Gizmos.DrawLine(transform.position, transform.position + down);
    }

    public override void GetDamage(float damage, Vector2 hitPosition = default)
    {
        //do nothing if hit shield
        if (HitShield(hitPosition))
            return;

        base.GetDamage(damage, hitPosition);
    }

    public override void PushBack(Vector2 push, Vector2 hitPosition = default, bool resetPreviousPush = false)
    {
        //do nothing if hit shield
        if (HitShield(hitPosition))
            return;

        base.PushBack(push, hitPosition, resetPreviousPush);
    }

    bool HitShield(Vector2 hitPosition)
    {
        //get angle (change if enemy is looking right or left)
        Vector2 direction = (hitPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        float angle = Vector2.SignedAngle(transform.localScale.x > 0 ? Vector2.right : Vector2.left, direction);

        //check if inside shield angle
        if (angle > -angleDefense && angle < angleDefense)
        {
            return true;
        }

        return false;
    }
}
