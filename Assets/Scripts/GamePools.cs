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
                case BlockType.Block1:
                    return block1Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.Block2:
                    return block2Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.Block3:
                    return block3Pool.GetObject().GetComponent<TileEntity>();

                case BlockType.Block4:
                    return block4Pool.GetObject().GetComponent<TileEntity>();
                

                case BlockType.RandomBlock:
                {
                    var randomIdx = Random.Range(0, level.availableColors.Count);
                    return _blockPools[(int)level.availableColors[randomIdx]].GetObject().GetComponent<TileEntity>();
                }

                case BlockType.Empty:
                    return emptyTilePool.GetObject().GetComponent<TileEntity>();

                case BlockType.Box:
                    return boxPool.GetObject().GetComponent<TileEntity>();

                case BlockType.Stone:
                    return stonePool.GetObject().GetComponent<TileEntity>();
                
                case BlockType.Vase:
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
}