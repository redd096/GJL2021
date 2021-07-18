using UnityEngine;
using redd096;

public class BurnModifier : MonoBehaviour
{
    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float remainingTime;
    [ReadOnly] [SerializeField] float timeToNextDamage;

    GetModifiersObject modifierObject;
    float duration;
    float damage;
    float delayBetweenDamages;
    float firstDelay;

    float timeFinishModifier = 0;
    float timeDoDamage = 0;

    public void Init(float duration, float damage, float delayBetweenDamages, float firstDelay)
    {
        this.duration = duration;
        this.damage = damage;
        this.delayBetweenDamages = delayBetweenDamages;
        this.firstDelay = firstDelay;

        //on start, set time to damage
        OnStartDamage();

        //set timer
        OnStartTimer();
    }

    void Update()
    {
        //check do damage
        if (CheckDoDamage())
            DoDamage();

        //check if finish timer, and call when finish
        if (CheckFinishTimer())
            OnFinishTImer();
    }

    void SetModifier(bool activate)
    {
        //set modifier
        if (modifierObject)
            modifierObject.GetBurn(activate);
    }

    #region damage

    void OnStartDamage()
    {
        //set time to next damage
        timeDoDamage = Time.time + firstDelay;
    }

    bool CheckDoDamage()
    {
        //debug
        timeToNextDamage = timeDoDamage - Time.time;

        //on finish timer
        if (timeDoDamage > 0 && Time.time > timeDoDamage)
        {
            return true;
        }

        return false;
    }

    void DoDamage()
    {
        //damage modifier object
        modifierObject.GetComponent<IDamageable>()?.GetDamage(damage);

        //set delay next damage
        timeDoDamage = Time.time + delayBetweenDamages;
    }

    #endregion

    #region timer

    void OnStartTimer()
    {
        //set timer
        timeFinishModifier = Time.time + duration;

        //and set modifier
        modifierObject = GetComponent<GetModifiersObject>();
        SetModifier(true);
    }

    bool CheckFinishTimer()
    {
        //debug
        remainingTime = timeFinishModifier - Time.time;

        //on finish timer
        if (timeFinishModifier > 0 && Time.time > timeFinishModifier)
        {
            return true;
        }

        return false;
    }

    void OnFinishTImer()
    {
        //reset timer
        timeFinishModifier = 0;

        //and set modifier
        SetModifier(false);
        modifierObject = null;

        //remove this component
        Destroy(this);
    }

    #endregion
}
