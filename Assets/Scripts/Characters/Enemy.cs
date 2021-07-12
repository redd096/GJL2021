using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private void Update()
    {
        //just push back
        MoveCharacter(Vector2.zero);
    }
}
