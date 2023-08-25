using System;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Booster
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public Direction direction;

    public override List<GameObject> Resolve(int idx)
    {
        var tiles = new List<GameObject>();
        var x = idx % LevelManager.Instance.level.grid_width;
        var y = idx / LevelManager.Instance.level.grid_width;
        switch (direction)
        {
            case Direction.Horizontal:
            {
                var combo = GetComboType(x, y);
                switch (combo)
                {
                    case ComboType.None:
                        for (var i = 0; i < LevelManager.Instance.level.grid_width; i++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_width, i, y);
                        }

                        break;

                    case ComboType.RocketCombo:

                        for (var j = 0; j < LevelManager.Instance.level.grid_height; j++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_height, x, j);
                        }

                        for (var i = 0; i < LevelManager.Instance.level.grid_width; i++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_width, i, y);
                        }
                        break;

                    case ComboType.TNTCombo:
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
                }
            }
                break;

            case Direction.Vertical:
            {
                var combo = GetComboType(x, y);
                switch (combo)
                {
                    case ComboType.None:
                    {
                        for (var j = 0; j < LevelManager.Instance.level.grid_height; j++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_width, x, j);
                        }

                        break;
                    }
                    case ComboType.RocketCombo:
                        for (var j = 0; j < LevelManager.Instance.level.grid_height; j++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_height, x, j);
                        }

                        for (var i = 0; i < LevelManager.Instance.level.grid_width; i++)
                        {
                            AddTile(tiles, LevelManager.Instance.level.grid_width, i, y);
                        }

                        break;

                    case ComboType.TNTCombo:
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
                }
            }
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
                return ComboType.TNTCombo;
            }

            return ComboType.RocketCombo;
        }

        return ComboType.None;
    }

    protected int GetComboPoints(int x, int y)
    {
        var tileEntities = LevelManager.Instance.tileEntities;
        var idx = x + (y * LevelManager.Instance.level.grid_width);
        if (IsValidTile(LevelManager.Instance.level, x, y) &&
            tileEntities[idx] != null)
        {
            if (tileEntities[idx].TryGetComponent(out TNT tnt))
            {
                if(tnt != null)
                return 10;
            }

            if (tileEntities[idx].TryGetComponent(out Rocket rocket))
            {
                if(rocket != null)
                    return 1;
            }
        }

        return 0;
    }
}