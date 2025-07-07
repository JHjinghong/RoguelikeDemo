using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap
{
    public int Width;
    public int Height;
    public TileType[,] map;

    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;
        map = new TileType[width, height];

        // Ä¬ÈÏÌî³äÎªEmpty
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = TileType.Empty;
            }
        }
    }
}

