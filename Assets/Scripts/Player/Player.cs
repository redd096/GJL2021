using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }
}
