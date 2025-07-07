using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonVisualizer : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    public BSPDungeonGenerator generator;

    void Start()
    {
        if (generator == null)
        {
            Debug.LogError("DungeonGenerator 未设置！");
            return;
        }

        DungeonMap map = generator.GenerateDungeon();
        VisualizeDungeon(map);
    }

    // 可视化
    public void VisualizeDungeon(DungeonMap map)
    {
        // 清除现有的Tilemap
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileType type = map.map[x, y];

                switch (type)
                {
                    case TileType.Floor:
                        floorTilemap.SetTile(pos, floorTile);
                        break;

                    case TileType.Wall:
                        wallTilemap.SetTile(pos, wallTile);
                        break;
                }
            }
        }
    }
}


