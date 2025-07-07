using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public BSPDungeonGenerator generator;
    public DungeonVisualizer visualizer;

    public int mapWidth = 40;
    public int mapHeight = 40;

    void Start()
    {
        DungeonMap dungeonMap = new DungeonMap(mapWidth, mapHeight);
        generator.map = dungeonMap;
        generator.GenerateDungeon();
        visualizer.VisualizeDungeon(dungeonMap);
    }
}
