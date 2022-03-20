using System.Collections;
using System.Collections.Generic;
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
/// LIFE terrain generator v0.2 rev 0.1
/// {   
///     Added texture in terrain,
///     Added optin to create or not create caves,
///     Added function to optimized the time,
///     Added trees
///     
/// }
/// 
/// /// LIFE terrain generator v0.3 rev 0.2
/// {   
///     Chuncks added
///     Bugs in casting float to int fixs
/// }
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Texture Configurations")]
    [SerializeField, Range(1, 15)] private int dirtLayerHeight = 5;
    [Header("Global Atlas")]
    [SerializeField] private Sprite grass;
    [SerializeField] private Sprite dirt;
    [SerializeField] private Sprite stone;

    [Header("Trees")]
    [SerializeField] private int treeChance = 10;
    [SerializeField] private int minTreeHeight = 4;
    [SerializeField] private int maxTreeHeight = 6;
    [SerializeField] private Sprite log;
    [SerializeField] private Sprite leaf;

    [Header("Generation Settings")]
    [SerializeField] private int chunckSize = 16;

    [Header("Set World Configurations")]
    [SerializeField, Range(8, 10048)] private int worldSize = 100;
    [SerializeField] private float caveFreq = 0.05f; // The higher the frequency, the greater the amount of caves
    [SerializeField] private float seed;
    [SerializeField] private Texture2D noiseTexture;


    [Header("Advanced Configurations of World")]
    [SerializeField, Range(1, 100)] private float heightMultiplier = 4f;
    [SerializeField, Range(1, 100)] private int heightAddition = 25;
    [SerializeField, Range(0.01f, 1)] private float terrainFreq = 0.05f;
    [SerializeField, Range(0.1f, 1)] private float caveSurfaceValue = 0.25f;
    [SerializeField] private List<Vector2> worldTiles = new List<Vector2> ();

    [Header("Boolean Variables to Configurate Caves and others")]
    [SerializeField] private bool generateCaves = true;

    //Private variables dont need the inspector

    private GameObject[] worldChuncks;


    private void Start()
    {
        seed = Random.Range(-10000, 10000); // Seed random generation
        //seed = -1816;
        GenerateNoiseTexture();
        CreateChuncks();
        GenerateTerrain();
    }

    private void CreateChuncks()
    {
        int numChunks = worldSize / chunckSize;

        worldChuncks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = "Chunk[ " + i.ToString() + " ]";
            newChunk.transform.parent = this.transform; // The objects = Sons objects
            worldChuncks[i] = newChunk;
        }
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
                Sprite tileSprite;

                if(y < height - dirtLayerHeight)
                {
                    tileSprite = stone;
                }
                else if(y < height - 1)
                {
                    tileSprite = dirt;
                }
                else
                {
                    //top layer of the terrain//
                    tileSprite = grass;
                }

                if (generateCaves)
                {
                    if (noiseTexture.GetPixel(x, y).r > caveSurfaceValue) // If pixels x or y > 0.2f then create game objects with perlim noise 2D
                    {
                        PlaceTile(tileSprite, x, y);
                    }
                }
                else
                {
                    PlaceTile(tileSprite, x, y);
                }

                if(y >= height - 1)
                {
                    int t = Random.Range(0, treeChance);
                    if (t == 1)
                    {
                        //Generate tree
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            GenerateTree(x, y + 1);
                        }
                    }
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

    private void GenerateTree(int x, int y)
    {   
        // Define our tree

        //Generate Log
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(log, x, y + i);
        }

        //Generate Leaves

        #region Y Leaves
        PlaceTile(leaf, x, y + treeHeight);
        PlaceTile(leaf, x, y + treeHeight + 1);
        PlaceTile(leaf, x, y + treeHeight + 2);
        #endregion

        #region X left Leaves
        PlaceTile(leaf, x - 1, y + treeHeight);
        PlaceTile(leaf, x - 1, y + treeHeight + 1);
        #endregion

        #region X right Leaves
        PlaceTile(leaf, x + 1, y + treeHeight);
        PlaceTile(leaf, x + 1, y + treeHeight + 1);
        #endregion
    }

    private void PlaceTile(Sprite tileSprite, int x, int y)
    {
        GameObject newTile = new GameObject(); // Tile as a new Object

        float chuckCoord = (Mathf.Round(x / chunckSize) * chunckSize);
        chuckCoord /= chunckSize;
        newTile.transform.parent = worldChuncks[(int)chuckCoord].transform; // All objects'll be  sons of Main object


        newTile.AddComponent<SpriteRenderer>(); // Add a SpriteRenderer component in GameObject Tile
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite; // The tile var = SpriteRenderer component
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f); // Updated tile position in centered

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
