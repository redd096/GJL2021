using UnityEngine;
using redd096;

public class PropAndTrap : BASEDestructibleProps
{
    [Header("Damage on Hit (use trigger or collision enter")]
    [SerializeField] bool doDamageOnHit = false;
    [CanShow("doDamageOnHit")] [SerializeField] bool ignoreShieldOnHit = false;
    [CanShow("doDamageOnHit")] [SerializeField] bool useTrigger = false;
    [CanShow("doDamageOnHit")] [SerializeField] LayerMask layerToIgnore = default;
    [CanShow("doDamageOnHit")] [SerializeField] float damageOnHit = 10;
    [CanShow("doDamageOnHit")] [SerializeField] float knockBackOnHit = 10;
    [CanShow("doDamageOnHit")] [SerializeField] bool dieOnHit = false;

    //animation events
    public System.Action onHit { get; set; }
    public System.Action<bool> onActiveByTimer { get; set; }

    TimerTrap timerTrap;

    protected override void Awake()
    {
        base.Awake();

        //check if has a timer
        timerTrap = GetComponent<TimerTrap>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //don't hit if is a timer trap
        if (timerTrap)
            return;

        //check if do damage
        if (useTrigger == false || doDamageOnHit == false)
            return;

        //hit
        OnHit(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //don't hit if is a timer trap
        if (timerTrap)
            return;

        //check if do damage
        if (useTrigger || doDamageOnHit == false)
            return;

        //hit
        OnHit(collision.gameObject);
    }

    public void OnHit(GameObject collisionObject, bool callEvent = true)
    {
        //be sure is not already dead
        if (alreadyDead)
            return;

        //check if layer is to ignore
        if (layerToIgnore.ContainsLayer(collisionObject.layer))
            return;

        //call event
        if(callEvent)
            onHit?.Invoke();

        //do damage and push back
        IDamageable damageable = collisionObject.GetComponentInParent<IDamageable>();
        damageable?.GetDamage(damageOnHit, ignoreShieldOnHit, transform.position);
        damageable?.PushBack((collisionObject.transform.position - transform.position).normalized * knockBackOnHit);

        //die on hit
        if(dieOnHit)
        {
            Die();
        }
    }

    /// <summary>
    /// Called by TimerTrap, to call event
    /// </summary>
    /// <param name="active"></param>
    public void ActiveByTimer(bool active)
    {
        //call event
        onActiveByTimer?.Invoke(active);
    }
}
