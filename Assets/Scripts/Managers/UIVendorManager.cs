using UnityEngine;
using UnityEngine.UI;
using TMPro;
using redd096;

[System.Serializable]
public struct WeaponVendorStruct
{
    public Button weaponButton;
    public WeaponBASE weapon;
}

public class UIVendorManager : MonoBehaviour
{
    [Header("UI Vendor")]
    [SerializeField] GameObject shopToActive = default;

    [Header("Weapons")]
    [SerializeField] WeaponVendorStruct[] weapons = default;

    [Header("Selected Weapon UI")]
    [SerializeField] TextMeshProUGUI weaponNameText = default;
    [SerializeField] Image weaponImage = default;
    [SerializeField] Button buyButton = default;
    [SerializeField] Button backButton = default;

    Player playerUsingVendor;
    WeaponBASE selectedWeapon;

    //TODO
    //sempre avere un'arma selezionata
    //se premi back, chiude il negozio senza comprare

    void Start()
    {
        //by default, hide shop
        CloseVendor();

        //set buttons vendor
        SetButtons();

        //hide selected weapon UI by default
        ShowHideSelectedWeaponUI(false);
    }

    #region private API

    void SetButtons()
    {
        foreach (WeaponVendorStruct weaponStruct in weapons)
        {
            if (weaponStruct.weaponButton == null)
                continue;

            //don't show if there is no weapon
            if (weaponStruct.weapon == null)
            {
                weaponStruct.weaponButton.gameObject.SetActive(false);
                continue;
            }

            //set event on click
            weaponStruct.weaponButton.onClick.AddListener(() => SelectWeapon(weaponStruct.weapon));

            //set button image and price
            weaponStruct.weaponButton.GetComponent<Image>().sprite = weaponStruct.weapon.uiSprite;
            weaponStruct.weaponButton.GetComponentInChildren<TextMeshProUGUI>().text = weaponStruct.weapon.WeaponPrice.ToString();
        }

        //set event on click for Buy and Back buttons
        buyButton.onClick.AddListener(() => Buy());
        buyButton.onClick.AddListener(() => Back());
    }

    void ShowHideSelectedWeaponUI(bool show)
    {
        //show or hide every element in selected weapon UI
        weaponNameText.gameObject.SetActive(show);
        weaponImage.gameObject.SetActive(show);
        buyButton.gameObject.SetActive(show);
        backButton.gameObject.SetActive(show);
    }

    void SetSelectableButtons()
    {
        //active/deactive interactable button based on current toilet paper
        foreach (WeaponVendorStruct weaponStruct in weapons)
        {
            if (weaponStruct.weaponButton == null || weaponStruct.weapon == null)
                continue;

            if (weaponStruct.weapon.WeaponPrice <= GameManager.instance.CurrentToiletPaper)
                weaponStruct.weaponButton.interactable = true;
            else
                weaponStruct.weaponButton.interactable = false;
        }

        //deactive weapons already seen
        foreach (WeaponVendorStruct weaponStruct in weapons)
        {
            if (weaponStruct.weaponButton == null || weaponStruct.weapon == null)
                continue;

            if (GameManager.instance.WeaponsAlreadyUsed.Contains(weaponStruct.weapon))
                weaponStruct.weaponButton.interactable = false;
        }
    }

    #endregion

    #region buttons on click

    /// <summary>
    /// Called by every weapon button - will show selected weapon
    /// </summary>
    /// <param name="weapon"></param>
    void SelectWeapon(WeaponBASE weapon)
    {
        if (weapon == null)
            return;

        //set selected weapon
        selectedWeapon = weapon;

        //set name and sprite
        weaponNameText.text = weapon.WeaponName;
        weaponImage.sprite = weapon.uiSprite;

        //show selected weapon UI
        ShowHideSelectedWeaponUI(true);
    }

    /// <summary>
    /// Called when press BUY - will give weapon to player
    /// </summary>
    void Buy()
    {
        //buy selected weapon
        playerUsingVendor.PickWeapon(selectedWeapon);
        GameManager.instance.CurrentToiletPaper -= selectedWeapon.WeaponPrice;

        //close vendor
        CloseVendor();
    }

    /// <summary>
    /// Called when press back - will hide selected weapon
    /// </summary>
    void Back()
    {
        //remove selected weapon
        selectedWeapon = null;

        //hide selected weapon UI
        ShowHideSelectedWeaponUI(false);
    }

    #endregion

    #region public API

    /// <summary>
    /// Active Vendor UI
    /// </summary>
    public void OpenVendor(Player player)
    {
        //set player using this vendor
        playerUsingVendor = player;
        playerUsingVendor.SetState("Vendor");

        //active/deactive buttons
        SetSelectableButtons();

        //show vendor
        shopToActive.SetActive(true);
    }

    /// <summary>
    /// Deactive Vendor UI
    /// </summary>
    public void CloseVendor()
    {
        //stop player using this vendor
        playerUsingVendor?.SetState("Vendor");
        playerUsingVendor = null;

        //hide vendor
        shopToActive.SetActive(false);
    }

    #endregion
}
