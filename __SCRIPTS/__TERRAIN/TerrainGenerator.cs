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
/// LIFE terrain generator v0.05 rev 0.04
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Sprite tile;
    [SerializeField, Range(8, 10048)] private int worldSize = 100;
    [SerializeField] private float noiseFreq = 0.05f;
    [SerializeField] private float seed;
    [SerializeField] private Texture2D noiseTexture;

    private void Start() // This function go make a dynamic update seed and Terrain Generation without void Update(){...} and without press
                             // Play - Stop
    {
        seed = Random.Range(-10000, 10000); // Seed random generation
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        //Read X and Y axis for world size
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                GameObject newTile = new GameObject(name = "tile"); // Tile as a new Object
                newTile.AddComponent<SpriteRenderer>(); // Add a SpriteRenderer component in GameObject Tile
                newTile.GetComponent<SpriteRenderer>().sprite = tile; // The tile var = SpriteRenderer component
                newTile.transform.position = new Vector2(x, y); // Updated tile position
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
                float v = Mathf.PerlinNoise((x + seed) * noiseFreq, (y + seed) * noiseFreq); // Implement this for combine Perlin Noise and Noise Map
                                                                                            // to the seed. Diferent configurations of terrain
                noiseTexture.SetPixel(x, y, new Color(v, v, v)); // Paint Texture
            }
        }

        noiseTexture.Apply(); // Apply modifications
    }
}
