using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

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

    public static List<Point> FindPath(bool[,] grid, Point start, Point end)
    {
        List<Point> path = new List<Point>();
        HashSet<Point> closedSet = new HashSet<Point>();
        PriorityQueue<Point, int> openSet = new PriorityQueue<Point, int>(new IntComparer());
        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        Dictionary<Point, int> gScore = new Dictionary<Point, int>();
        Dictionary<Point, int> fScore = new Dictionary<Point, int>();
        HashSet<Point> inOpenSet = new HashSet<Point>(); // Track elements in the openSet

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = HeuristicCostEstimate(start, end);
        inOpenSet.Add(start);

        while (openSet.Count > 0)
        {
            Point current = openSet.Dequeue();
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

            foreach (var neighbor in GetNeighbors(current, grid))
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

        return path; // No path found
    }

    private static int HeuristicCostEstimate(Point start, Point end)
    {
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y); // Manhattan distance
    }

    private static IEnumerable<Point> GetNeighbors(Point current, bool[,] grid)
    {
        int maxX = grid.GetLength(0);
        int maxY = grid.GetLength(1);

        int x = current.X;
        int y = current.Y;

        List<Point> neighbors = new List<Point>();

        if (x > 0 && !grid[x - 1, y])
            neighbors.Add(new Point(x - 1, y));

        if (x < maxX - 1 && !grid[x + 1, y])
            neighbors.Add(new Point(x + 1, y));

        if (y > 0 && !grid[x, y - 1])
            neighbors.Add(new Point(x, y - 1));

        if (y < maxY - 1 && !grid[x, y + 1])
            neighbors.Add(new Point(x, y + 1));

        return neighbors;
    }
}