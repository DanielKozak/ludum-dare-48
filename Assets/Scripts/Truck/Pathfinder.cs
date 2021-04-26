using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public static bool FindPath(IntegerMap localMap, int localWidth, int localHeight, (int, int) start, (int x, int y) goal, out Queue<Vector3> result)
    {
        result = new Queue<Vector3>();
        List<(int, int)> path = new List<(int, int)>();

        List<Node> OpenList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        Node initial = new Node();
        initial.pos = start;
        initial.g = 0;
        initial.h = Mathf.Abs((goal.x - initial.pos.x)) + Mathf.Abs((goal.y - initial.pos.y));
        initial.f = initial.g + initial.h;

        OpenList.Add(initial);

        Node currentNode = initial;

        while (OpenList.Count != 0)
        {
            {
                float min = float.MaxValue;
                foreach (var n in OpenList)
                {
                    if (n.f < min)
                    {
                        min = n.f;
                        currentNode = n;
                    }
                }
                if (currentNode.pos == goal)
                {
                    while (currentNode != null)
                    {
                        path.Add(currentNode.pos);
                        currentNode = currentNode.parent;
                    }
                    path.Reverse();
                    for (var i = 0; i < path.Count; i++)
                    {
                        // Debug.Log($"Pathfinder add {path[i]}");

                        result.Enqueue(MapUtils.GetWorldPosByCellWithLayer(path[i]));
                    }

                    return true;
                }
                OpenList.Remove(currentNode);
                ClosedList.Add(currentNode);

                var nList = new List<Node>();

                for (int i = currentNode.pos.x - 1; i <= currentNode.pos.x + 1; i++)
                    for (int j = currentNode.pos.y - 1; j <= currentNode.pos.y + 1; j++)
                    {
                        if (i < 0 || i >= localWidth || j < 0 || j >= localHeight) continue;
                        else if ((i, j) == currentNode.pos) continue;
                        else
                        {
                            if (localMap.GetValue((i, j)) != 2) continue;
                            var neighb = new Node();
                            neighb.pos = (i, j);
                            nList.Add(neighb);

                        }
                    }
                foreach (var neighbour in nList)
                {
                    bool gScoreBetter = false;
                    if (ClosedList.Contains(neighbour))
                    {
                        continue;
                    }
                    float tentativeGScore = (currentNode.g + MapUtils.EuclidDist2D(neighbour.pos, currentNode.pos)); //TODO Movement costs;
                    if (!OpenList.Contains(neighbour))
                    {
                        OpenList.Add(neighbour);
                        gScoreBetter = true;
                    }
                    else
                    {
                        if (tentativeGScore < neighbour.g) gScoreBetter = true;
                        else gScoreBetter = false;
                    }

                    if (gScoreBetter)
                    {
                        neighbour.parent = currentNode;
                        neighbour.g = tentativeGScore;
                        neighbour.h = Mathf.Abs((goal.x - neighbour.pos.x)) + Mathf.Abs((goal.y - neighbour.pos.y));
                        neighbour.f = neighbour.g + neighbour.h;

                    }

                }
            }
        }
        ToastController.Instance.Toast("Truck can't find path to destination");
        return false;

    }

    public class Node
    {
        internal Node parent;
        internal (int x, int y) pos;
        internal float g, h, f;

        public override bool Equals(object obj)
        {
            Node b = obj as Node;
            return this.pos == b.pos;
        }
    }
}