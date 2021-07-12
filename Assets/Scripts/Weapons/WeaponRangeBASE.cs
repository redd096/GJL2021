using System.Collections;
using UnityEngine;

public class WeaponRangeBASE : WeaponBASE
{
    [Header("Range Weapon")]
    [SerializeField] bool automatic = true;                 //keep pressed or click
    [SerializeField] float rateOfFire = 0.2f;
    [SerializeField] Transform[] barrels = default;         //spawn bullets - if more than one, shoot from every barrel
    [SerializeField] bool barrelSimultaneously = true;      //when more than one barrel, shoot every bullet simultaneously or with a delay (use always rate of fire)
    [SerializeField] float recoil = 1;                      //push back character when shoot
    [SerializeField] float charge = 0;                      //keep pressed to charge and release to shoot

    [Header("Bullet")]
    [SerializeField] GameObject bulletPrefab = default;
    [SerializeField] float damage = 10;
    [SerializeField] float bulletSpeed = 10;

    float lastShoot;

    public override void PressAttack()
    {
        //check rate of fire
        if (Time.time > lastShoot + rateOfFire)
        {
        }
    }

    public override void ReleaseAttack()
    {
        //if automatic, stop shoot
    }

    void Shoot()
    {
        //set delay
        lastShoot = Time.time;

        //shoot bullet
    }
}
