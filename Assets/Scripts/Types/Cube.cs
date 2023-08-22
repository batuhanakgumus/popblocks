// Copyright (C) 2017-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
	/// The base class for blocks.
	/// </summary>
	public class Cube : Cell
	{
	    public CubeType type;
	    // private SpriteRenderer _renderer;
	    // public Sprite rocketHintSprite;
	    // public Sprite tntHintSprite;
	    // public Sprite sprite;
	    //
	    // private void Awake()
	    // {
		   //  _renderer = GetComponent<SpriteRenderer>();
	    // }
	    //
	    // public void GiveRocketHint()
	    // {
		   //  _renderer.sprite = rocketHintSprite;
	    // }
	    // public void GiveTntHint()
	    // {
		   //  _renderer.sprite = tntHintSprite;
	    // }
	    //
	    // public void NoHint()
	    // {
		   //  _renderer.sprite = sprite;
	    // }

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

