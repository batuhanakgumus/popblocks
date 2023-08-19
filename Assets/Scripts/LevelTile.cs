/// <summary>
/// The base class used for the tiles in the visual editor.
/// </summary>
public class LevelTile
{
    public BlockerType blockerType;
}

/// <summary>
/// The class used for block tiles.
/// </summary>
public class BlockTile : LevelTile
{
    public BlockType type;
}

/// <summary>
/// The class used for booster tiles.
/// </summary>
public class BoosterTile : LevelTile
{
    public BoosterType type;
}