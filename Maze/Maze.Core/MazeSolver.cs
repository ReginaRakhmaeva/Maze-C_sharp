using System;
using System.Collections.Generic;

namespace Core
{
    public class MazeSolver
    {
        public List<Point> Solve(Maze maze, Point start, Point end)
        {
            if (maze == null)
                throw new ArgumentNullException(nameof(maze));

            if (!IsValidPoint(maze, start))
                throw new ArgumentException("Start point is out of bounds", nameof(start));

            if (!IsValidPoint(maze, end))
                throw new ArgumentException("End point is out of bounds", nameof(end));

            if (start == end)
                return new List<Point> { start };

            var queue = new Queue<Point>();
            var visited = new HashSet<Point>();
            var parent = new Dictionary<Point, Point>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == end)
                {
                    return ReconstructPath(parent, start, end);
                }

                var neighbors = GetNeighbors(maze, current);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        parent[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return new List<Point>(); // No path found
        }

        private bool IsValidPoint(Maze maze, Point point)
        {
            return point.Row >= 0 && point.Row < maze.Rows &&
                   point.Col >= 0 && point.Col < maze.Cols;
        }

        private List<Point> GetNeighbors(Maze maze, Point point)
        {
            var neighbors = new List<Point>();

            if (point.Row > 0 && !maze.BottomWalls[point.Row - 1, point.Col])
            {
                neighbors.Add(new Point(point.Row - 1, point.Col));
            }

            if (point.Row < maze.Rows - 1 && !maze.BottomWalls[point.Row, point.Col])
            {
                neighbors.Add(new Point(point.Row + 1, point.Col));
            }

            if (point.Col > 0 && !maze.RightWalls[point.Row, point.Col - 1])
            {
                neighbors.Add(new Point(point.Row, point.Col - 1));
            }

            if (point.Col < maze.Cols - 1 && !maze.RightWalls[point.Row, point.Col])
            {
                neighbors.Add(new Point(point.Row, point.Col + 1));
            }

            return neighbors;
        }

        private List<Point> ReconstructPath(Dictionary<Point, Point> parent, Point start, Point end)
        {
            var path = new List<Point>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = parent[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }
    }
}
