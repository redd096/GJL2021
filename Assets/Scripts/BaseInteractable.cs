using UnityEngine;
using UnityEngine.Events;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("On Interact")]
    [SerializeField] UnityEvent onInteract = default;

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        //call on interact
        onInteract?.Invoke();
    }
}
