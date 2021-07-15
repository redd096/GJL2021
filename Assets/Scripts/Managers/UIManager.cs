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

        [Header("Toilet Paper")]
        [SerializeField] Image toiletPaperImage = default;
        [SerializeField] TextMeshProUGUI toiletPaperText = default;
        [SerializeField] string format = "x{0:00}";

        [Header("Weapon")]
        [SerializeField] Image weaponImage = default;
        [SerializeField] Sprite spriteWhenNoWeapon = default;

        [Header("Health")]
        [SerializeField] Slider sliderHealth = default;

        public Image ToiletPaperImage => toiletPaperImage;

        void Start()
        {
            //by default, deactive pause menu
            PauseMenu(false);
            UpdateToiletPaper(GameManager.instance.CurrentToiletPaper);
            UpdateWeaponImage(GameManager.instance.CurrentWeapon?.GetComponentInChildren<SpriteRenderer>().sprite);
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
        /// Set slider health
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="maxHealth"></param>
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (sliderHealth)
                sliderHealth.value = currentHealth / maxHealth;
        }
    }
}