namespace redd096
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        [Header("Toilet Paper")]
        [SerializeField] Image toiletPaperImage = default;
        [SerializeField] TextMeshProUGUI toiletPaperText = default;
        [SerializeField] string format = "x{0:00}";

        [Header("Weapon")]
        [SerializeField] Image weaponImage = default;
        [SerializeField] Sprite spriteWhenNoWeapon = default;

        [Header("Health")]
        [SerializeField] Slider sliderHealth = default;
        [SerializeField] TextMeshProUGUI textHealth = default;
        [SerializeField] string stringAfterTextHealth = "%";

        public Image ToiletPaperImage => toiletPaperImage;
        public Slider SliderHealth => sliderHealth;

        void Start()
        {
            //by default, deactive pause menu
            PauseMenu(false);
            EndMenu(false);
            UpdateToiletPaper(GameManager.instance.CurrentToiletPaper);
            UpdateWeaponImage(GameManager.instance.CurrentWeaponSprite);
            UpdateHealth(GameManager.instance.CurrentLife, GameManager.instance.levelManager.Players[0].MaxHealth);
        }

        /// <summary>
        /// Active/Deactive pause menu
        /// </summary>
        /// <param name="active"></param>
        public void PauseMenu(bool active)
        {
            if (pauseMenu == null)
            {
                return;
            }

            //active or deactive pause menu
            pauseMenu.SetActive(active);
        }

        /// <summary>
        /// Active/Deactive end menu
        /// </summary>
        /// <param name="active"></param>
        public void EndMenu(bool active)
        {
            if (endMenu == null)
                return;

            //active or deactive end menu
            endMenu.SetActive(active);

            //when active, be sure to deactive pause menu
            if (active)
                PauseMenu(false);
        }

        /// <summary>
        /// Set ToiletPaper text
        /// </summary>
        /// <param name="currentToiletPaper"></param>
        public void UpdateToiletPaper(int currentToiletPaper)
        {
            if(toiletPaperText)
                toiletPaperText.text = string.Format(format, currentToiletPaper);
        }

        /// <summary>
        /// Change WeaponImage's sprite
        /// </summary>
        /// <param name="weapon"></param>
        public void UpdateWeaponImage(Sprite weapon)
        {
            if (weaponImage)
                weaponImage.sprite = weapon != null ? weapon : spriteWhenNoWeapon;
        }

        /// <summary>
        /// Set slider health and text
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="maxHealth"></param>
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (sliderHealth)
                sliderHealth.value = currentHealth / maxHealth;

            //text is health*100 with clamp at 0
            if (textHealth)
                textHealth.text = Mathf.Max(0, (currentHealth / maxHealth * 100)).ToString("F0") + stringAfterTextHealth;
        }
    }
}