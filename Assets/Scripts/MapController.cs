using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public static MapController Instance;
    public Tilemap GroundTileMap;
    public Tilemap JungleTileMap;

    public int MapW = 200;
    public int MapH = 50;

    public IntegerMap TerrainMap;
    public IntegerMap BananaMap;

    public Grid mGrid;

    private void Awake()
    {

        Instance = this;
        mGrid = GetComponent<Grid>();
    }

    public void InitMap()
    {
        // for (int i = 0; i < 500; i++)
        //     for (var j = 0; j < 100; j++)
        //     {
        //         if (GroundTileMap.GetSprite(new Vector3Int(i, j, 0))) Debug.Log((i, j));
        //     }

        TerrainMap = new IntegerMap(MapW, MapH);
        for (int i = 0; i < MapW; i++)
            for (var j = 0; j < MapH; j++)
            {
                if (i < 0 || i >= MapW || j < 0 || j >= MapH) continue;
                if (GroundTileMap.GetSprite(new Vector3Int(i, j, 0)) == null)
                    Debug.Log((i, j) + " " + (MapW, MapH));
                string name = GroundTileMap.GetSprite(new Vector3Int(i, j, 0)).name;
                switch (name)
                {
                    case "water": //-1
                        TerrainMap.SetValue((i, j), -1);
                        break;
                    case "ground": //1
                        TerrainMap.SetValue((i, j), 1);

                        break;
                    case "sand": //0
                        TerrainMap.SetValue((i, j), 0);

                        break;
                    case "concrete"://2
                        TerrainMap.SetValue((i, j), 2);

                        break;
                    default:
                        break;
                }

            }

        PopulateJungleMap();
        PopulateBananaMap();
        UpdateMap();
        //TODO update map
    }



    public void UpdateMap()
    {
        UpdateMap(0, 0, MapW, MapH);
    }

    public void UpdateMap(int startx, int starty, int endx, int endy)
    {
        Vector3Int[] positions;
        TileBase[] tiles;
        List<Tile> tileList = new List<Tile>();
        List<Vector3Int> positionList = new List<Vector3Int>();
        List<Vector3Int> positionRemovalList = new List<Vector3Int>();

        for (int i = startx; i <= endx; i++)
            for (var j = starty; j <= endy; j++)
            {
                if (i < 0 || i >= MapW || j < 0 || j >= MapH) continue;

                var tile = (Tile)ScriptableObject.CreateInstance("Tile");
                // Debug.Log($"map update {TerrainMap.GetValue(i, j)}");
                int index = 0;
                if (BananaMap.GetValue(i, j) > 0)
                {

                    tile.sprite = TileTypeEngine.Instance.SandSprites[index];
                    tile.color = Color.yellow;
                    JungleTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                    tile.color = Color.white;

                }
                switch (TerrainMap.GetValue(i, j))
                {
                    case -1:
                        index = Random.Range(0, TileTypeEngine.Instance.WaterSprites.Count);
                        tile.sprite = TileTypeEngine.Instance.WaterSprites[index];
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 0:
                        index = Random.Range(0, TileTypeEngine.Instance.SandSprites.Count);

                        tile.sprite = TileTypeEngine.Instance.SandSprites[index];
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 1:
                        index = Random.Range(0, TileTypeEngine.Instance.GroundSprites.Count);

                        tile.sprite = TileTypeEngine.Instance.GroundSprites[index];
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 2:
                        index = Random.Range(0, TileTypeEngine.Instance.ConcreteSprites.Count);

                        tile.sprite = TileTypeEngine.Instance.ConcreteSprites[index];
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 3:
                        index = Random.Range(0, TileTypeEngine.Instance.JungleSprites.Count);

                        tile.sprite = TileTypeEngine.Instance.GroundSprites[index];
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);

                        tile.sprite = TileTypeEngine.Instance.JungleSprites[index];
                        JungleTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                }


            }
    }

    bool debug = false;
    public void RemoveJungle((int x, int y) pos)
    {
        TerrainMap.SetValue(pos, 1);
        UpdateMap(pos.x - 1, pos.y - 1, pos.x + 1, pos.y + 1);
    }

    public void SetTileValue((int x, int y) pos, int value)
    {
        TerrainMap.SetValue(pos, value);
        UpdateMap(pos.x - 1, pos.y - 1, pos.x + 1, pos.y + 1);
    }


    void PopulateJungleMap()
    {
        Debug.Log($"Populating jungle map");

        float treshold = 380f;
        float waterTreshold = 200f;
        for (var x = 35; x <= MapW; x++)
            for (var y = 0; y <= MapH; y++)
            {
                if (TerrainMap.GetValue((x, y)) == 1)
                {
                    float noize = Mathf.PerlinNoise(x / 10f, y / 10f) * 1000f;
                    // Debug.Log($"noize {noize}");

                    if (noize > treshold)
                    {
                        // Debug.Log($"Jungle now at {(x, y)}");
                        TerrainMap.SetValue((x, y), 3);

                    }
                    if (noize < waterTreshold)
                    {
                        TerrainMap.SetValue((x, y), -1);

                    }

                }
            }
    }
    void PopulateBananaMap()
    {
        BananaMap = new IntegerMap(MapW, MapH);
        float treshold = 10f;
        for (var x = 35; x < MapW; x++)
            for (var y = 0; y < MapH; y++)
            {
                if (TerrainMap.GetValue((x, y)) == 3)
                {
                    float noize = Mathf.PerlinNoise(x / 10f + 250, y / 10f + 10) * 1000f;
                    if (noize / 20f > treshold)
                    {
                        // Debug.Log($"noize {Mathf.FloorToInt(noize / 20f)}");

                        BananaMap.SetValue((x, y), Mathf.FloorToInt(noize / 20f));
                        // Debug.Log($"{BananaMap.GetValue((x, y))}");

                    }

                }
            }
    }
}