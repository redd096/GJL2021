using UnityEngine;
using redd096;

public class BulletFeedbacks : MonoBehaviour
{
    [Header("On Hit")]
    [SerializeField] ParticleSystem particlesOnHit = default;
    [SerializeField] AudioStruct audioOnHit = default;

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

    void OnHit()
    {

    }
}
