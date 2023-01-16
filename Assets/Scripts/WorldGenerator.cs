using System;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Map Properties")]
    public int width;
    public int height;
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private GameObject MapHolderPrefab;

    [Header("Launch Settings")]
    [SerializeField] private bool GenerateNewWorld;
    [SerializeField] private bool LoadFromFile;
    public bool EnableTileCoordinates;

    [Header("Map Load Settings")]
    [SerializeField] private Texture2D mapTexture;

    private static Map latestMap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Map GenerateMap()
    {
        if (width <= 0 || height <= 0)
        {
            Debug.Log("Map size must be bigger than 0.");
            return null;
        }
        if (Application.isPlaying)
        {
            if (GenerateNewWorld)
            {
                latestMap = InitializeMap();
            }
            else if (LoadFromFile)
            {
                latestMap = LoadMapFromFile();
            }
        }
        else if (Application.isEditor)
        {
            if (latestMap != null) { ClearMap(); }
            if (GenerateNewWorld)
            {
                latestMap = InitializeMap();
            }
            else
            {
                latestMap =  LoadMapFromFile();
            }
        }
        return latestMap;
    }

    public Map LoadMapFromFile()
    {
        Debug.Log("Loading map from file.");
        this.width = mapTexture.width;
        this.height = mapTexture.height;
        Tile[,] tiles = new Tile[width, height];
        
        if (mapTexture == null) { throw new Exception("Missing map to load."); }

        Color[] pixels = mapTexture.GetPixels();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newTile = Instantiate(TilePrefab, transform);
                newTile.transform.position = new Vector3(i * 0.74f, j * 0.86328125f + 0.431640625f * (i % 2));
                tiles[i, j] = newTile.GetComponent<Tile>();
                tiles[i, j].InitTile(i, j, EnableTileCoordinates, pixels[i + j * width]);
            }
        }

        return new Map(tiles, width, height);
    }

    private Map InitializeMap()
    {
        Tile[,] map = new Tile[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newTile = Instantiate(TilePrefab, transform);
                newTile.transform.position = new Vector3(i*0.74f, j * 0.86328125f + 0.431640625f * (i % 2));
                map[i, j] = newTile.GetComponent<Tile>();
                map[i, j].InitTile(i, j, EnableTileCoordinates);
            }
        }
        return new Map(map, width, height);
    }

    private void ClearMap()
    {
        latestMap.ClearMap();
    }

    public void DestroyAllTiles()
    {
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Tile"))
        {
            o.GetComponent<Tile>().Deconstruct();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
