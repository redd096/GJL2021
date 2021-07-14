using System.Collections;
using UnityEngine;
using redd096;

public class WeaponRange : WeaponBASE
{
    [Header("Range Weapon")]
    [SerializeField] bool automatic = true;                 //keep pressed or click
    [SerializeField] float rateOfFire = 0.2f;
    [SerializeField] float recoil = 1;                      //push back character when shoot
    public float NoiseAccuracy = 10;                        //rotate random the shot when instantiated

    [Header("Barrel")]
    [SerializeField] Transform[] barrels = default;         //spawn bullets - if more than one, shoot from every barrel
    [SerializeField] bool barrelSimultaneously = true;      //when more than one barrel, shoot every bullet simultaneously or with a delay (use always rate of fire)

    [Header("Bullet")]
    [SerializeField] Bullet bulletPrefab = default;
    [SerializeField] float damage = 10;
    [SerializeField] float bulletSpeed = 10;

    float timeForNextShot;
    Coroutine automaticShootCoroutine;

    //bullets
    Pooling<Bullet> bulletsPooling = new Pooling<Bullet>();
    GameObject bulletsParent;
    GameObject BulletsParent
    {
        get
        {
            if (bulletsParent == null)
                bulletsParent = new GameObject("Bullets Parent");

            return bulletsParent;
        }
    }

    //events
    public System.Action<Transform> onInstantiateBullet { get; set; }
    public System.Action onShoot { get; set; }

    public override void PressAttack()
    {
        //check rate of fire
        if (Time.time > timeForNextShot)
        {
            timeForNextShot = Time.time + rateOfFire;

            //shoot
            Shoot();

            //start coroutine if automatic
            if(automatic)
                automaticShootCoroutine = StartCoroutine(AutomaticShootCoroutine());
        }
    }

    public override void ReleaseAttack()
    {
        //stop coroutine if running (automatic shoot)
        if (automaticShootCoroutine != null)
            StopCoroutine(automaticShootCoroutine);
    }

    #region private API

    /// <summary>
    /// Create bullet from every barrel or one random barrel
    /// </summary>
    void Shoot()
    {
        //shoot every bullet
        if (barrelSimultaneously)
        {
            foreach (Transform barrel in barrels)
            {
                InstantiateBullet(barrel);
            }
        }
        //or shoot one bullet from random barrel
        else
        {
            Transform barrel = barrels[Random.Range(0, barrels.Length)];
            InstantiateBullet(barrel);
        }

        //pushback character
        Owner.PushBack(-Owner.DirectionAim * recoil);

        //call event
        onShoot?.Invoke();
    }

    /// <summary>
    /// Instantiate bullet and set it
    /// </summary>
    /// <param name="barrel"></param>
    void InstantiateBullet(Transform barrel)
    {
        //create random noise in accuracy
        float randomNoiseAccuracy = Random.Range(-NoiseAccuracy, NoiseAccuracy);
        Vector3 direction = Owner.DirectionAim.x < 0 ? -barrel.right : barrel.right;            //normal direction
        direction = Quaternion.AngleAxis(randomNoiseAccuracy, Vector3.forward) * direction;     //direction with noise

        Debug.DrawLine(barrel.position, barrel.position + direction, Color.red, 1);

        //instantiate bullet
        Bullet bullet = bulletsPooling.Instantiate(bulletPrefab, BulletsParent.transform);
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = barrel.rotation * Quaternion.AngleAxis(randomNoiseAccuracy, Vector3.forward);
        bullet.transform.localScale = Owner.DirectionAim.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;    //rotate bullet

        //and set it
        bullet.Init(Owner, direction, damage, bulletSpeed);

        //call event
        onInstantiateBullet?.Invoke(barrel);
    }

    /// <summary>
    /// Continue shooting
    /// </summary>
    /// <returns></returns>
    IEnumerator AutomaticShootCoroutine()
    {
        while (true)
        {
            //stop if lose owner
            if (Owner == null)
                break;

            //check rate of fire
            if (Time.time > timeForNextShot)
            {
                timeForNextShot = Time.time + rateOfFire;

                //shoot
                Shoot();
            }

            yield return null;
        }
    }

    #endregion
}
