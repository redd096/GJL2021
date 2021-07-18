namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    #region save class

    [System.Serializable]
    public class TutorialSaveClass
    {
        public bool firstTime = true;

        public TutorialSaveClass(bool firstTime)
        {
            this.firstTime = firstTime;
        }
    }

    #endregion

    [AddComponentMenu("redd096/Singletons/Game Manager")]
    [DefaultExecutionOrder(-100)]
    public class GameManager : Singleton<GameManager>
    {
        public const string TUTORIALNAME = "First Time";

        [Header("Tutorial or Shop ?")]
        [SerializeField] bool saveToNotRepeatAgain = true;
        [SerializeField] string tutorialSceneName = "Scena Tutorial";
        [SerializeField] string shopSceneName = "Scena Negozio";

        [Header("Saved elements for this run")]
        public List<GameObject> LevelsAlreadySeen = new List<GameObject>();
        public List<WeaponBASE> WeaponsAlreadyUsed = new List<WeaponBASE>();
        public WeaponBASE CurrentWeapon = null;
        public Sprite CurrentWeaponSprite = default;
        public int CurrentToiletPaper = 0;
        public float CurrentLife = 0;
        public int CurrentRoom = 0;
        [SerializeField] bool firstRoom = true;

        public UIManager uiManager { get; private set; }
        public LevelManager levelManager { get; private set; }
        public UIVendorManager uIVendorManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            //if this is the unique game manager
            if(instance == this)
            {
                //load and set options in scene
                OptionsManager.SetInScene(OptionsManager.LoadOptions());
            }
        }

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

                //set sprite (in case randomizer setted another sprite)
                if(player.CurrentWeapon)
                    player.CurrentWeapon.GetComponentInChildren<SpriteRenderer>().sprite = CurrentWeaponSprite;
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
            CurrentWeaponSprite = null;
            CurrentToiletPaper = 0;
            CurrentLife = 0;
            CurrentRoom = 0;
            firstRoom = true;
        }

        public void LoadTutorialOrShop()
        {
            //check if already saved tutorial, load shop
            if (saveToNotRepeatAgain)
            {
                TutorialSaveClass save = SaveLoadJSON.Load<TutorialSaveClass>(TUTORIALNAME);
                if (save != null && save.firstTime == false)
                {
                    FindObjectOfType<SceneLoader>().LoadScene(shopSceneName);
                    return;
                }
            }

            //else load tutorial scene
            FindObjectOfType<SceneLoader>().LoadScene(tutorialSceneName);
            SaveLoadJSON.Save(TUTORIALNAME, new TutorialSaveClass(true));
        }
    }
}