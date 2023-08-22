using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : Cube
{
    private SpriteRenderer _renderer;
    public Sprite brokenVase;
    public bool isDamaged;
    
    public void TakeDamage()
    {
        if (!isDamaged)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = brokenVase;
            isDamaged = true;
        }
    }
}
