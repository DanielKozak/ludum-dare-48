using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTypeEngine : MonoBehaviour
{
    public static TileTypeEngine Instance;
    public List<Sprite> SandSprites = new List<Sprite>();
    public List<Sprite> WaterSprites = new List<Sprite>();
    public List<Sprite> GroundSprites = new List<Sprite>();
    public List<Sprite> ConcreteSprites = new List<Sprite>();
    public List<Sprite> JungleSprites = new List<Sprite>();


    private void Awake() => Instance = this;
}
