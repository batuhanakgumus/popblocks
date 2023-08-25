using System.Collections.Generic;
using UnityEngine;

public class TNT : Booster
{
    public override List<GameObject> Resolve(int idx)
    {
        var scene = LevelManager.Instance;
        var tiles = new List<GameObject>();
        var x = idx % scene.level.grid_width;
        var y = idx / scene.level.grid_width;
        var combo = GetComboType(x, y);
        switch (combo)
        {
            case ComboType.None:
                AddTile(tiles, scene.level.grid_width, x, y);
                AddTile(tiles, scene.level.grid_width, x + 1, y + 1);
                AddTile(tiles, scene.level.grid_width, x, y + 1);
                AddTile(tiles, scene.level.grid_width, x + 1, y);
                
                AddTile(tiles, scene.level.grid_width, x, y - 1);
                AddTile(tiles, scene.level.grid_width, x - 1, y);
                AddTile(tiles, scene.level.grid_width, x - 1, y - 1);
                AddTile(tiles, scene.level.grid_width, x + 1, y - 1);
                AddTile(tiles, scene.level.grid_width, x - 1, y + 1);
                break;

            case ComboType.RocketCombo:

                for (int k = -1; k < 2; k++)
                {
                    for (var j = 0; j < LevelManager.Instance.level.grid_height; j++)
                    {
                        AddTile(tiles, LevelManager.Instance.level.grid_height, x + k, j);
                    }

                    for (var i = 0; i < LevelManager.Instance.level.grid_width; i++)
                    {
                        AddTile(tiles, LevelManager.Instance.level.grid_width, i, y + k);
                    }
                }

                break;

            case ComboType.TNTCombo:
                AddTile(tiles, scene.level.grid_width, x, y);
                AddTile(tiles, scene.level.grid_width, x - 1, y - 1);
                AddTile(tiles, scene.level.grid_width, x, y - 1);
                AddTile(tiles, scene.level.grid_width, x + 1, y - 1);
                AddTile(tiles, scene.level.grid_width, x - 1, y);
                AddTile(tiles, scene.level.grid_width, x + 1, y);
                AddTile(tiles, scene.level.grid_width, x - 1, y + 1);
                AddTile(tiles, scene.level.grid_width, x, y + 1);
                AddTile(tiles, scene.level.grid_width, x + 1, y + 1);
                AddTile(tiles, scene.level.grid_width, x - 2, y - 2);
                AddTile(tiles, scene.level.grid_width, x - 1, y - 2);
                AddTile(tiles, scene.level.grid_width, x, y - 2);
                AddTile(tiles, scene.level.grid_width, x + 1, y - 2);
                AddTile(tiles, scene.level.grid_width, x + 2, y - 2);
                AddTile(tiles, scene.level.grid_width, x - 2, y - 1);
                AddTile(tiles, scene.level.grid_width, x + 2, y - 1);
                AddTile(tiles, scene.level.grid_width, x - 2, y);
                AddTile(tiles, scene.level.grid_width, x + 2, y);
                AddTile(tiles, scene.level.grid_width, x - 2, y + 1);
                AddTile(tiles, scene.level.grid_width, x + 2, y + 1);
                AddTile(tiles, scene.level.grid_width, x - 2, y + 2);
                AddTile(tiles, scene.level.grid_width, x - 1, y + 2);
                AddTile(tiles, scene.level.grid_width, x, y + 2);
                AddTile(tiles, scene.level.grid_width, x + 1, y + 2);
                AddTile(tiles, scene.level.grid_width, x + 2, y + 2);
                break;
        }

        return tiles;
    }
    protected ComboType GetComboType(int x, int y)
    {
        var up = new TileDef(x, y - 1);
        var down = new TileDef(x, y + 1);
        var left = new TileDef(x - 1, y);
        var right = new TileDef(x + 1, y);

        int comboCount = 0;

        comboCount += GetComboPoints(up.x, up.y)
                      + (GetComboPoints(down.x, down.y))
                      + (GetComboPoints(left.x, left.y))
                      + GetComboPoints(right.x, right.y);

        if (comboCount > 0)
        {
            if (comboCount >= 10)
            {
                return ComboType.RocketCombo;
            }

            return ComboType.TNTCombo;
        }

        return ComboType.None;
    }

    
    protected int GetComboPoints(int x, int y)
    {
        var scene = LevelManager.Instance;
        var idx = x + (y * scene.level.grid_width);
        if (IsValidTile(scene.level, x, y) &&
            scene.cellEntities[idx] != null)
        {
            if (scene.cellEntities[idx].TryGetComponent(out TNT tnt))
            {
                return 1;
            }
            if (scene.cellEntities[idx].TryGetComponent(out Rocket rocket))
            {
                return 10;
            }
        }

        return 0;
    }
}