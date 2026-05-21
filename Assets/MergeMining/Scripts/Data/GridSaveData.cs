using System;
using System.Collections.Generic;

[Serializable]
public class GridPickaxeEntry
{
    public int row;
    public int col;
    public int level;
}

[Serializable]
public class GridSaveData
{
    public List<GridPickaxeEntry> entries = new List<GridPickaxeEntry>();
}
