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
            Debug.LogError("DungeonGenerator δ���ã�");
            return;
        }

        DungeonMap map = generator.GenerateDungeon();
        VisualizeDungeon(map);
    }

    // ���ӻ�
    public void VisualizeDungeon(DungeonMap map)
    {
        // ������е�Tilemap
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


