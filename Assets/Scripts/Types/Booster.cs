using System.Collections.Generic;

using UnityEngine;
public class Booster : Cell
	{
		public BoosterType type;
		public virtual List<GameObject> Resolve(int idx)
		{
	        return new List<GameObject>();
	    }

		public virtual void ShowFx(GamePools gamePools, int idx)
	    {
	    }
	    protected void AddTile(List<GameObject> tiles, int levelWidth, int x, int y)
	    {
	        if (x < 0 || x >= LevelManager.Instance.level.grid_width ||
	            y < 0 || y >= LevelManager.Instance.level.grid_height)
	        {
	            return;
	        }

	        var tileIndex = x + (y * LevelManager.Instance.level.grid_width);
	        var tile = LevelManager.Instance.cellEntities[tileIndex];
		    if (tile != null)
		    {
			    var block = tile.GetComponent<Cube>();
			    if (block != null && (block.type == CubeType.Empty))
			    {
				    return;
			    }
			    if (tiles.Contains(tile))
			    {
				    return;
			    }
			    tiles.Add(tile);
		    }
	    }
	    protected enum ComboType
	    {
		    None,
		    TNTCombo,
		    RocketCombo
	    }
		protected bool IsValidTile(Level level, int x, int y)
        {
            return x >= 0 && x < level.grid_width && y >= 0 && y < level.grid_height;
        }
	}

