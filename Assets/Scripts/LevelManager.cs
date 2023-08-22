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
    public Transform levelLocation;
    public List<Vector2> tilePositions = new List<Vector2>();
    private float blockFallSpeed = 0.3f;
    public static LevelManager instance;
    private int currentLevel = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateLevel();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentLevel++;
            GenerateLevel();
        }
    }

    private GameObject CreateBlock(GameObject go)
    {
        go.GetComponent<Cell>().Spawn();
        return go;
    }


    private void GenerateLevel()
    {
        var serializer = new fsSerializer();
        level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/level_" + currentLevel);
        level.grid.Reverse();
        foreach (var entity in tileEntities)
        {
            entity.GetComponent<PooledObject>().pool.ReturnObject(entity.gameObject);   
        }
        tileEntities.Clear();
        tilePositions.Clear();
        for (var j = 0; j < level.grid_height; j++)
        {
            for (var i = 0; i < level.grid_width; i++)
            {
                var tileIndex = i + (j * level.grid_width);
                var tileToGet = gamePools.GetCellWithString(level, level.grid[tileIndex]);
                var tile = CreateBlock(tileToGet.gameObject);
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                blockWidth = spriteRenderer.bounds.size.x;
                blockHeight = spriteRenderer.bounds.size.y;
                tile.transform.position = new Vector2((float)(i * (blockWidth + horizontalSpacing)),
                    (float)(-j * (blockHeight + verticalSpacing)));
                tileEntities.Add(tile);
                spriteRenderer.sortingOrder = level.grid_height - j;
            }
        }

        var totalWidth = (level.grid_width - 1) * (blockWidth + horizontalSpacing);
        var totalHeight = (level.grid_height - 1) * (blockHeight + verticalSpacing);
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
        mainCamera.transform.LookAt(gamePools.transform);
    }


    public GameObject CreateNewBlock()
    {
        return CreateBlock(gamePools.GetCell(level, new BlockTile { type = CubeType.rand }).gameObject);
    }

    public void ApplyGravity()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            for (var i = 0; i < level.grid_width; i++)
            {
                for (var j= level.grid_height-1 ; j >0 ; j--)
                {
                    var tileIndex = i + (j * level.grid_width);
                    if (tileEntities[tileIndex] == null ||
                        IsEmptyBlock(tileEntities[tileIndex].GetComponent<Cell>()) ||
                        IsStoneBlock(tileEntities[tileIndex].GetComponent<Cell>()))
                    {
                        continue;
                    }

                    var bottom = -1;
                    for (var k = j; k < level.grid_height; k++)
                    {
                        var idx = i + (k * level.grid_width);
                        if (tileEntities[idx] == null)
                        {
                            bottom = k;
                        }
                        else
                        {
                            var block = tileEntities[idx].GetComponent<Cube>();
                            if (block != null && block.type == CubeType.s)
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
                            tileEntities[tileIndex + (numTilesToFall * level.grid_width)] =
                                tileEntities[tileIndex];
                            var tween = tile.transform.DOMove(
                                tilePositions[tileIndex + level.grid_width * numTilesToFall],
                                blockFallSpeed).SetEase(Ease.InQuad);
                            tileEntities[tileIndex] = null;
                        }
                    }
                }
            }

            for (var i = 0; i < level.grid_width; i++)
            {
                var numEmpties = 0;
                for (var j = 0; j < level.grid_height; j++)
                {
                    var idx = i + (j * level.grid_width);
                    if (tileEntities[idx] == null)
                    {
                        numEmpties += 1;
                    }
                    else
                    {
                        var block = tileEntities[idx].GetComponent<Cube>();
                        if (block != null && block.type == CubeType.s)
                        {
                            break;
                        }
                    }
                }

                if (numEmpties > 0)
                {
                    for (var j = 0; j < level.grid_height; j++)
                    {
                        var tileIndex = i + (j * level.grid_width);
                        var isEmptyTile = false;
                        var isStoneTile = false;
                        if (tileEntities[tileIndex] != null)
                        {
                            var blockTile = tileEntities[tileIndex].GetComponent<Cube>();
                            if (blockTile != null)
                            {
                                isEmptyTile = blockTile.type == CubeType.Empty;
                                isStoneTile = blockTile.type == CubeType.s;
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
                                level.grid_height - j;
                        }
                    }
                }
            }
        });
    }

    public void CreateBooster(int numMatchedBlocks, int cubeId)
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
            CreateBooster(GetBoosterPool(booster.Key).GetObject(), cubeId);
        }
    }
    
    private void CreateBooster(GameObject booster, int cubeId)
    {
        booster.transform.position = tilePositions[cubeId];
        tileEntities[cubeId] = booster;
        var j = cubeId / level.grid_height;
        booster.GetComponent<SpriteRenderer>().sortingOrder = level.grid_height - j;
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
    private bool IsEmptyBlock(Cell Cell)
    {
        var block = Cell as Cube;
        return block != null && block.type == CubeType.Empty;
    }

    private bool IsStoneBlock(Cell Cell)
    {
        var block = Cell as Cube;
        return block != null && block.type == CubeType.s;
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