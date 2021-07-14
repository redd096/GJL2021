using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    void GetDamage(float damage, Vector2 hitPosition = default);

    /// <summary>
    /// Call it when health reach 0
    /// </summary>
    void Die();

    /// <summary>
    /// Push back character
    /// </summary>
    /// <param name="push"></param>
    /// <param name="resetPreviousPush"></param>
    void PushBack(Vector2 push, Vector2 hitPosition = default, bool resetPreviousPush = false);
}
