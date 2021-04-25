using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public static MapController Instance;
    public Tilemap GroundTileMap;
    public Tilemap JungleTileMap;
    public Tilemap WaterTileMap;

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
        // Debug.Log($"ZLevel StructureMapUpdate has changes");

        for (int i = startx; i < endx; i++)
            for (var j = starty; j < endy; j++)
            {
                if (i < 0 || i >= MapW || j < 0 || j >= MapH) continue;

                var tile = (Tile)ScriptableObject.CreateInstance("Tile");
                Debug.Log($"map update {TerrainMap.GetValue(i, j)}");
                switch (TerrainMap.GetValue(i, j))
                {
                    case -1:
                        tile.sprite = TileTypeEngine.Instance.WaterSprite;
                        WaterTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 0:
                        tile.sprite = TileTypeEngine.Instance.SandSprite;
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 1:
                        tile.sprite = TileTypeEngine.Instance.GroundSprite;
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 2:
                        tile.sprite = TileTypeEngine.Instance.ConcreteSprite;
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                    case 3:
                        tile.sprite = TileTypeEngine.Instance.GroundSprite;
                        GroundTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        tile.sprite = TileTypeEngine.Instance.JungleSprite;
                        JungleTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                        break;
                }
            }
        //         // Debug.Log($"Setting structure @ {(i, j)} id: {StructureMap.GetValue(i, j)}");
        //         if (tile != null)
        //         {
        //             tileList.Add(tile);
        //             positionList.Add(new Vector3Int(i, j, 0));
        //         }
        //         else
        //         {
        //             positionRemovalList.Add(new Vector3Int(i, j, 0));
        //         }
        //     }

        // positions = positionList.ToArray();
        // tiles = tileList.ToArray();

        // FloorTileMap.SetTiles(positions, tiles);

        // foreach (var item in positionRemovalList)
        // {
        //     FloorTileMap.SetTile(item, null);
        // }
    }

    bool debug = false;
    public void RemoveJungle((int x, int y) pos)
    {
        TerrainMap.SetValue(pos, 1);
        debug = true;
        UpdateMap(pos.x - 1, pos.y - 1, pos.x + 1, pos.y + 1);
    }
}