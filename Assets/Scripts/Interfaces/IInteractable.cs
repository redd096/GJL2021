
public interface IInteractable
{
    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    void Interact(Player player);

    /// <summary>
    /// Called to re-enable interactable (for example when close vendor)
    /// </summary>
    void ReactiveInteractable();
}
