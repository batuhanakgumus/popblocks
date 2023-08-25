using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public static LevelManager Instance;
    public GamePools gamePools;
    public GameMechanic gameMechanic;


    [Header("LevelSettings")]
    public Transform levelLocation;
    public Level level;
    public List<GameObject> cellEntities = new List<GameObject>();
    public List<Vector2> tilePositions = new List<Vector2>();
    private float blockWidth;
    private float blockHeight;
    private double horizontalSpacing = -0.02;
    private double verticalSpacing = -0.02;
    private int currentLevel => PlayerPrefs.GetInt("currentLevel");
    public Dictionary<BoosterType, int> boosterNeededMatches = new Dictionary<BoosterType, int>();
    private float blockFallSpeed = 0.3f;
    private  float _bugFreeDelay = .2f;
    private  float _levelResetDelay = 1f;




    [Header("Level Passing")] public Transform goalGrid;
    public TextMeshProUGUI moveText;
    public int remainingMove;
    public int boxGoalCount;
    public int stoneGoalCount;
    public int vaseGoalCount;
    public List<Goal> goals;
    public Goal goalPrefab;
    public GameObject winPanel;
    public bool isGameBusy;

    public LevelManager()
    {
        boosterNeededMatches.Add(BoosterType.HorizontalRocket, 3);
        boosterNeededMatches.Add(BoosterType.VerticalRocket, 3);
        boosterNeededMatches.Add(BoosterType.TNT, 5);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
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
            LevelPassed();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Lose();
        }
        
    }

    private GameObject CreateBlock(GameObject go)
    {
        go.GetComponent<Cell>().Spawn();
        return go;
    }


    private void GenerateLevel()
    {
        ClearLevel();
        level = JsonConvert.DeserializeObject<Level>(Resources.Load<TextAsset>("Levels/level_" + currentLevel).text);
        remainingMove = level.move_count;
        moveText.text = remainingMove.ToString();
        int index = 0;
        for (var j =  level.grid_height-1; j >= 0; j--)
        {
            for (var i = 0; i < level.grid_width; i++)
            {
                index++;
                var tileIndex = index;
                var tileToGet = gamePools.GetCellWithString(level, level.grid[tileIndex-1]);
                var tile = CreateBlock(tileToGet.gameObject);
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                var bounds = spriteRenderer.bounds;
                blockWidth = bounds.size.x;
                blockHeight = bounds.size.y;
                tile.transform.position = new Vector2((float)(i * (blockWidth + horizontalSpacing)),
                    (float)(-j * (blockHeight + verticalSpacing)));
                cellEntities.Add(tile);
                spriteRenderer.sortingOrder = level.grid_height - j;
            }
        }

        var totalWidth = (level.grid_width - 1) * (blockWidth + horizontalSpacing);
        var totalHeight = (level.grid_height - 1) * (blockHeight + verticalSpacing);
        foreach (var cell in cellEntities)
        {
            var newPos = cell.transform.position;
            newPos.x -= (float)totalWidth / 2;
            newPos.y += (float)totalHeight / 2;
            newPos.y += levelLocation.position.y;
            cell.transform.position = newPos;
            tilePositions.Add(newPos);
        }
        var zoomLevel = 1.2f;
        mainCamera.orthographicSize = (float)(totalWidth+totalHeight / zoomLevel * Screen.height / Screen.width)/2;
        SetGoal();
        CheckHint();
        cellEntities.Reverse();
        tilePositions.Reverse();
    }


    public GameObject CreateNewBlock()
    {
        return CreateBlock(gamePools.GetCell(level, new CubeTile { type = CubeType.rand }).gameObject);
    }

    private void ClearLevel()
    {
        foreach (var goal in goals)
        {
            Destroy(goal.gameObject);
        }
        gamePools.ResetAllPools();
        cellEntities.Clear();
        tilePositions.Clear();
        goals.Clear();
        
    }
    public void CheckHint()
    {
        var cellsToBeHint = new List<GameObject>();

        foreach (var cell in cellEntities)
        {
            if (cell!=null && cell.TryGetComponent(out Cube cube))
            {
                if (IsColorCube(cube))
                {
                    cube.NoHint();
                    gameMechanic.GetMatches(cell, cellsToBeHint);

                    if (cellsToBeHint.Count >= 5)
                    {
                        foreach (var cellObj in cellsToBeHint)
                        {
                            cellObj.TryGetComponent(out Cube cCube);
                            cCube.GiveTntHint();
                        }
                    }
                    else if (cellsToBeHint.Count > 2 && cellsToBeHint.Count < 5)
                    {
                        foreach (var cellObj in cellsToBeHint)
                        {
                            cellObj.TryGetComponent(out Cube cCube);
                            cCube.GiveRocketHint();
                        }
                    }
                    else
                    {
                        foreach (var cellObj in cellsToBeHint)
                        {
                            cellObj.TryGetComponent(out Cube cCube);
                            cCube.NoHint();
                        }
                    }

                    cellsToBeHint.Clear();
                }
            }
        }
    }

    public void ApplyGravity()
    {
        isGameBusy = true;
        for (var i = 0; i < level.grid_width; i++)
        {
            for (var j = level.grid_height - 1; j >= 0; j--)
            {
                var tileIndex = i + (j * level.grid_width);
                if (cellEntities[tileIndex] == null ||
                    IsEmptyBlock(cellEntities[tileIndex].GetComponent<Cell>()) ||
                    IsStoneBlock(cellEntities[tileIndex].GetComponent<Cell>()))
                {
                    continue;
                }

                var bottom = -1;
                for (var k = j; k < level.grid_height; k++)
                {
                    var idx = i + (k * level.grid_width);
                    if (cellEntities[idx] == null)
                    {
                        bottom = k;
                    }
                    else
                    {
                        var block = cellEntities[idx].GetComponent<Cube>();
                        if (block != null && block.type == CubeType.s)
                        {
                            break;
                        }
                    }
                }

                if (bottom != -1)
                {
                    var tile = cellEntities[tileIndex];
                    if (tile != null)
                    {
                        var numTilesToFall = bottom - j;
                        cellEntities[tileIndex + (numTilesToFall * level.grid_width)] =
                            cellEntities[tileIndex];
                        tile.transform.DOMove(
                            tilePositions[tileIndex + level.grid_width * numTilesToFall],
                            blockFallSpeed).SetEase(Ease.InQuad);
                        cellEntities[tileIndex] = null;
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
                if (cellEntities[idx] == null)
                {
                    numEmpties += 1;
                }
                else
                {
                    var block = cellEntities[idx].GetComponent<Cube>();
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
                    if (cellEntities[tileIndex] != null)
                    {
                        var blockTile = cellEntities[tileIndex].GetComponent<Cube>();
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

                    if (cellEntities[tileIndex] == null && !isEmptyTile)
                    {
                        var tile = CreateNewBlock();
                        var pos = tilePositions[i];
                        pos.y = (float)(tilePositions[i].y +
                                        (numEmpties * (blockHeight + verticalSpacing)));
                        --numEmpties;
                        tile.transform.position = pos;
                        tile.transform.DOMove(
                            tilePositions[tileIndex],
                            blockFallSpeed).SetEase(Ease.InQuad);
                        cellEntities[tileIndex] = tile;
                    }

                    if (cellEntities[tileIndex] != null)
                    {
                        cellEntities[tileIndex].GetComponent<SpriteRenderer>().sortingOrder =
                            level.grid_height - j;
                    }
                }
            }
        }

        DOVirtual.DelayedCall(blockFallSpeed+_bugFreeDelay, () => isGameBusy = false);
        CheckHint();
        
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
        if (remainingMove>0)
        {
            booster.transform.position = tilePositions[cubeId];
            cellEntities[cubeId] = booster;
            var j = cubeId / level.grid_height;
            booster.GetComponent<SpriteRenderer>().sortingOrder = level.grid_height - j;
        }
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

    public void PerformMove()
    {
        remainingMove--;
        moveText.text = remainingMove.ToString();
        if (remainingMove == 0)
        {
            Lose();
        }

        CheckGoalCount();
    }

    public void SetGoal()
    {
        boxGoalCount = 0;
        stoneGoalCount = 0;
        vaseGoalCount = 0;
        for (int i = 0; i < level.grid_height * level.grid_width; i++)
        {
            if (level.grid[i] == "bo")
            {
                boxGoalCount++;
            }

            if (level.grid[i] == "s")
            {
                stoneGoalCount++;
            }

            if (level.grid[i] == "v")
            {
                vaseGoalCount++;
            }
        }

        if (boxGoalCount > 0)
        {
            var goal = Instantiate(goalPrefab, goalGrid);
            goal.SetGoal(CubeType.bo, boxGoalCount);
            goals.Add(goal);
        }

        if (stoneGoalCount > 0)
        {
            var goal = Instantiate(goalPrefab, goalGrid);
            goal.SetGoal(CubeType.s, stoneGoalCount);
            goals.Add(goal);
        }

        if (vaseGoalCount > 0)
        {
            var goal = Instantiate(goalPrefab, goalGrid);
            goal.SetGoal(CubeType.v, vaseGoalCount);
            goals.Add(goal);
        }
    }

    public void CheckGoalCount()
    {
        foreach (var goal in goals)
        {
            if (goal.type == CubeType.bo)
            {
                goal.SetAmountAndAndCheckGoal(boxGoalCount);
            }

            if (goal.type == CubeType.s)
            {
                goal.SetAmountAndAndCheckGoal(stoneGoalCount);
            }

            if (goal.type == CubeType.v)
            {
                goal.SetAmountAndAndCheckGoal(vaseGoalCount);
            }
        }

        if (stoneGoalCount + vaseGoalCount + boxGoalCount <= 0)
        {
            winPanel.SetActive(true);
            isGameBusy = true;
        }
    }

    private bool IsColorCube(Cell cell)
    {
        var cube = cell as Cube;
        return cube != null &&
               (cube.type == CubeType.b ||
                cube.type == CubeType.r ||
                cube.type == CubeType.g ||
                cube.type == CubeType.y);
    }

    public void LevelPassed()
    {
        PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("currentLevel") + 1);
        SceneManager.LoadScene(0);
    }

    private void Lose()
    {
        isGameBusy = true;
        DOVirtual.DelayedCall(_levelResetDelay, () =>
        {
            GenerateLevel();

        });
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
}