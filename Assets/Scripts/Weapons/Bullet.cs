using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Use trigger or collision enter")]
    [SerializeField] bool useTrigger = true;

    [Header("Layer Penetrable")]
    [SerializeField] LayerMask layerPenetrable = default;

    [Header("Bullet")]
    [SerializeField] bool doAreaDamage = false;
    [CanShow("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 0;         //damage other characters in radius area
    [CanShow("doAreaDamage")] [SerializeField] bool areaCanDamageWhoShoot = false;          //is possible to damage owner with area damage
    [CanShow("doAreaDamage")] [SerializeField] bool areaCanDamageWhoHit = false;            //is possible to damage again who hit this bullet
    [SerializeField] float knockBack = 1;                       //knockback hitted character

    [Header("Timer Autodestruction (0 = no autodestruction)")]
    [SerializeField] float delayAutodestruction = 0;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
    [ReadOnly] [SerializeField] float damage = 0;
    [ReadOnly] [SerializeField] float bulletSpeed = 0;

    Rigidbody2D rb;
    Character owner;
    bool alreadyDead;
    List<IDamageable> alreadyHit = new List<IDamageable>();

    Coroutine autodestructionCoroutine;

    //events
    public System.Action onHit { get; set; }
    public System.Action onAutodestruction { get; set; }

    private void Awake()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        //draw area damage
        if (doAreaDamage && radiusAreaDamage > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusAreaDamage);
        }
    }

    /// <summary>
    /// Initialize bullet
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="damage"></param>
    /// <param name="bulletSpeed"></param>
    public void Init(Character owner, Vector2 direction, float damage, float bulletSpeed)
    {
        //reset vars
        alreadyDead = false;
        alreadyHit.Clear();

        this.direction = direction;
        this.damage = damage;
        this.bulletSpeed = bulletSpeed;

        this.owner = owner;

        //ignore every collision with owner
        if (owner)
        {
            foreach (Collider2D ownerCol in owner.GetComponentsInChildren<Collider2D>())
                foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(bulletCol, ownerCol);
        }

        //autodestruction coroutine
        if(delayAutodestruction > 0)
        {
            autodestructionCoroutine = StartCoroutine(AutoDestructionCoroutine());
        }
    }

    void FixedUpdate()
    {
        //move bullet
        rb.velocity = direction * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //be sure which collider use, then call OnHit
        if (useTrigger)
            OnHit(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //be sure which collider use, then call OnHit
        if (useTrigger == false)
            OnHit(collision.gameObject);
    }

    #region private API

    void OnHit(GameObject hit)
    {
        if (alreadyDead)
            return;

        //be sure to hit something, but not other bullets or this bullet owner
        if (hit == null || hit.GetComponentInParent<Bullet>() || hit.GetComponentInParent<Character>() == owner)
            return;

        //don't hit again same damageable (for penetrate shots)
        IDamageable damageable = hit.GetComponentInParent<IDamageable>();
        if (alreadyHit.Contains(damageable))
            return;

        //if hit something damageable, do damage and push back
        damageable?.GetDamage(damage, transform.position);
        damageable?.PushBack(direction * knockBack, transform.position);

        //if is not a penetrable layer, destroy this object
        if (layerPenetrable.ContainsLayer(hit.layer) == false)
        {
            alreadyDead = true;

            //call event
            onHit?.Invoke();

            //damage in area too
            if (doAreaDamage && radiusAreaDamage > 0)
                DamageInArea(damageable);

            //destroy
            Die();
        }
    }

    void DamageInArea(IDamageable hit)
    {
        //be sure to not hit again the same
        List<IDamageable> damageables = new List<IDamageable>();

        //be sure to not hit owner (if necessary)
        if (areaCanDamageWhoShoot == false)
            damageables.Add(owner.GetComponent<IDamageable>());

        //be sure to not hit who was already hit by bullet (if necessary)
        if (areaCanDamageWhoHit == false && hit != null)
            damageables.Add(hit);

        //find every object damageable in area
        foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
        {
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null && damageables.Contains(damageable) == false)
            {
                //add only one time in the list, and do damage
                damageables.Add(damageable);
                damageable.GetDamage(damage, transform.position);
            }
        }
    }

    IEnumerator AutoDestructionCoroutine()
    {
        //wait
        yield return new WaitForSeconds(delayAutodestruction);

        //call event
        onAutodestruction?.Invoke();

        //then destroy
        Die();
    }

    #endregion

    void Die()
    {
        //if coroutine is running, stop it
        if (autodestructionCoroutine != null)
            StopCoroutine(autodestructionCoroutine);

        //destroy bullet
        Pooling.Destroy(gameObject);
    }
}
