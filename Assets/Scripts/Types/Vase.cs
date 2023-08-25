using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : Cube
{
    public Sprite brokenVase;
    public bool isDamaged;
    
    public void TakeDamage()
    {
        if (!isDamaged)
        {
            _renderer.sprite = brokenVase;
            isDamaged = true;
        }
    }
}
