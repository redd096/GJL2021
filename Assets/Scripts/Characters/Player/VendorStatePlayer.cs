using UnityEngine;
using redd096;

public class VendorStatePlayer : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //inputs
        BuyWeaponVendor(InputRedd096.GetButtonDown("Buy Weapon Vendor"));
        CloseVendor(InputRedd096.GetButtonDown("Close Vendor"));
    }

    void BuyWeaponVendor(bool inputBuy)
    {
        //buy weapon (and close vendor)
        if (inputBuy)
        {
            GameManager.instance.uIVendorManager.Buy();
        }
    }

    void CloseVendor(bool inputClose)
    {
        //close vendor
        if(inputClose)
        {
            GameManager.instance.uIVendorManager.Back();
        }
    }
}
