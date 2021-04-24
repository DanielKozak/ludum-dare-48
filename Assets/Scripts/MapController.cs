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

    private void Awake() => Instance = this;


    public void UpdateMap()
    {
        Vector3Int[] positions;
        TileBase[] tiles;
        List<Tile> tileList = new List<Tile>();
        List<Vector3Int> positionList = new List<Vector3Int>();
        List<Vector3Int> positionRemovalList = new List<Vector3Int>();
        // Debug.Log($"ZLevel StructureMapUpdate has changes");

        for (int i = 0; i < MapW; i++)
            for (var j = 0; j < MapH; j++)
            {
                var tile = (Tile)ScriptableObject.CreateInstance("Tile");
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
}