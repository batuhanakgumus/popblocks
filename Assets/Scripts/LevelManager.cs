using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FullSerializer;
using DG.Tweening;


public class LevelManager : MonoBehaviour
{
    public Level level;
    public List<GameObject> tileEntities = new List<GameObject>();
    public Camera mainCamera;
    public GamePools gamePools;
    private float blockWidth;
    private float blockHeight;
    private double horizontalSpacing = -0.02;
    private double verticalSpacing = -0.02;
    private int generatedCollectables;
    public Transform levelLocation;
    [HideInInspector]
    public List<Vector2> tilePositions = new List<Vector2>();
    private int currentLevel=1;
    private float blockFallSpeed = 0.3f;
    public static LevelManager instance;
    private Vector3 lastClickedPoint;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var serializer = new fsSerializer();
        level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + 1);
        CreateBackgroundTiles();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Block"))
            {
                var hitBlock = hit.collider.gameObject.GetComponent<TileEntity>();
                lastClickedPoint = hit.transform.position;
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


    private void DestroyBooster(TileEntity blockToDestroy)
    {
        var score = 0;

        var blocksToDestroy = new List<GameObject>();
        var usedBoosters = new List<GameObject>();
        DestroyBoosterRecursive(blockToDestroy, blocksToDestroy, usedBoosters);

        foreach (var block in blocksToDestroy)
        {
            var idx = tileEntities.FindIndex(x => x == block);
            score += DestroyTileEntity(block.GetComponent<TileEntity>(), idx, false);
        }


        ApplyGravity();
    }
    private void DestroyBoosterRecursive(TileEntity blockToDestroy, List<GameObject> blocksToDestroy, List<GameObject> usedBoosters)
    {
        var blockIdx = tileEntities.FindIndex(x => x == blockToDestroy.gameObject);
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


    private bool IsBoosterBlock(TileEntity tileEntity)
    {
        return tileEntity is Booster;
    }
    
    private void DestroyBlock(TileEntity blockToDestroy)
    {
        var blockIdx = tileEntities.FindIndex(x => x == blockToDestroy.gameObject);
        var blocksToDestroy = new List<GameObject>();
        GetMatches(blockToDestroy.gameObject, blocksToDestroy);
        var score = 0;
        if (blocksToDestroy.Count > 0)
        {
            foreach (var block in blocksToDestroy)
            {
                var idx = tileEntities.FindIndex(x => x == block);
                score += DestroyConnectedStones(idx);
                score += DestroyTileEntity(block.GetComponent<TileEntity>(), idx);
            }
            CreateBooster(blocksToDestroy.Count, blockIdx);
            ApplyGravity();
        }
        else
        {
            Debug.Log("Not matched");

        }
    }
    
    private int DestroyConnectedStones(int idx)
    {
        var score = 0;

        var i = idx % level.width;
        var j = idx / level.width;

        var topTile = new TileDef(i, j - 1);
        var bottomTile = new TileDef(i, j + 1);
        var leftTile = new TileDef(i - 1, j);
        var rightTile = new TileDef(i + 1, j);
        var surroundingTiles = new List<TileDef> {topTile, bottomTile, leftTile, rightTile};
        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidTileEntity(surroundingTile))
            {
                var tileIndex = (level.width * surroundingTile.y) + surroundingTile.x;
                var tile = tileEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Block>();
                    if (block != null && (block.type == BlockType.Stone || block.type == BlockType.Box))
                    {
                        score += DestroyTileEntity(block, tileIndex);
                    }
                }
            }
        }

        return score;
    }
    
    private int DestroyTileEntity(TileEntity tileEntity, int tileIndex, bool playSound = true)
    {
        var block = tileEntity.GetComponent<Block>();
        if (block != null)
        {
            //collect blocks
        }
        tileEntity.Explode();
        tileEntities[tileIndex] = null;
        tileEntity.GetComponent<PooledObject>().pool.ReturnObject(tileEntity.gameObject);
        return 10;
    }
    private void GetMatches(GameObject go, List<GameObject> matchedTiles)
        {
            var idx = tileEntities.FindIndex(x => x == go);
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
                    var tile = tileEntities[tileIndex];
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
                    var tile = tileEntities[tileIndex];
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
    
    private bool IsColorBlock(TileEntity tileEntity)
    {
        var block = tileEntity as Block;
        return block != null &&
               (block.type == BlockType.Block1 ||
                block.type == BlockType.Block2 ||
                block.type == BlockType.Block3 ||
                block.type == BlockType.Block4
               );
    }
    private bool IsValidTileEntity(TileDef tileEntity)
    {
        return tileEntity.x >= 0 && tileEntity.x < level.width &&
               tileEntity.y >= 0 && tileEntity.y < level.height;
    }


    private GameObject CreateBlock(GameObject go)
    {
        go.GetComponent<TileEntity>().Spawn();
        return go;
    }


    private void CreateBackgroundTiles()
    {
        for (var j = 0; j < level.height; j++)
        {
            for (var i = 0; i < level.width; i++)
            {
                var tileIndex = i + (j * level.width);
                var tileToGet = gamePools.GetTileEntity(level, level.tiles[tileIndex]);
                var tile = CreateBlock(tileToGet.gameObject);
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                blockWidth = spriteRenderer.bounds.size.x;
                blockHeight = spriteRenderer.bounds.size.y;
                tile.transform.position = new Vector2((float)(i * (blockWidth + horizontalSpacing)),
                    (float)(-j * (blockHeight + verticalSpacing)));
                tileEntities.Add(tile);
                spriteRenderer.sortingOrder = level.height - j;
            }
        }

        var totalWidth = (level.width - 1) * (blockWidth + horizontalSpacing);
        var totalHeight = (level.height - 1) * (blockHeight + verticalSpacing);
        foreach (var block in tileEntities)
        {
            var newPos = block.transform.position;
            newPos.x -= (float)totalWidth / 2;
            newPos.y += (float)totalHeight / 2;
            newPos.y += levelLocation.position.y;
            block.transform.position = newPos;
            tilePositions.Add(newPos);
        }
        

        var zoomLevel = 1.8f;
        mainCamera.orthographicSize = (float)((totalWidth * zoomLevel) * (Screen.height / (float)Screen.width) * 0.5f);

        var backgroundTiles = new GameObject("BackgroundTiles");
        for (var j = 0; j < level.height; j++)
        {
            for (var i = 0; i < level.width; i++)
            {
                var tileIndex = i + (j * level.width);

                var go = new GameObject("Background");
                go.transform.parent = backgroundTiles.transform;
                var sprite = go.AddComponent<SpriteRenderer>();
                sprite.color = Color.black;
                sprite.sortingLayerName = "Game";
                sprite.sortingOrder = -2;
                sprite.transform.position = tileEntities[tileIndex].transform.position;
            }
        }
    }
    
    private void ApplyGravity()
        {
            for (var i = 0; i < level.width; i++)
            {
                for (var j = level.height - 1; j >= 0; j--)
                {
                    var tileIndex = i + (j * level.width);
                    if (tileEntities[tileIndex] == null ||
                        IsEmptyBlock(tileEntities[tileIndex].GetComponent<TileEntity>()) ||
                        IsStoneBlock(tileEntities[tileIndex].GetComponent<TileEntity>()))
                    {
                        continue;
                    }

                    // Find bottom.
                    var bottom = -1;
                    for (var k = j; k < level.height; k++)
                    {
                        var idx = i + (k * level.width);
                        if (tileEntities[idx] == null)
                        {
                            bottom = k;
                        }
                        else
                        {
                            var block = tileEntities[idx].GetComponent<Block>();
                            if (block != null && block.type == BlockType.Stone)
                            {
                                break;
                            }
                        }
                    }

                    if (bottom != -1)
                    {
                        var tile = tileEntities[tileIndex];
                        if (tile != null)
                        {
                            var numTilesToFall = bottom - j;
                            tileEntities[tileIndex + (numTilesToFall * level.width)] = tileEntities[tileIndex];
                            var tween = tile.transform.DOMove(
                                tilePositions[tileIndex + level.width * numTilesToFall],
                                blockFallSpeed).SetEase(Ease.InQuad);
                            tileEntities[tileIndex] = null;
                        }
                    }
                }
            }

            for (var i = 0; i < level.width; i++)
            {
                var numEmpties = 0;
                for (var j = 0; j < level.height; j++)
                {
                    var idx = i + (j * level.width);
                    if (tileEntities[idx] == null)
                    {
                        numEmpties += 1;
                    }
                    else
                    {
                        var block = tileEntities[idx].GetComponent<Block>();
                        if (block != null && block.type == BlockType.Stone)
                        {
                            break;
                        }
                    }
                }

                if (numEmpties > 0)
                {
                    for (var j = 0; j < level.height; j++)
                    {
                        var tileIndex = i + (j * level.width);
                        var isEmptyTile = false;
                        var isStoneTile = false;
                        if (tileEntities[tileIndex] != null)
                        {
                            var blockTile = tileEntities[tileIndex].GetComponent<Block>();
                            if (blockTile != null)
                            {
                                isEmptyTile = blockTile.type == BlockType.Empty;
                                isStoneTile = blockTile.type == BlockType.Stone;
                            }
                            if (isStoneTile)
                            {
                                break;
                            }
                        }
                        if (tileEntities[tileIndex] == null && !isEmptyTile)
                        {
                            var tile = CreateNewBlock();
                            var pos = tilePositions[i];
                            pos.y = (float)(tilePositions[i].y + (numEmpties * (blockHeight + verticalSpacing)));
                            --numEmpties;
                            tile.transform.position = pos;
                            var tween = tile.transform.DOMove(
                                tilePositions[tileIndex],
                                blockFallSpeed).SetEase(Ease.InQuad);
                            tileEntities[tileIndex] = tile;
                        }
                        if (tileEntities[tileIndex] != null)
                        {
                            tileEntities[tileIndex].GetComponent<SpriteRenderer>().sortingOrder = level.height - j;
                        }
                    }
                }
            }
        }
    
    private GameObject CreateNewBlock()
    {
        return CreateBlock(gamePools.GetTileEntity(level, new BlockTile { type = BlockType.RandomBlock }).gameObject);
    }
    private bool IsEmptyBlock(TileEntity tileEntity)
    {
        var block = tileEntity as Block;
        return block != null && block.type == BlockType.Empty;
    }
    
    private bool IsStoneBlock(TileEntity tileEntity)
    {
        var block = tileEntity as Block;
        return block != null && block.type == BlockType.Stone;
    }
    
    private void CreateBooster(int numMatchedBlocks, int blockIdx)
    {
        var eligibleBoosters = new List<KeyValuePair<BoosterType, int>>();
        foreach (var pair in boosterNeededMatches)
        {
            if (numMatchedBlocks >= pair.Value)
            {
                eligibleBoosters.Add(pair);
            }
        }

        if (eligibleBoosters.Count > 0)
        {
            var max = eligibleBoosters.Max(x => x.Value);
            eligibleBoosters.RemoveAll(x => x.Value != max);
            var idx = UnityEngine.Random.Range(0, eligibleBoosters.Count);
            var booster = eligibleBoosters[idx];
            CreateBooster(GetBoosterPool(booster.Key).GetObject(), blockIdx);
        }
    }
        
    /// <summary>
    /// Creates a booster at the specified index.
    /// </summary>
    /// <param name="booster">The booster to create.</param>
    /// <param name="blockIdx">The index at which to create the booster.</param>
    private void CreateBooster(GameObject booster, int blockIdx)
    {
        booster.transform.position = tilePositions[blockIdx];
        tileEntities[blockIdx] = booster;
        var j = blockIdx / level.height;
        booster.GetComponent<SpriteRenderer>().sortingOrder = level.height - j;
    }
    
    private ObjectPool GetBoosterPool(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.HorizontalRocket:
                return gamePools.horizontalRocketPool;

            case BoosterType.VerticalRocket:
                return gamePools.verticalRocketPool;

            case BoosterType.TNT:
                return gamePools.tntPool;
        }
        return null;
    }
    public Dictionary<BoosterType, int> boosterNeededMatches = new Dictionary<BoosterType, int>();

    public LevelManager()
    {
        boosterNeededMatches.Add(BoosterType.HorizontalRocket, 3);
        boosterNeededMatches.Add(BoosterType.VerticalRocket, 3);
        boosterNeededMatches.Add(BoosterType.TNT, 5);
    }
}

public struct TileDef
{
    public readonly int x;
    public readonly int y;

    public TileDef(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
