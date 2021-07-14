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
    [SerializeField] [Min(0)] float radiusAreaDamage = 0;       //damage other characters in radius area
    [SerializeField] float knockBack = 1;                       //knockback hitted character

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
    [ReadOnly] [SerializeField] float damage = 0;
    [ReadOnly] [SerializeField] float bulletSpeed = 0;

    Rigidbody2D rb;
    Character owner;

    bool alreadyDead;

    //events
    public System.Action onHit { get; set; }

    private void Awake()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
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

        this.direction = direction;
        this.damage = damage;
        this.bulletSpeed = bulletSpeed;

        this.owner = owner;

        //ignore every collision with owner
        if(owner)
        {
            foreach (Collider2D ownerCol in owner.GetComponentsInChildren<Collider2D>())
                foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(bulletCol, ownerCol);
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

    void OnHit(GameObject hit)
    {
        if (alreadyDead)
            return;

        alreadyDead = true;

        //be sure to hit something, but not other bullets or this bullet owner
        if (hit == null || hit.GetComponentInParent<Bullet>() || hit.GetComponentInParent<Character>() == owner)
            return;

        //if hit something damageable, do damage and push back
        IDamageable damageable = hit.GetComponentInParent<IDamageable>();
        damageable?.GetDamage(damage, transform.position);
        damageable?.PushBack(direction * knockBack, transform.position);

        //if is not a penetrable layer, destroy this object
        if (layerPenetrable.ContainsLayer(hit.layer) == false)
        {
            //call event
            onHit?.Invoke();

            //damage in area too
            if (radiusAreaDamage > 0)
                DamageInArea(damageable);

            Pooling.Destroy(gameObject);
        }
    }

    void DamageInArea(IDamageable hit)
    {
        //be sure to not hit again the same
        List<IDamageable> damageables = new List<IDamageable>();
        if (hit != null)
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
}
