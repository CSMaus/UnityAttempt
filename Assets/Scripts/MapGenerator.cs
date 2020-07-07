using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //public sealed struct Color : System.ValueType { };

    public enum DrawMode {NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    public const int mapCunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;

    public float noiseScale;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeighCurve;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapCunkSize, mapCunkSize, seed,  noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapCunkSize * mapCunkSize]; //ЦВЕТ
        for (int y = 0; y < mapCunkSize; y++)
        {
            for (int x = 0; x < mapCunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapCunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>(); //отображение на экране
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapCunkSize, mapCunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeighCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapCunkSize, mapCunkSize));
        }
    }
    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

