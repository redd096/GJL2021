﻿using UnityEngine;
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

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isOpen;                                    //vendor is open or close?
    [ReadOnly] [SerializeField] Player playerUsingVendor;                       //which player is using this vendor
    IInteractable interactableForThisVendor;                                    //which interactable opened this vendor? - Unfortunately can't be shown in inspector
    [ReadOnly] [SerializeField] WeaponVendorStruct selectedWeapon;              //the weapon currently selected

    void Start()
    {
        //by default, hide shop
        CloseVendor();

        //set buttons vendor
        SetButtons();

        //hide selected weapon UI by default
        ShowHideSelectedWeaponUI(false);
    }

    void Update()
    {
        //when open
        if(isOpen)
        {
            //check, if player select a weapon
            GameObject uiSelectedObject = EventSystemRedd096.current.currentSelectedGameObject;
            if(uiSelectedObject != null)
            {
                foreach(WeaponVendorStruct weaponStruct in weapons)
                {
                    //if different from already selected, select this
                    if (weaponStruct.weaponButton.gameObject == EventSystemRedd096.current.currentSelectedGameObject && selectedWeapon.weaponButton != weaponStruct.weaponButton)
                        SelectWeapon(weaponStruct);
                }

                //if selected a weapon not available, select first available
                if (selectedWeapon.weaponButton == null || selectedWeapon.weaponButton.interactable == false)
                {
                    SelectFirstWeaponAvailable();
                    EventSystemRedd096.current.SetSelectedGameObject(selectedWeapon.weaponButton.gameObject);
                }
            }
            //else if player didn't select anything, select first weapon available
            else
            {
                SelectFirstWeaponAvailable();
                EventSystemRedd096.current.SetSelectedGameObject(selectedWeapon.weaponButton.gameObject);
            }
        }
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
            weaponStruct.weaponButton.onClick.AddListener(() => SelectWeapon(weaponStruct));

            //set button image and price
            weaponStruct.weaponButton.GetComponent<Image>().sprite = weaponStruct.weapon.GetComponentInChildren<SpriteRenderer>().sprite;
            weaponStruct.weaponButton.GetComponentInChildren<TextMeshProUGUI>().text = weaponStruct.weapon.WeaponPrice.ToString();
        }

        //set event on click for Buy and Back buttons
        buyButton.onClick.AddListener(() => Buy());
        backButton.onClick.AddListener(() => Back());
    }

    /// <summary>
    /// In theory we have always a weapon selected, but in case of error, don't show anything
    /// </summary>
    /// <param name="show"></param>
    void ShowHideSelectedWeaponUI(bool show)
    {
        //show or hide elements in selected weapon UI
        weaponNameText.gameObject.SetActive(show);
        weaponImage.gameObject.SetActive(show);
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

    void SelectFirstWeaponAvailable()
    {
        //select first weapon available
        for(int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weaponButton != null && weapons[i].weaponButton.interactable && weapons[i].weapon != null)
            {
                SelectWeapon(weapons[i]);
                break;
            }
        }

        //if no weapon selected, hide selected weapon UI
        if (selectedWeapon.weapon == null)
            ShowHideSelectedWeaponUI(false);
    }

    #endregion

    #region buttons on click

    /// <summary>
    /// Called by every weapon button - will show selected weapon
    /// </summary>
    /// <param name="weapon"></param>
    void SelectWeapon(WeaponVendorStruct weaponStruct)
    {
        //if selected a button without weapon, hide selected weapon UI
        if (weaponStruct.weaponButton == null || weaponStruct.weapon == null)
        {
            selectedWeapon.weaponButton = null;
            selectedWeapon.weapon = null;
            ShowHideSelectedWeaponUI(false);

            return;
        }

        //set selected weapon
        selectedWeapon = weaponStruct;

        //set name and sprite
        weaponNameText.text = selectedWeapon.weapon.WeaponName;
        weaponImage.sprite = selectedWeapon.weapon.GetComponentInChildren<SpriteRenderer>().sprite;

        //show selected weapon UI
        ShowHideSelectedWeaponUI(true);
    }

    /// <summary>
    /// Called when press BUY - will give weapon to player
    /// </summary>
    public void Buy()
    {
        if (selectedWeapon.weapon == null)
            return;

        //buy selected weapon
        playerUsingVendor.PickWeapon(selectedWeapon.weapon);
        GameManager.instance.CurrentToiletPaper -= selectedWeapon.weapon.WeaponPrice;

        //close vendor
        CloseVendor();
    }

    /// <summary>
    /// Called when press back - will close vendor
    /// </summary>
    public void Back()
    {
        //close vendor
        CloseVendor();
    }

    #endregion

    #region interact

    /// <summary>
    /// Active Vendor UI
    /// </summary>
    public void OpenVendor(Player player, IInteractable interactable)
    {
        //set is open and save from which interactable
        isOpen = true;
        interactableForThisVendor = interactable;

        //set player using this vendor
        playerUsingVendor = player;
        playerUsingVendor.SetState("Vendor");

        //active/deactive buttons
        SetSelectableButtons();

        //select first weapon available
        SelectFirstWeaponAvailable();

        //show vendor
        shopToActive.SetActive(true);
    }

    /// <summary>
    /// Deactive Vendor UI
    /// </summary>
    void CloseVendor()
    {
        //set is close and reactive interactable
        isOpen = false;
        interactableForThisVendor?.ReactiveInteractable();
        interactableForThisVendor = null;

        //stop player using this vendor
        playerUsingVendor?.SetState("Vendor");
        playerUsingVendor = null;

        //remove selected weapon
        selectedWeapon.weaponButton = null;
        selectedWeapon.weapon = null;

        //hide vendor
        shopToActive.SetActive(false);
    }

    #endregion
}
