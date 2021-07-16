using UnityEngine;
using redd096;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        //open vendor
        GameManager.instance.uIVendorManager.OpenVendor(player);
    }
}
