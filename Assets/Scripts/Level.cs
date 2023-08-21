using System.Collections.Generic;


/// <summary>
/// This class stores the settings of a game level.
/// </summary>
public class Level
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public List<string> grid = new List<string>();
    public int move_count;
    public List<ColorBlockType> availableColors = new List<ColorBlockType>();
}