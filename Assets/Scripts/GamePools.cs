using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GamePools : MonoBehaviour
{
    public ObjectPool block1Pool;
    public ObjectPool block2Pool;
    public ObjectPool block3Pool;
    public ObjectPool block4Pool;
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
        Assert.IsNotNull(block1Pool);
        Assert.IsNotNull(block2Pool);
        Assert.IsNotNull(block3Pool);
        Assert.IsNotNull(block4Pool);
        Assert.IsNotNull(emptyTilePool);
        Assert.IsNotNull(boxPool);
        Assert.IsNotNull(stonePool);
        Assert.IsNotNull(horizontalRocketPool);
        Assert.IsNotNull(verticalRocketPool);
        Assert.IsNotNull(tntPool);

        _blockPools.Add(block1Pool);
        _blockPools.Add(block2Pool);
        _blockPools.Add(block3Pool);
        _blockPools.Add(block4Pool);
    }

    /// <summary>
    /// Returns the tile entity corresponding to the specified level tile.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="tile">The level tile.</param>
    /// <returns>The tile entity corresponding to the specified level tile.</returns>
    public TileEntity GetTileEntity(Level level, LevelTile tile)
    {
        if (tile is BlockTile)
        {
            var blockTile = (BlockTile)tile;
            switch (blockTile.type)
            {
                case BlockType.b:
                    return block1Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.g:
                    return block2Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.r:
                    return block3Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.y:
                    return block4Pool.GetObject().GetComponent<TileEntity>();
                

                case BlockType.rand:
                {
                    var randomIdx = Random.Range(0, 4);
                    return _blockPools[randomIdx].GetObject().GetComponent<TileEntity>();
                }

                case BlockType.Empty:
                    return emptyTilePool.GetObject().GetComponent<TileEntity>();

                case BlockType.bo:
                    return boxPool.GetObject().GetComponent<TileEntity>();

                case BlockType.s:
                    return stonePool.GetObject().GetComponent<TileEntity>();
                
                case BlockType.v:
                    return vasePool.GetObject().GetComponent<TileEntity>();
            }
        }
        else if (tile is BoosterTile)
        {
            var boosterTile = (BoosterTile)tile;
            switch (boosterTile.type)
            {
                case BoosterType.HorizontalRocket:
                    return horizontalRocketPool.GetObject().GetComponent<TileEntity>();

                case BoosterType.VerticalRocket:
                    return verticalRocketPool.GetObject().GetComponent<TileEntity>();

                case BoosterType.TNT:
                    return tntPool.GetObject().GetComponent<TileEntity>();
            }
        }

        return null;
    }

    public TileEntity GetTileEntityWithString(Level level, string tile)
    {
        if (tile is string)
        {
            var blockTile = tile;
            switch (blockTile)
            {
                case "b":
                    return block1Pool.GetObject().GetComponent<TileEntity>();

                case "g":
                    return block2Pool.GetObject().GetComponent<TileEntity>();

                case "r":
                    return block3Pool.GetObject().GetComponent<TileEntity>();

                case "y":
                    return block4Pool.GetObject().GetComponent<TileEntity>();


                case "rand":
                {
                    var randomIdx = Random.Range(0, 4);
                    return _blockPools[randomIdx].GetObject().GetComponent<TileEntity>();
                }

                case "Empty":
                    return emptyTilePool.GetObject().GetComponent<TileEntity>();

                case "bo":
                    return boxPool.GetObject().GetComponent<TileEntity>();

                case "s":
                    return stonePool.GetObject().GetComponent<TileEntity>();

                case "v":
                    return vasePool.GetObject().GetComponent<TileEntity>();
                case "roh":
                    return horizontalRocketPool.GetObject().GetComponent<TileEntity>();
                case "rov":
                    return verticalRocketPool.GetObject().GetComponent<TileEntity>();
                case "t":
                    return tntPool.GetObject().GetComponent<TileEntity>();
            }
        }
        return null;
    }
}