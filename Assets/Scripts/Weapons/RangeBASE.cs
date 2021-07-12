using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBASE : WeaponBASE
{
    [Header("Range Weapon")]
    [SerializeField] bool automatic = true;
    [SerializeField] float rateOfFire = 0.2f;
    [SerializeField] Transform[] bulletSpawns = default;

    [Header("Bullet")]
    [SerializeField] GameObject bullet = default;
    [SerializeField] float damage = 10;
    [SerializeField] float bulletSpeed = 10;

    float lastShoot;

    public void Shoot()
    {
        //check rate of fire
        if(Time.time > lastShoot + rateOfFire)
        {
            lastShoot = Time.time;
        }
    }
}
