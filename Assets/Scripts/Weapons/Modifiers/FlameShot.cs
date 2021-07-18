using UnityEngine;

public class FlameShot : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] float duration = 2;
    [SerializeField] bool ignoreIfAlreadyBurn = false;
    [SerializeField] bool ignoreShield = false;

    [Header("Damage")]
    [SerializeField] float damage = 2;
    [SerializeField] float delayBetweenDamages = 0.2f;
    [SerializeField] float firstDelay = 0;

    Bullet bullet;

    void OnEnable()
    {
        //get references
        bullet = GetComponent<Bullet>();

        //add events
        if (bullet)
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
        if (CanApplyModifier<BurnModifier>(hit, out modifiersObject))
            ApplyModifier(modifiersObject);
    }

    void ApplyModifier(GetModifiersObject modifierObject)
    {
        //add new modifier and initialize
        modifierObject.gameObject.AddComponent<BurnModifier>().Init(duration, damage, delayBetweenDamages, firstDelay);
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
                if (ignoreIfAlreadyBurn)
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
