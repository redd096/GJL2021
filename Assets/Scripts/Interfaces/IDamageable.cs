using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Get damage and check if dead
    /// </summary>
    /// <param name="damage"></param>
    void GetDamage(float damage);

    /// <summary>
    /// Call it when health reach 0
    /// </summary>
    void Die();

    /// <summary>
    /// Push back character
    /// </summary>
    /// <param name="push"></param>
    void PushBack(Vector2 push);
}
