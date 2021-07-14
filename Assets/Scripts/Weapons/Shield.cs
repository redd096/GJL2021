using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] float angleDefense = 90;

    void OnDrawGizmos()
    {
        //draw shield angle
        Gizmos.color = Color.green;
        Vector3 up = Quaternion.AngleAxis(angleDefense, transform.localScale.x > 0 ? Vector3.forward : Vector3.back) * Vector3.right;
        Vector3 down = Quaternion.AngleAxis(-angleDefense, transform.localScale.x > 0 ? Vector3.forward : Vector3.back) * Vector3.right;
        Gizmos.DrawLine(transform.position, transform.position + up);
        Gizmos.DrawLine(transform.position, transform.position + down);
    }

    public bool HitShield(Vector2 hitPosition)
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
