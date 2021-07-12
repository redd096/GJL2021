using System.Collections;
using UnityEngine;
using redd096;

public class WeaponRangeBASE : WeaponBASE
{
    [Header("Range Weapon")]
    [SerializeField] bool automatic = true;                 //keep pressed or click
    [SerializeField] float rateOfFire = 0.2f;
    [SerializeField] Transform[] barrels = default;         //spawn bullets - if more than one, shoot from every barrel
    [SerializeField] bool barrelSimultaneously = true;      //when more than one barrel, shoot every bullet simultaneously or with a delay (use always rate of fire)
    [SerializeField] float recoil = 1;                      //push back character when shoot

    [Header("Bullet")]
    [SerializeField] GameObject bulletPrefab = default;
    [SerializeField] float damage = 10;
    [SerializeField] float bulletSpeed = 10;

    float lastShoot;
    Coroutine automaticShootCoroutine;

    Pooling<GameObject> bulletsPooling = new Pooling<GameObject>();
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

    public override void PressAttack()
    {
        //check rate of fire
        if (Time.time > lastShoot + rateOfFire)
        {
            lastShoot = Time.time;

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
                CreateBullet(barrel);
            }
        }
        //or shoot one bullet from random barrel
        else
        {
            Transform barrel = barrels[Random.Range(0, barrels.Length)];
            CreateBullet(barrel);
        }
    }

    /// <summary>
    /// Instantiate bullet and set it
    /// </summary>
    /// <param name="barrel"></param>
    void CreateBullet(Transform barrel)
    {
        //instantiate bullet
        GameObject bullet = bulletsPooling.Instantiate(bulletPrefab, BulletsParent.transform);
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = barrel.rotation;

        //and set it
    }

    /// <summary>
    /// Continue shooting
    /// </summary>
    /// <returns></returns>
    IEnumerator AutomaticShootCoroutine()
    {
        while (true)
        {
            //check rate of fire
            if (Time.time > lastShoot + rateOfFire)
            {
                lastShoot = Time.time;

                //shoot
                Shoot();
            }

            yield return null;
        }
    }

    #endregion
}
