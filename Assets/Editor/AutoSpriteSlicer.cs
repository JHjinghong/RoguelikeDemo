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
        GUILayout.Label("�Զ���Ƭ����", EditorStyles.boldLabel);

        // �ز�ѡ��
        texture = (Texture2D)EditorGUILayout.ObjectField("Ŀ���ز�", texture, typeof(Texture2D), false);

        // ��Ƭ����
        cellSizeX = EditorGUILayout.IntField("��Ԫ����", cellSizeX);
        cellSizeY = EditorGUILayout.IntField("��Ԫ��߶�", cellSizeY);

        // ��������
        namingPattern = EditorGUILayout.TextField("��������", namingPattern);
        EditorGUILayout.HelpBox("���ñ�����\n{ACTION} - ��������\n{INDEX} - ֡���", MessageType.Info);

        if (GUILayout.Button("ִ����Ƭ") && texture != null)
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
            Debug.LogError("�������ļ�: " + path);
            return;
        }

        // ��������
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.filterMode = FilterMode.Point;
        ti.spritePixelsPerUnit = cellSizeX;

        // ����������
        int width = texture.width;
        int height = texture.height;
        int cols = width / cellSizeX;
        int rows = height / cellSizeY;

        // ������ƬԪ����
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

        // Ӧ������
        ti.spritesheet = slices;
        ti.SaveAndReimport();
        Debug.Log($"�ɹ���Ƭ: {slices.Length} ֡ | ·��: {path}");
    }
}