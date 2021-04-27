using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class MapController : MonoBehaviour
{
    public static MapController Instance;
    public Tilemap GroundTileMap;
    public Tilemap JungleTileMap;
    public Tilemap BananaTileMap;

    public int MapW = 110;
    public int MapH = 75;

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

        StartCoroutine(TutorialController.Instance.StartTutorialRoutine());
        //TODO update map
    }

    bool overlayToasted = false;

    public void ToggleBananaOverLay()
    {
        if (!overlayToasted)
        {
            ToastController.Instance.Toast("Drop probes to reveal bananas on map. (usually in the jungle)", false, true, 7f);
            overlayToasted = true;
        }
        BananaTileMap.gameObject.SetActive(!BananaTileMap.gameObject.activeSelf);
    }
    public void EnableBananaOverlay()
    {
        BananaTileMap.gameObject.SetActive(true);
    }
    public void DisableBananaOverlay()
    {

        BananaTileMap.gameObject.SetActive(false);
    }

    public void UpdateMap()
    {
        UpdateMap(0, 0, MapW, MapH);
    }

    public void UpdateMap(int startx, int starty, int endx, int endy)
    {
        for (int i = startx; i <= endx; i++)
            for (var j = starty; j <= endy; j++)
            {
                if (i < 0 || i >= MapW || j < 0 || j >= MapH) continue;

                var tile = (Tile)ScriptableObject.CreateInstance("Tile");
                // Debug.Log($"map update {TerrainMap.GetValue(i, j)}");
                int index = 0;
                // if (BananaMap.GetValue(i, j) > 0)
                // {

                //     tile.sprite = TileTypeEngine.Instance.SandSprites[index];
                //     tile.color = Color.yellow;
                //     JungleTileMap.SetTile(new Vector3Int(i, j, 0), tile);
                //     tile.color = Color.white;

                // }
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
        JungleTileMap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        BananaTileMap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
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
        float waterTreshold = 250f;
        float randomx = Random.Range(0, 25);
        float randomy = Random.Range(0, 25);
        for (var x = 35; x <= MapW; x++)
            for (var y = 0; y <= MapH; y++)
            {
                if (TerrainMap.GetValue((x, y)) == 1)
                {
                    float noize = Mathf.PerlinNoise(x / 10f + randomx, y / 10f + randomy) * 1000f;
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
    [System.NonSerialized] public int maxBanana = 0;
    [System.NonSerialized] public int minBanana = 0;
    void PopulateBananaMap()
    {
        BananaMap = new IntegerMap(MapW, MapH);
        int treshold = 11;
        minBanana = treshold;
        float randomx = Random.Range(0, 25);
        float randomy = Random.Range(0, 25);
        for (var x = 35; x < MapW; x++)
            for (var y = 0; y < MapH; y++)
            {
                if (TerrainMap.GetValue((x, y)) == 3)
                {
                    float noize = Mathf.PerlinNoise(x / 10f + randomx, y / 10f + randomy) * 1000f;
                    if (noize / 35f > treshold)
                    {
                        if (maxBanana < Mathf.FloorToInt(noize / 35f))
                        {
                            maxBanana = Mathf.FloorToInt(noize / 35f);
                        }
                        // Debug.Log($"noize {Mathf.FloorToInt(noize / 20f)}");
                        SetBananaMapValue((x, y), Mathf.FloorToInt(noize / 35f));
                        // Debug.Log($"{BananaMap.GetValue((x, y))}");

                    }
                    else
                    {
                        SetBananaMapValue((x, y), 1);

                    }

                }
            }

    }

    public Transform GhostContainer;
    public void RevealBanana(int x, int y)
    {
        StartCoroutine(RevealRoutine(x, y));
    }
    IEnumerator RevealRoutine(int x, int y)
    {
        float delay = Random.Range(0f, 0.2f);
        var tileGO = Instantiate(DataContainer.Instance.GhostBananaPrefab, GhostContainer);

        tileGO.transform.position = new Vector3(x + 0.5f, y + 0.5f, -24);


        var renderer = tileGO.GetComponent<SpriteRenderer>();
        renderer.color = GetBananaColor((x, y), BananaMap.GetValue((x, y)));
        Vector3Int V3IPos = new Vector3Int(x, y, 0);

        yield return new WaitForSeconds(delay);
        AudioManager.PlaySound("reveal");
        Color c = renderer.color;
        c.a = 0;
        renderer.DOColor(c, 0.3f).OnComplete(() =>
        {
            RevealBananaTile((x, y));
            Destroy(tileGO);
        });// = dir;



    }


    public void SetBananaMapValue((int x, int y) pos, int value)
    {
        BananaMap.SetValue(pos, value);

        Vector3Int V3IPos = new Vector3Int(pos.x, pos.y, 0);
        if (value == 0)
        {
            BananaTileMap.SetTile(V3IPos, null);
            return;
        }
        var tile = BananaTileMap.GetTile<Tile>(V3IPos);
        if (tile == null) return;

        tile.color = GetBananaColor(pos, value);
        BananaTileMap.SetTile(V3IPos, tile);

    }


    public void RevealBananaTile((int x, int y) pos)
    {
        Vector3Int V3IPos = new Vector3Int(pos.x, pos.y, 0);
        Tile tile;
        tile = BananaTileMap.GetTile<Tile>(V3IPos);
        if (tile != null) return;
        tile = (Tile)ScriptableObject.CreateInstance("Tile");
        tile.sprite = DataContainer.Instance.BananaSprite;
        tile.color = GetBananaColor(pos, BananaMap.GetValue(pos));
        BananaTileMap.SetTile(V3IPos, tile);
    }


    Color GetBananaColor((int x, int y) pos, int value)
    {
        float maxA = 1f;
        float minA = 0.2f;
        // float x = (maxBanana - minBanana) / (maxA - minA);


        float lerpedValue = Mathf.InverseLerp((float)minBanana, (float)maxBanana, value);
        float a = Mathf.Lerp(minA, maxA, lerpedValue);
        Color c = new Color(1, 1, 1, a);
        // Debug.Log($"{maxBanana} {minBanana} {value} = {a}");
        return c;
    }
}