using UnityEngine;
using redd096;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    [Header("Open also when player has no weapon?")]
    [SerializeField] bool checkPlayerHasNoWeapon = true;

    [Header("When Player Can Interact")]
    [SerializeField] GameObject objectToActivate = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isOpen;

    void Awake()
    {
        //by default, object to activate is hidden
        if (objectToActivate)
            objectToActivate.SetActive(false);
    }

    void FixedUpdate()
    {
        //if change state (can open or not)
        if (CheckCanOpen() != isOpen)
            OpenCloseShop();
    }

    #region open or close

    bool CheckCanOpen()
    {
        bool canOpen = true;

        //check player has no weapon
        if (canOpen && checkPlayerHasNoWeapon)
            canOpen = GameManager.instance.levelManager.Players[0].CurrentWeapon == null;

        return canOpen;
    }

    void OpenCloseShop()
    {
        //open or close
        isOpen = !isOpen;

        //activate object
        if (objectToActivate)
            objectToActivate.SetActive(isOpen);
    }

    #endregion

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        if (isOpen == false)
            return;

        //stop this script
        enabled = false;    //stop check open/close
        isOpen = false;     //can't interact anymore

        //open vendor
        GameManager.instance.uIVendorManager.OpenVendor(player);
    }
}
