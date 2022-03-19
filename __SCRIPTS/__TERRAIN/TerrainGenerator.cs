using System.Collections;
using UnityEngine;

/// <summary>
/// Vitor Felipe Ramos Mello
/// Studant of Computer Engenering in University Of Vale Of Itajai UNIVALI
/// Santa Catarina - BRAZIL - Itajai
/// Codname VDEV
/// GanjGameSudio
/// GanjGameStudio, GGS and VDEV
/// 
/// They are authentic brands, their use for commercial and distribution purposes is only allowed if the proper attribution
/// is made to the creator Vitor Felipe Ramos Mello. Failure to comply with the terms of reference will result in lawsuits!
/// 
/// LIFE terrain generator v0.07 rev 0.06
/// {Created the terrain generator}
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    [Header("Set World Configurations")]
    [SerializeField] private Sprite tile;
    [SerializeField, Range(8, 10048)] private int worldSize = 100;
    [SerializeField] private float caveFreq = 0.05f; // The higher the frequency, the greater the amount of caves
    [SerializeField] private float seed;
    [SerializeField] private Texture2D noiseTexture;


    [Header("Advanced Configurations of World")]
    [SerializeField, Range(1, 100)] private float heightMultiplier = 4f;
    [SerializeField, Range(1, 100)] private int heightAddition = 25;
    [SerializeField, Range(0.01f, 1)] private float terrainFreq = 0.05f;
    [SerializeField, Range(0.1f, 1)] private float caveSurfaceValue = 0.25f;


    private void Start() // This function go make a dynamic update seed and Terrain Generation without void Update(){...} and without press
                             // Play - Stop
    {
        seed = Random.Range(-10000, 10000); // Seed random generation
        //seed = -1035;
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        //Read X and Y axis for world size
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition; // Get height to apply perlim noise function and fix Y axis
            //This setup allows me to set a flat world on the Y axis which is what we need initially
            for (int y = 0; y < height; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > caveSurfaceValue) // If pixels x or y > 0.2f then create game objects with perlim noise 2D
                {
                    GameObject newTile = new GameObject(name = "tile"); // Tile as a new Object
                    newTile.transform.parent = this.transform; // All objects'll be  sons of Main object
                    newTile.AddComponent<SpriteRenderer>(); // Add a SpriteRenderer component in GameObject Tile
                    newTile.GetComponent<SpriteRenderer>().sprite = tile; // The tile var = SpriteRenderer component
                    newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f); // Updated tile position in centered
                }
            }
        }
    }

    private void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);

        ///
        /// Square configuration in X axis and Y axis __|__
        ///                                             |
        ///                                             

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                //V = function perlin noise 2D my width and my height of my texture is the X and Y vars
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq); // Implement this for combine Perlin Noise and Noise Map
                                                                                            // to the seed. Diferent configurations of terrain
                noiseTexture.SetPixel(x, y, new Color(v, v, v)); // Paint Texture
            }
        }

        noiseTexture.Apply(); // Apply modifications
    }
}
