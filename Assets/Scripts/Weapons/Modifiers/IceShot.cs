using UnityEngine;

public class IceShot : MonoBehaviour
{
    [Header("Ice")]
    [SerializeField] float duration = 2;

    Bullet bullet;

    void OnEnable()
    {
        //get references
        bullet = GetComponent<Bullet>();

        //add events
        if(bullet)
        {
            bullet.onHit += OnHit;
        }
    }

    void OnDisable()
    {
        //remove events
        if (bullet)
        {
            bullet.onHit -= OnHit;
        }
    }

    void OnHit(GameObject hit)
    {
        //add frozen on hit and initialize
        if (hit.GetComponentInParent<IGetModifiers>() != null)
        {
            hit.AddComponent<FrozenModifier>().Init(duration);
        }
    }
}
