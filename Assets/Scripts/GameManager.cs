using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Start()
    {
        CameraControllerTopDown.Instance.InitCamera();
        MapController.Instance.TerrainMap = WorldGenerator.GenerateWorld(200, 50);
        MapController.Instance.UpdateMap();
    }
}
