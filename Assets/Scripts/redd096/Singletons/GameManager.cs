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
        public int CurrentToiletPaper = 0;
        public WeaponBASE CurrentWeapon = null;

        public UIManager uiManager { get; private set; }
        public LevelManager levelManager { get; private set; }

        protected override void SetDefaults()
        {
            //get references
            uiManager = FindObjectOfType<UIManager>();
            levelManager = FindObjectOfType<LevelManager>();
        }
    }
}