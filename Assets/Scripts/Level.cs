using System.Collections.Generic;

public enum LimitType
{
    Moves,
    Time
}

/// <summary>
/// This class stores the settings of a game level.
/// </summary>
public class Level
{
    public int id;

    public int width;
    public int height;
    public List<LevelTile> tiles = new List<LevelTile>();

    public LimitType limitType;
    public int limit;
    public int penalty;

    public List<Goal> goals = new List<Goal>();
    public List<ColorBlockType> availableColors = new List<ColorBlockType>();

    public int score1;
    public int score2;
    public int score3;

    public bool awardBoostersWithRemainingMoves;
    public BoosterType awardedBoosterType;

    public int collectableChance;

    public Dictionary<BoosterType, bool> availableBoosters = new Dictionary<BoosterType, bool>();
}