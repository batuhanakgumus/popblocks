using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GamePools : MonoBehaviour
{
    public ObjectPool blueCubePool;
    public ObjectPool greenCubePool;
    public ObjectPool RedCubePool;
    public ObjectPool yellowCubePool;
    public ObjectPool emptyTilePool;
    public ObjectPool boxPool;
    public ObjectPool stonePool;
    public ObjectPool vasePool;

    public ObjectPool horizontalRocketPool;
    public ObjectPool verticalRocketPool;
    public ObjectPool tntPool;

    private readonly List<ObjectPool> _blockPools = new List<ObjectPool>();

    private void Awake()
    {
        Assert.IsNotNull(blueCubePool);
        Assert.IsNotNull(greenCubePool);
        Assert.IsNotNull(RedCubePool);
        Assert.IsNotNull(yellowCubePool);
        Assert.IsNotNull(emptyTilePool);
        Assert.IsNotNull(boxPool);
        Assert.IsNotNull(stonePool);
        Assert.IsNotNull(horizontalRocketPool);
        Assert.IsNotNull(verticalRocketPool);
        Assert.IsNotNull(tntPool);

        _blockPools.Add(blueCubePool);
        _blockPools.Add(greenCubePool);
        _blockPools.Add(RedCubePool);
        _blockPools.Add(yellowCubePool);
    }

    /// <summary>
    /// Returns the tile entity corresponding to the specified level tile.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="tile">The level tile.</param>
    /// <returns>The tile entity corresponding to the specified level tile.</returns>
    public Cell GetCell(Level level, CellTile tile)
    {
        if (tile is CubeTile)
        {
            var blockTile = (CubeTile)tile;
            switch (blockTile.type)
            {
                case CubeType.b:
                    return blueCubePool.GetObject().GetComponent<Cell>();

                case CubeType.g:
                    return greenCubePool.GetObject().GetComponent<Cell>();

                case CubeType.r:
                    return RedCubePool.GetObject().GetComponent<Cell>();

                case CubeType.y:
                    return yellowCubePool.GetObject().GetComponent<Cell>();
                

                case CubeType.rand:
                {
                    var randomIdx = Random.Range(0, 4);
                    return _blockPools[randomIdx].GetObject().GetComponent<Cell>();
                }

                case CubeType.Empty:
                    return emptyTilePool.GetObject().GetComponent<Cell>();

                case CubeType.bo:
                    return boxPool.GetObject().GetComponent<Cell>();

                case CubeType.s:
                    return stonePool.GetObject().GetComponent<Cell>();
                
                case CubeType.v:
                    return vasePool.GetObject().GetComponent<Cell>();
            }
        }
        else if (tile is BoosterTile)
        {
            var boosterTile = (BoosterTile)tile;
            switch (boosterTile.type)
            {
                case BoosterType.HorizontalRocket:
                    return horizontalRocketPool.GetObject().GetComponent<Cell>();

                case BoosterType.VerticalRocket:
                    return verticalRocketPool.GetObject().GetComponent<Cell>();

                case BoosterType.TNT:
                    return tntPool.GetObject().GetComponent<Cell>();
            }
        }

        return null;
    }

    public Cell GetCellWithString(Level level, string tile)
    {
        if (tile is string)
        {
            var blockTile = tile;
            switch (blockTile)
            {
                case "b":
                    return blueCubePool.GetObject().GetComponent<Cell>();

                case "g":
                    return greenCubePool.GetObject().GetComponent<Cell>();

                case "r":
                    return RedCubePool.GetObject().GetComponent<Cell>();

                case "y":
                    return yellowCubePool.GetObject().GetComponent<Cell>();
                
                case "rand":
                {
                    var randomIdx = Random.Range(0, 4);
                    return _blockPools[randomIdx].GetObject().GetComponent<Cell>();
                }

                case "Empty":
                    return emptyTilePool.GetObject().GetComponent<Cell>();

                case "bo":
                    return boxPool.GetObject().GetComponent<Cell>();

                case "s":
                    return stonePool.GetObject().GetComponent<Cell>();

                case "v":
                    return vasePool.GetObject().GetComponent<Cell>();
                
                case "roh":
                    return horizontalRocketPool.GetObject().GetComponent<Cell>();
                
                case "rov":
                    return verticalRocketPool.GetObject().GetComponent<Cell>();
                
                case "t":
                    return tntPool.GetObject().GetComponent<Cell>();
            }
        }
        return null;
    }
}