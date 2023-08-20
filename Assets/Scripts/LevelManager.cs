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
    [HideInInspector] public List<Vector2> tilePositions = new List<Vector2>();
    private float blockFallSpeed = 0.3f;
    public static LevelManager instance;

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


        var zoomLevel = 1.4f;
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


    public GameObject CreateNewBlock()
    {
        return CreateBlock(gamePools.GetTileEntity(level, new BlockTile { type = BlockType.RandomBlock }).gameObject);
    }

    public void ApplyGravity()
    {
        DOVirtual.DelayedCall(0.5f, () =>
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
                            tileEntities[tileIndex + (numTilesToFall * level.width)] =
                                tileEntities[tileIndex];
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
                            pos.y = (float)(tilePositions[i].y +
                                            (numEmpties * (blockHeight + verticalSpacing)));
                            --numEmpties;
                            tile.transform.position = pos;
                            var tween = tile.transform.DOMove(
                                tilePositions[tileIndex],
                                blockFallSpeed).SetEase(Ease.InQuad);
                            tileEntities[tileIndex] = tile;
                        }

                        if (tileEntities[tileIndex] != null)
                        {
                            tileEntities[tileIndex].GetComponent<SpriteRenderer>().sortingOrder =
                                level.height - j;
                        }
                    }
                }
            }
        });
    }

    public void CreateBooster(int numMatchedBlocks, int blockIdx)
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