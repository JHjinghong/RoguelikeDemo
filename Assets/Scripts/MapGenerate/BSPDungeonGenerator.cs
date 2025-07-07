using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    public DungeonMap map;
    public int mapWidth = 40;
    public int mapHeight = 40;
    public int minRoomSize = 5;
    public int maxRoomSize = 10;
    public int roomCountMin = 5;
    public int roomCountMax = 8;

    private List<RectInt> rooms;

    public DungeonMap GenerateDungeon()
    {
        DungeonMap map = new DungeonMap(mapWidth, mapHeight);
        rooms = new List<RectInt>();
        int attempts = 0;

        // 随机生成不重叠的房间
        while (rooms.Count < Random.Range(roomCountMin, roomCountMax + 1) && attempts < 100)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize + 1);
            int x = Random.Range(1, mapWidth - roomWidth - 1);
            int y = Random.Range(1, mapHeight - roomHeight - 1);

            RectInt newRoom = new RectInt(x, y, roomWidth, roomHeight);

            bool overlaps = false;
            foreach (var room in rooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);
                CarveRoom(map, newRoom);
            }

            attempts++;
        }

        // 连通房间
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int a = Vector2Int.RoundToInt(rooms[i - 1].center);
            Vector2Int b = Vector2Int.RoundToInt(rooms[i].center);
            CarveCorridor(map, a, b);
        }

        AddWalls(map);
        return map;
    }

    // 创建房间
    private void CarveRoom(DungeonMap map, RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                map.map[x, y] = TileType.Floor;
            }
        }
    }

    // 创建走廊
    private void CarveCorridor(DungeonMap map, Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;

        while (current.x != to.x)
        {
            CarveWideTile(map, current);
            current.x += (to.x > current.x) ? 1 : -1;
        }

        while (current.y != to.y)
        {
            CarveWideTile(map, current);
            current.y += (to.y > current.y) ? 1 : -1;
        }

        CarveWideTile(map, to);
    }

    // 扩展走廊宽度
    private void CarveWideTile(DungeonMap map, Vector2Int pos)
    {
        for (int dx = 0; dx <= 1; dx++)   // 控制走廊宽度（2格）
        {
            for (int dy = 0; dy <= 1; dy++) // 控制走廊高度（2格）
            {
                int nx = pos.x + dx;
                int ny = pos.y + dy;

                if (nx >= 0 && nx < map.Width && ny >= 0 && ny < map.Height)
                {
                    map.map[nx, ny] = TileType.Floor;
                }
            }
        }
    }

    // 添加墙体
    private void AddWalls(DungeonMap map)
    {
        for (int x = 1; x < map.Width - 1; x++)
        {
            for (int y = 1; y < map.Height - 1; y++)
            {
                if (map.map[x, y] == TileType.Floor)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (map.map[nx, ny] == TileType.Empty)
                            {
                                map.map[nx, ny] = TileType.Wall;
                            }
                        }
                    }
                }
            }
        }
    }
}


