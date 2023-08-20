using System.Collections.Generic;
using UnityEngine;

public class GameMechanic : MonoBehaviour
{
    public LevelManager levelManager;
    private Camera _mainCamera;
    private Vector3 _lastClickedPoint;


    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Block"))
            {
                var hitBlock = hit.collider.gameObject.GetComponent<TileEntity>();
                _lastClickedPoint = hit.transform.position;
                if (IsBoosterBlock(hitBlock))
                {
                    DestroyBooster(hitBlock);
                }
                else
                {
                    DestroyBlock(hitBlock);
                }
            }
        }
    }

    private bool IsBoosterBlock(TileEntity tileEntity)
    {
        return tileEntity is Booster;
    }

    private void DestroyBlock(TileEntity blockToDestroy)
    {
        var blockIdx = levelManager.tileEntities.FindIndex(x => x == blockToDestroy.gameObject);
        var blocksToDestroy = new List<GameObject>();
        GetMatches(blockToDestroy.gameObject, blocksToDestroy);
        if (blocksToDestroy.Count > 0)
        {
            foreach (var block in blocksToDestroy)
            {
                var idx = levelManager.tileEntities.FindIndex(x => x == block);
                DestroyConnectedStones(idx);
                DestroyTileEntity(block.GetComponent<TileEntity>(), idx);
            }

            levelManager.CreateBooster(blocksToDestroy.Count, blockIdx);
            levelManager.ApplyGravity();
        }
        else
        {
            Debug.Log("Not matched");
        }
    }

    private void DestroyBooster(TileEntity blockToDestroy)
    {
        var blocksToDestroy = new List<GameObject>();
        var usedBoosters = new List<GameObject>();
        DestroyBoosterRecursive(blockToDestroy, blocksToDestroy, usedBoosters);
        foreach (var block in blocksToDestroy)
        {
            var idx = levelManager.tileEntities.FindIndex(x => x == block);
            DestroyTileEntity(block.GetComponent<TileEntity>(), idx);
        }

        levelManager.ApplyGravity();
    }

    private void GetMatches(GameObject go, List<GameObject> matchedTiles)
    {
        var level = levelManager.level;
        var idx = levelManager.tileEntities.FindIndex(x => x == go);
        var i = idx % level.width;
        var j = idx / level.width;

        var topTile = new TileDef(i, j - 1);
        var bottomTile = new TileDef(i, j + 1);
        var leftTile = new TileDef(i - 1, j);
        var rightTile = new TileDef(i + 1, j);
        var surroundingTiles = new List<TileDef> { topTile, bottomTile, leftTile, rightTile };

        var hasMatch = false;
        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidTileEntity(surroundingTile))
            {
                var tileIndex = (level.width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.tileEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Block>();
                    if (block != null && block.type == go.GetComponent<Block>().type)
                    {
                        hasMatch = true;
                    }
                }
            }
        }

        if (!hasMatch)
        {
            return;
        }

        if (!matchedTiles.Contains(go))
        {
            matchedTiles.Add(go);
        }

        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidTileEntity(surroundingTile))
            {
                var tileIndex = (level.width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.tileEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Block>();
                    if (block != null && block.type == go.GetComponent<Block>().type &&
                        !matchedTiles.Contains(tile))
                    {
                        GetMatches(tile, matchedTiles);
                    }
                }
            }
        }
    }

    

    private void DestroyBoosterRecursive(TileEntity blockToDestroy, List<GameObject> blocksToDestroy,
        List<GameObject> usedBoosters)
    {
        var blockIdx = levelManager.tileEntities.FindIndex(x => x == blockToDestroy.gameObject);
        var newBlocksToDestroy = blockToDestroy.GetComponent<Booster>().Resolve(blockIdx);
        usedBoosters.Add(blockToDestroy.gameObject);
        //blockToDestroy.GetComponent<Booster>().ShowFx(gamePools, this, blockIdx);

        foreach (var block in newBlocksToDestroy)
        {
            if (!blocksToDestroy.Contains(block))
            {
                blocksToDestroy.Add(block);
            }
        }
    }

    private int DestroyConnectedStones(int idx)
    {
        var level = levelManager.level;
        var score = 0;

        var i = idx % level.width;
        var j = idx / level.width;

        var topTile = new TileDef(i, j - 1);
        var bottomTile = new TileDef(i, j + 1);
        var leftTile = new TileDef(i - 1, j);
        var rightTile = new TileDef(i + 1, j);
        var surroundingTiles = new List<TileDef> { topTile, bottomTile, leftTile, rightTile };
        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidTileEntity(surroundingTile))
            {
                var tileIndex = (level.width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.tileEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Block>();
                    if (block != null && (block.type == BlockType.Stone || block.type == BlockType.Box))
                    {
                        DestroyTileEntity(block, tileIndex);
                    }
                }
            }
        }

        return score;
    }

    private void DestroyTileEntity(TileEntity tileEntity, int tileIndex)
    {
        var block = tileEntity.GetComponent<Block>();
        if (block != null)
        {
            //collect blocks
        }
        tileEntity.Explode();
        levelManager.tileEntities[tileIndex] = null;
        if (IsBoosterBlock(tileEntity))
        {
            DestroyBooster(tileEntity);
        }

        tileEntity.GetComponent<PooledObject>().pool.ReturnObject(tileEntity.gameObject);

    }
    

    private bool IsValidTileEntity(TileDef tileEntity)
    {
        var level = levelManager.level;
        return tileEntity.x >= 0 && tileEntity.x < level.width &&
               tileEntity.y >= 0 && tileEntity.y < level.height;
    }


}