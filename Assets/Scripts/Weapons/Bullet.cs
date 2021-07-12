using UnityEngine;
using redd096;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Use trigger or collision enter")]
    [SerializeField] bool useTrigger = true;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
    [ReadOnly] [SerializeField] float damage = 0;
    [ReadOnly] [SerializeField] float bulletSpeed = 0;

    Rigidbody2D rb;
    Character owner;

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
        //be sure which collider use, and be sure to not hit other bullets
        if (useTrigger == false || collision.GetComponentInParent<Bullet>())
            return;

        //do nothing if hit owner
        if (collision.GetComponentInParent<Character>() == owner)
            return;

        //if hit something damageable, do damage
        collision.GetComponentInParent<IDamageable>()?.GetDamage(damage);

        //destroy this object
        Pooling.Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //be sure which collider use, and be sure to not hit other bullets
        if (useTrigger || collision.gameObject.GetComponentInParent<Bullet>())
            return;

        //do nothing if hit owner
        if (collision.gameObject.GetComponentInParent<Character>() == owner)
            return;

        //if hit something damageable, do damage
        collision.gameObject.GetComponentInParent<IDamageable>()?.GetDamage(damage);

        //destroy this object
        Pooling.Destroy(gameObject);
    }
}
