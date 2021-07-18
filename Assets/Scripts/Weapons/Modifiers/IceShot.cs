using UnityEngine;

public class IceShot : MonoBehaviour
{
    [Header("Ice - ignore or restart Effect")]
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
        GetModifiersObject modifiersObject;

        //if can, apply modifier
        if (CanApplyModifier<FrozenModifier>(hit, out modifiersObject))
            ApplyModifier(modifiersObject);
    }

    void ApplyModifier(GetModifiersObject modifierObject)
    {
        //add new modifier and initialize
        modifierObject.gameObject.AddComponent<FrozenModifier>().Init(duration);
    }

    #region private API

    bool CanApplyModifier<T>(GameObject hit, out GetModifiersObject modifierObject) where T : Component
    {
        modifierObject = hit.GetComponentInParent<GetModifiersObject>();

        //if can get modifiers and can apply it
        if (modifierObject != null && modifierObject.CanApplyModifiers(ignoreShield, transform.position))
        {
            //if already applied
            T alreadyApplied = modifierObject.GetComponent<T>();
            if (alreadyApplied)
            {
                //ignore all, or remove old script
                if (ignoreIfAlreadyFrozen)
                    return false;
                else
                    Destroy(alreadyApplied);
            }

            return true;
        }

        return false;
    }

    #endregion
}
