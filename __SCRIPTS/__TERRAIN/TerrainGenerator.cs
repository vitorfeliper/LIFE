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
/// 
/// /// /// LIFE terrain generator v0.4 rev 0.3
/// {   
///     Miners added
///     Bugs fixs
///     Scripts reorder
/// }
/// /// /// /// LIFE terrain generator v0.5 rev 0.4
/// {   
///     Logic optimized
/// }
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Texture Configurations")]
    [SerializeField, Range(1, 15)] private int dirtLayerHeight = 5;

    [Header("Global Atlas")]
    [SerializeField] private TileAtlas tileAtlas;

    [Header("Trees")]
    [SerializeField] private int treeChance = 10;
    [SerializeField] private int minTreeHeight = 4;
    [SerializeField] private int maxTreeHeight = 6;

    [Header("Addons")]
    [SerializeField] private int tallGrassChance = 10;


    [Header("Generation Settings")]
    [SerializeField] private int chunckSize = 16;

    [Header("Set World Configurations")]
    [SerializeField, Range(8, 10048)] private int worldSize = 100;
    [SerializeField] private float caveFreq = 0.05f; // The higher the frequency, the greater the amount of caves
    [SerializeField] private float seed;
    [SerializeField] private Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    [SerializeField] private OreClass[] ores;
/*    [SerializeField] private float coalRarity;
    [SerializeField] private float coalSize;
    [SerializeField] private float ironRarity;
    [SerializeField] private float ironSize;
    [SerializeField] private float goldRarity;
    [SerializeField] private float goldSize;
    [SerializeField] private float diamondRarity;
    [SerializeField] private float diamondSize;
    [SerializeField] private Texture2D coalSpread;
    [SerializeField] private Texture2D ironSpread;
    [SerializeField] private Texture2D goldSpread;
    [SerializeField] private Texture2D diamondSpread;
*/

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

    private void OnValidate()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        GenerateNoiseTexture(caveFreq, caveSurfaceValue, caveNoiseTexture);

        //Ores
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);

    }

    private void Start()
    {
        seed = Random.Range(-10000, 10000); // Seed random generation

        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        //seed = 5053;
        GenerateNoiseTexture(caveFreq, caveSurfaceValue,caveNoiseTexture);

        //Ores
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);

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
                Sprite[] tileSprites;

                if(y < height - dirtLayerHeight)
                {
                    tileSprites = tileAtlas.stone.tileSprites;
                    //Coal
                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpwnHeight)
                        tileSprites = tileAtlas.coal.tileSprites;                     
                                                                                    
                    //Iron                                                          
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpwnHeight)
                        tileSprites = tileAtlas.iron.tileSprites;                     
                                                                                    
                    //Gold                                                          
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpwnHeight)
                        tileSprites = tileAtlas.gold.tileSprites;                     
                                                                                    
                    //Diamond                                                       
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpwnHeight)
                        tileSprites = tileAtlas.diamond.tileSprites;
                }
                else if(y < height - 1)
                {
                    tileSprites = tileAtlas.dirt.tileSprites;
                }
                else
                {
                    //top layer of the terrain//
                    tileSprites = tileAtlas.grass.tileSprites;
                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f) // If pixels x or y > 0.2f then create game objects with perlim noise 2D
                    {
                        PlaceTile(tileSprites, x, y);
                    }
                }
                else
                {
                    PlaceTile(tileSprites, x, y);
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
                    else
                    {
                        int i = Random.Range(0, tallGrassChance);

                        //Generate tree
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            PlaceTile(tileAtlas.tallGrass.tileSprites, x, y + 1);
                        }
                    }

                    if(t == 2)
                    {
                        //Generate tree
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            GenerateTreeOrange(x, y + 1);
                        }
                    }
                }
            }
        }
    }

    private void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        //Texture2D noise = new Texture2D(worldSize, worldSize);

        ///
        /// Square configuration in X axis and Y axis __|__
        ///                                             |
        ///                                             

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                //V = function perlin noise 2D my width and my height of my texture is the X and Y vars
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency); // Implement this for combine Perlin Noise and Noise Map
                                                                                             // to the seed. Diferent configurations of terrain
                if(v > limit)
                    noiseTexture.SetPixel(x, y, Color.white); // Paint Texture
                else
                    noiseTexture.SetPixel(x, y, Color.black); // Paint Texture
            }
        }

        noiseTexture.Apply(); // Apply modifications
        //noiseTexture = noise;
    }

    private void GenerateTreeOrange(int x, int y)
    {
        // Define our tree

        //Generate Log
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(tileAtlas.log.tileSprites, x, y + i);
        }

        //Generate Leaves

        #region Y Leaves

        PlaceTile(tileAtlas.leafOrange.tileSprites, x, y + treeHeight);
        PlaceTile(tileAtlas.leafOrange.tileSprites, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leafOrange.tileSprites, x, y + treeHeight + 2);

        #endregion

        #region X left Leaves

        PlaceTile(tileAtlas.leafOrange.tileSprites, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leafOrange.tileSprites, x - 1, y + treeHeight + 1);

        #endregion

        #region X right Leaves

        PlaceTile(tileAtlas.leafOrange.tileSprites, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leafOrange.tileSprites, x + 1, y + treeHeight + 1);

        #endregion
    }

    private void GenerateTree(int x, int y)
    {   
        // Define our tree

        //Generate Log
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(tileAtlas.log.tileSprites, x, y + i);
        }

        //Generate Leaves

        #region Y Leaves

        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);

        #endregion

        #region X left Leaves

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);

        #endregion

        #region X right Leaves

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);

        #endregion
    }

    private void PlaceTile(Sprite[] tileSprites, int x, int y)
    {
        GameObject newTile = new GameObject(); // Tile as a new Object

        float chuckCoord = (Mathf.Round(x / chunckSize) * chunckSize);
        chuckCoord /= chunckSize;
        newTile.transform.parent = worldChuncks[(int)chuckCoord].transform; // All objects'll be  sons of Main object


        newTile.AddComponent<SpriteRenderer>(); // Add a SpriteRenderer component in GameObject Tile

        int spriteIndex = Random.Range(0, tileSprites.Length);

        newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex]; // The tile var = SpriteRenderer component
        newTile.name = tileSprites[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f); // Updated tile position in centered

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
