using System;
using UnityEngine;

[Serializable]
public class IntegerMap
{
    internal int[,] data { get; private set; }
    internal int width { get; private set; }
    internal int height { get; private set; }

    [NonSerialized] bool hasPendingChanges;
    public IntegerMap(int w, int h)
    {
        data = new int[w, h];
        width = w;
        height = h;
        hasPendingChanges = true;
    }


    public int GetValue((int x, int y) position)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height) return int.MinValue;
        return data[position.x, position.y];
    }
    public int GetValue(int x, int y)
    {
        (int x, int y) position = (x, y);
        return GetValue(position);
    }
    public bool SetValue((int x, int y) position, int value)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height) return false;
        else
        {
            data[position.x, position.y] = value;
            hasPendingChanges = true;
            return true;
        }
    }
    public bool SetValueAdditive((int x, int y) position, int value)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height) return false;
        else
        {
            data[position.x, position.y] += value;
            hasPendingChanges = true;
            return true;
        }
    }
    public bool SetValue(int x, int y, int value)
    {
        (int x, int y) position = (x, y);
        return SetValue(position, value);
    }

    public void OnViewRefreshed()
    {
        hasPendingChanges = false;
    }


    public bool HasPendingChanges()
    {
        return hasPendingChanges;
    }

}