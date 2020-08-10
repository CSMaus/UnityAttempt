using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public static class Noise3D //: MonoBehaviour
{
    //public static float[,] GenerateNoise3D(float amplitude, float frequency)
    //{
    //    System.Random snoise = new System.Random();



    //    float fractalNoise(float3 point)
    //    {
    //        noiseSum = 0;
    //        amplitude = 1;
    //        frequency = 1;

    //        for(int i = 0; i < 5; i++)
    //        {
    //            noiseSum += snoise(point * frequency) * amplitude;

    //            frequency *= 2;

    //            amplitude *= 0.5;
    //        }

    //        return noiseSum;
    //    }
    //}

    public static float[, ,] GenerateNoiseMap(int mapWidth, int mapHeight, int mapLength, int seed, float scale, int octaves, float persistance, float lacunarity, Vector3 offset)
    {
        float[, ,] noiseMap = new float[mapWidth, mapHeight, mapLength];

        System.Random prng = new System.Random(seed);
        Vector3[] octaveOffsets = new Vector3[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-10000, 10000) + offset.x;
            float offsetY = prng.Next(-10000, 10000) + offset.y;
            float offsetZ = prng.Next(-10000, 10000) + offset.z;

            octaveOffsets[i] = new Vector3(offsetX, offsetY, offsetZ);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        float halfLength = mapLength / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int zy = 0; zy < mapLength; zy++)
                {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;
                        float sampleZ = (y - halfLength) / scale * frequency + octaveOffsets[i].z;

                        float xy = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        float xz = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                        float yz = Mathf.PerlinNoise(sampleY, sampleZ) * 2 - 1;
                        float yx = Mathf.PerlinNoise(sampleY, sampleX) * 2 - 1;
                        float zx = Mathf.PerlinNoise(sampleZ, sampleX) * 2 - 1;
                        float z_y = Mathf.PerlinNoise(sampleZ, sampleY) * 2 - 1;

                        float perlinValue = (xy + xz + yz + yx + zx + z_y) / 6; //Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;
                        //noiseMap[x, y] = perlinValue;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y, zy] = noiseHeight;
                }
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int zy = 0; zy < mapLength; zy++)
                {
                    noiseMap[x, y, zy] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y, zy]);
                }
            }
        }

        return noiseMap;
    }

}
