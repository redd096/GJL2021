using UnityEngine;
using redd096;

public class FrozenModifier : MonoBehaviour
{
    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float remainingTime;

    GetModifiersObject modifierObject;
    float duration;

    float timeFinishModifier = 0;

    public void Init(float duration)
    {
        this.duration = duration;

        //set timer
        OnStartTimer();
    }

    void Update()
    {
        //check if finish timer, and call when finish
        if(CheckFinishTimer())
            OnFinishTImer();
    }

    void SetModifier(bool activate)
    {
        //set modifier
        if (modifierObject)
            modifierObject.GetFrozen(activate);
    }

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
