using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MapUtils
{


    public static List<(int, int)> GetNeighbours(int x, int y, int width, int height, bool returnSelf = false)
    {
        List<(int, int)> res = new List<(int, int)>();
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i < 0 || i >= width || j < 0 || j >= height) continue;
                if ((i, j) == (x, y) && !returnSelf) continue;
                res.Add((i, j));
            }
        return res;
    }

    public static List<(int, int)> GetNeighboursByDistance(int x, int y, int width, int height, int dist, bool returnSelf = false)
    {
        List<(int, int)> res = new List<(int, int)>();
        for (int i = x - dist; i <= x + dist; i++)
            for (int j = y - dist; j <= y + dist; j++)
            {
                if (EuclidDist2D((x, y), (i, j)) > (float)dist) continue;
                if (i < 0 || i >= width || j < 0 || j >= height) continue;
                if ((i, j) == (x, y) && !returnSelf) continue;
                res.Add((i, j));
            }
        return res;
    }


    public static float EuclidDist2D((int x, int y) start, (int x, int y) end)
    {
        return Mathf.Sqrt((end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y));
    }
    public static float MnhDist2D((int x, int y) start, (int x, int y) end)
    {
        return Mathf.Abs((end.x - start.x)) + Mathf.Abs((end.y - start.y));
    }
    public static (int, int) GetTileFromScreenPosition(float3 screenPosition, Camera camera, Grid grid)
    {
        var res = grid.WorldToCell(camera.ScreenToWorldPoint(screenPosition));
        return (res.x, res.y);
    }

    public static (int, int) GetCellFromMousePos()
    {
        var pos = MapController.Instance.mGrid.WorldToCell(GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition));
        return (pos.x, pos.y);
    }

    public static Vector3 GetWorldPosByCell((int x, int y) cell)
    {

        var pos = MapController.Instance.mGrid.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
    }
    public static Vector3 GetWorldPosByCellWithLayer((int x, int y) cell)
    {

        var pos = MapController.Instance.mGrid.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, -25);
    }
    public static (int, int) GetCellByWorldPos(Vector3 worldPos)
    {
        var unityCellCoords = MapController.Instance.mGrid.WorldToCell(worldPos);
        return (unityCellCoords.x, unityCellCoords.y);
    }





}