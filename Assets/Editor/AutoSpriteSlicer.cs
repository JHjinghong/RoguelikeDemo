using UnityEditor;
using UnityEngine;
using System.IO;

public class AutoSpriteSlicer : EditorWindow
{
    private Texture2D texture;
    private int cellSizeX = 32;
    private int cellSizeY = 32;
    private string namingPattern = "character_{ACTION}_{INDEX}";

    [MenuItem("Tools/Sprite/Auto Slice Sprites")]
    public static void ShowWindow()
    {
        GetWindow<AutoSpriteSlicer>("Sprite Slicer");
    }

    void OnGUI()
    {
        GUILayout.Label("自动切片设置", EditorStyles.boldLabel);

        // 素材选择
        texture = (Texture2D)EditorGUILayout.ObjectField("目标素材", texture, typeof(Texture2D), false);

        // 切片参数
        cellSizeX = EditorGUILayout.IntField("单元格宽度", cellSizeX);
        cellSizeY = EditorGUILayout.IntField("单元格高度", cellSizeY);

        // 命名规则
        namingPattern = EditorGUILayout.TextField("命名规则", namingPattern);
        EditorGUILayout.HelpBox("可用变量：\n{ACTION} - 动作名称\n{INDEX} - 帧序号", MessageType.Info);

        if (GUILayout.Button("执行切片") && texture != null)
        {
            SliceTexture();
        }
    }

    void SliceTexture()
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        if (ti == null)
        {
            Debug.LogError("非纹理文件: " + path);
            return;
        }

        // 基础设置
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.filterMode = FilterMode.Point;
        ti.spritePixelsPerUnit = cellSizeX;

        // 计算行列数
        int width = texture.width;
        int height = texture.height;
        int cols = width / cellSizeX;
        int rows = height / cellSizeY;

        // 生成切片元数据
        SpriteMetaData[] slices = new SpriteMetaData[rows * cols];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = y * cols + x;
                slices[index] = new SpriteMetaData
                {
                    rect = new Rect(x * cellSizeX, height - (y + 1) * cellSizeY, cellSizeX, cellSizeY),
                    name = namingPattern
                        .Replace("{ACTION}", Path.GetFileNameWithoutExtension(path))
                        .Replace("{INDEX}", index.ToString("D2"))
                };
            }
        }

        // 应用设置
        ti.spritesheet = slices;
        ti.SaveAndReimport();
        Debug.Log($"成功切片: {slices.Length} 帧 | 路径: {path}");
    }
}