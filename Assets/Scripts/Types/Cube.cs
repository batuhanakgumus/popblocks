using System;
using UnityEngine;

	public class Cube : Cell
	{
	    public CubeType type;
	    protected SpriteRenderer _renderer;
	    public Sprite rocketHintSprite;
	    public Sprite tntHintSprite;
	    public Sprite sprite;
	    
	    private void Awake()
	    {
		    _renderer = GetComponent<SpriteRenderer>();
	    }

	    public override void OnEnable()
	    {
		    NoHint();
		    base.OnEnable();
	    }

	    public void GiveRocketHint()
	    {
		    _renderer.sprite = rocketHintSprite;
	    }
	    public void GiveTntHint()
	    {
		    _renderer.sprite = tntHintSprite;
	    }
	    
	    public void NoHint()
	    {
		    _renderer.sprite = sprite;
	    }

	}
	
	public enum CubeType
	{
		b,
		g,
		r,
		y,
		rand,
		Empty,
		bo,
		s,
		v
	}

