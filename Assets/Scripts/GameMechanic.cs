using System.Collections.Generic;
using UnityEngine;

public class GameMechanic : MonoBehaviour
{
    public LevelManager levelManager;
    private Camera _mainCamera;
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !levelManager.isGameBusy)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Cell"))
            {
                var hitBlock = hit.collider.gameObject.GetComponent<Cell>();
                if (IsBoosterBlock(hitBlock))
                {
                    DestroyBooster(hitBlock,true);
                }
                else
                {
                    DestroyBlock(hitBlock);
                }
            }
        }
    }

    private bool IsBoosterBlock(Cell Cell)
    {
        return Cell is Booster;
    }

    private void DestroyBlock(Cell cellToDestroy)
    {
        var cubeId = levelManager.cellEntities.FindIndex(x => x == cellToDestroy.gameObject);
        var cellsToBeDestroyed = new List<GameObject>();
        GetMatches(cellToDestroy.gameObject, cellsToBeDestroyed);
        if (cellsToBeDestroyed.Count > 0)
        {
            foreach (var block in cellsToBeDestroyed)
            {
                var idx = levelManager.cellEntities.FindIndex(x => x == block);
                DestroyConnectedObstacles(idx);
                DestroyCell(block.GetComponent<Cell>(), idx);
            }
            levelManager.PerformMove();
            levelManager.CreateBooster(cellsToBeDestroyed.Count, cubeId);
            levelManager.ApplyGravity();
        }
    }

    public void GetMatches(GameObject go, List<GameObject> matchedTiles)
    {
        var level = levelManager.level;
        var idx = levelManager.cellEntities.FindIndex(x => x == go);
        var i = idx % level.grid_width;
        var j = idx / level.grid_width;

        var topTile = new TileDef(i, j - 1);
        var bottomTile = new TileDef(i, j + 1);
        var leftTile = new TileDef(i - 1, j);
        var rightTile = new TileDef(i + 1, j);
        var surroundingTiles = new List<TileDef> { topTile, bottomTile, leftTile, rightTile };

        var hasMatch = false;
        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidCell(surroundingTile))
            {
                var tileIndex = (level.grid_width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.cellEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Cube>();
                    if (block != null && block.type == go.GetComponent<Cube>().type)
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
            if (IsValidCell(surroundingTile))
            {
                var tileIndex = (level.grid_width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.cellEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Cube>();
                    if (block != null && block.type == go.GetComponent<Cube>().type &&
                        !matchedTiles.Contains(tile))
                    {
                        GetMatches(tile, matchedTiles);
                    }
                }
            }
        }
    }

    private void DestroyBooster(Cell cellToDestroy, bool isApplyGravity)
    {
        var cellsToBeDestroyed = new List<GameObject>();
        var usedBoosters = new List<GameObject>();
        DestroyConnectedBoosters(cellToDestroy, cellsToBeDestroyed, usedBoosters);
        foreach (var block in cellsToBeDestroyed)
        {
            var idx = levelManager.cellEntities.FindIndex(x => x == block);
            DestroyCell(block.GetComponent<Cell>(), idx);
        }
        if (isApplyGravity)
        {
            levelManager.ApplyGravity();
        }
        levelManager.PerformMove();
    }

    private void DestroyConnectedBoosters(Cell cellToDestroy, List<GameObject> cellsToBeDestroyed,
        List<GameObject> usedBoosters)
    {
        var cubeId = levelManager.cellEntities.FindIndex(x => x == cellToDestroy.gameObject);
        var newcellsToBeDestroyed = cellToDestroy.GetComponent<Booster>().Resolve(cubeId);
        usedBoosters.Add(cellToDestroy.gameObject);
        //cellToDestroy.GetComponent<Booster>().ShowFx(gamePools, this, cubeId);
        foreach (var block in newcellsToBeDestroyed)
        {
            if (block.GetComponent<Booster>() != null && !usedBoosters.Contains(block))
            {
                usedBoosters.Add(block);
                DestroyConnectedBoosters(block.GetComponent<Cell>(), cellsToBeDestroyed, usedBoosters);
            }
        }
        foreach (var block in newcellsToBeDestroyed)
        {
            if (!cellsToBeDestroyed.Contains(block))
            {
                cellsToBeDestroyed.Add(block);
            }
        }
    }


    private void DestroyConnectedObstacles(int idx)
    {
        var level = levelManager.level;

        var i = idx % level.grid_width;
        var j = idx / level.grid_width;

        var topTile = new TileDef(i, j - 1);
        var bottomTile = new TileDef(i, j + 1);
        var leftTile = new TileDef(i - 1, j);
        var rightTile = new TileDef(i + 1, j);
        var surroundingTiles = new List<TileDef> { topTile, bottomTile, leftTile, rightTile };
        foreach (var surroundingTile in surroundingTiles)
        {
            if (IsValidCell(surroundingTile))
            {
                var tileIndex = (level.grid_width * surroundingTile.y) + surroundingTile.x;
                var tile = levelManager.cellEntities[tileIndex];
                if (tile != null)
                {
                    var block = tile.GetComponent<Cube>();
                    if (block != null && (block.type == CubeType.v || block.type == CubeType.bo))
                    {
                        DestroyCell(block, tileIndex);
                    }
                }
            }
        }
    }

    private void DestroyCell(Cell Cell, int tileIndex)
    {
        Cell.TryGetComponent(out Cube cube);
        if (cube != null)
        {
            if (cube.type == CubeType.v)
            {
                cube.TryGetComponent(out Vase vase);
                if (!vase.isDamaged)
                {
                    vase.TakeDamage();
                    return;
                }
                levelManager.vaseGoalCount--;
            }
            if (cube.type == CubeType.s)
            {
                levelManager.stoneGoalCount--;
            }
            if (cube.type == CubeType.bo)
            {
                levelManager.boxGoalCount--;
            }
        }
        Cell.Explode();
        levelManager.cellEntities[tileIndex] = null;
        Cell.GetComponent<PooledObject>().pool.ReturnObject(Cell.gameObject);

    }


    private bool IsValidCell(TileDef Cell)
    {
        var level = levelManager.level;
        return Cell.x >= 0 && Cell.x < level.grid_width &&
               Cell.y >= 0 && Cell.y < level.grid_height;
    }


}