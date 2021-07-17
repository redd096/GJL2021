namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Game Manager")]
    [DefaultExecutionOrder(-100)]
    public class GameManager : Singleton<GameManager>
    {
        [Header("Saved elements for this run")]
        public List<GameObject> LevelsAlreadySeen = new List<GameObject>();
        public List<WeaponBASE> WeaponsAlreadyUsed = new List<WeaponBASE>();
        public WeaponBASE CurrentWeapon = null;
        public int CurrentToiletPaper = 0;
        public float CurrentLife = 0;
        public bool firstRoom = true;

        public UIManager uiManager { get; private set; }
        public LevelManager levelManager { get; private set; }
        public UIVendorManager uIVendorManager { get; private set; }

        protected override void SetDefaults()
        {
            //get references
            uiManager = FindObjectOfType<UIManager>();
            levelManager = FindObjectOfType<LevelManager>();
            uIVendorManager = FindObjectOfType<UIVendorManager>();
        }

        /// <summary>
        /// When player enter in first room, can pick weapon from inspector, after use game manager weapon
        /// </summary>
        public void PickWeaponSaved(Player player, WeaponBASE playerWeaponPrefab)
        {
            //in first room, pick weapon from player inspector
            if(firstRoom)
            {
                firstRoom = false;

                player.PickWeapon(playerWeaponPrefab);
            }
            //after, pick weapon from saved one
            else
            {
                player.PickWeapon(CurrentWeapon);
            }
        }

        /// <summary>
        /// Reset every var
        /// </summary>
        public void ResetAll()
        {
            LevelsAlreadySeen.Clear();
            WeaponsAlreadyUsed.Clear();
            CurrentWeapon = null;
            CurrentToiletPaper = 0;
            CurrentLife = 0;
            firstRoom = true;
        }
    }
}