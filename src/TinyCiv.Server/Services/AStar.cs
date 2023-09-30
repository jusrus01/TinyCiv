using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;

namespace TinyCiv.Server.Services;

class AStar
{
    public class IntComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }

    public static List<ServerPosition> FindPath(Map map, ServerPosition start, ServerPosition end)
    {
        List<ServerPosition> path = new();
        HashSet<ServerPosition> closedSet = new();
        PriorityQueue<ServerPosition, int> openSet = new PriorityQueue<ServerPosition, int>(new IntComparer());
        Dictionary<ServerPosition, ServerPosition> cameFrom = new();
        Dictionary<ServerPosition, int> gScore = new ();
        Dictionary<ServerPosition, int> fScore = new();
        HashSet<ServerPosition> inOpenSet = new(); // Track elements in the openSet

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = HeuristicCostEstimate(start, end);
        inOpenSet.Add(start);

        while (openSet.Count > 0)
        {
            ServerPosition current = openSet.Dequeue();
            inOpenSet.Remove(current);

            if (current == end)
            {
                path.Add(current);
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }
                return path;
            }

            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current, map))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + 1;

                if (!inOpenSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);

                    if (!inOpenSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                        inOpenSet.Add(neighbor);
                    }
                }
            }
        }

        return path;
    }

    private static int HeuristicCostEstimate(ServerPosition start, ServerPosition end)
    {
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y); // Manhattan distance
    }

    private static IEnumerable<ServerPosition> GetNeighbors(ServerPosition current, Map map)
    {
        int maxX = Constants.Game.WidthSquareCount;
        int maxY = Constants.Game.HeightSquareCount;

        int x = current.X;
        int y = current.Y;

        List<ServerPosition> neighbors = new List<ServerPosition>();

        if (x > 0)
            neighbors.Add(new ServerPosition { X = x - 1, Y = y });

        if (x < maxX - 1)
            neighbors.Add(new ServerPosition { X = x + 1, Y = y });

        if (y > 0)
            neighbors.Add(new ServerPosition { X = x, Y = y - 1 });

        if (y < maxY - 1)
            neighbors.Add(new ServerPosition { X = x, Y = y + 1 });

        // Check if the neighbor positions are valid and not occupied by obstacles or other units
        neighbors.RemoveAll(neighbor =>
        {
            var gameObject = map.Objects?.Find(obj => obj.Position == neighbor);
            return gameObject?.Type != GameObjectType.Empty;
        });

        return neighbors;
    }
}