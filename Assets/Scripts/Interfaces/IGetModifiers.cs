public interface IGetModifiers
{
    System.Action<bool> onFrozen { get; set; }

    /// <summary>
    /// Hitted by Ice Shot
    /// </summary>
    /// <param name="activateFrozen"></param>
    void GetFrozen(bool activateFrozen);
}
