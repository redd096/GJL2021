using UnityEngine;

public class IceShot : MonoBehaviour
{
    [Header("Ice")]
    [SerializeField] float duration = 2;
    [SerializeField] bool ignoreIfAlreadyFrozen = false;
    [SerializeField] bool ignoreShield = false;

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
        GetModifiersObject frozenObject = hit.GetComponentInParent<GetModifiersObject>();

        //if can get modifiers and can apply it
        if (frozenObject != null && frozenObject.CanApplyModifiers(ignoreShield, transform.position))
        {
            //if already frozen
            FrozenModifier alreadyFrozen = frozenObject.GetComponent<FrozenModifier>();
            if(alreadyFrozen)
            {
                //ignore all, or remove old script
                if (ignoreIfAlreadyFrozen)
                    return;
                else
                    Destroy(alreadyFrozen);
            }

            //add new frozen and initialize
            frozenObject.gameObject.AddComponent<FrozenModifier>().Init(duration);
        }
    }
}
