using System;
using Core;
using Xunit;

namespace Maze.Tests
{
    public class MazeSolverTests
    {
        [Fact]
        public void Solve_NullMaze_ThrowsArgumentNullException()
        {
            var solver = new MazeSolver();
            var start = new Point(0, 0);
            var end = new Point(1, 1);

            Assert.Throws<ArgumentNullException>(() => solver.Solve(null!, start, end));
        }

        [Fact]
        public void Solve_InvalidStartPoint_ThrowsArgumentException()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(5, 5);
            var invalidStart = new Point(10, 10);
            var end = new Point(1, 1);

            Assert.Throws<ArgumentException>(() => solver.Solve(maze, invalidStart, end));
        }

        [Fact]
        public void Solve_InvalidEndPoint_ThrowsArgumentException()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(5, 5);
            var start = new Point(0, 0);
            var invalidEnd = new Point(10, 10);

            Assert.Throws<ArgumentException>(() => solver.Solve(maze, start, invalidEnd));
        }

        [Fact]
        public void Solve_SameStartAndEnd_ReturnsSinglePoint()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(5, 5);
            var point = new Point(2, 2);

            var result = solver.Solve(maze, point, point);

            Assert.Single(result);
            Assert.Equal(point, result[0]);
        }

        [Fact]
        public void Solve_SimpleHorizontalPath_ReturnsCorrectPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(3, 3);
            
            maze.RightWalls[0, 0] = false;
            maze.RightWalls[0, 1] = false;

            var start = new Point(0, 0);
            var end = new Point(0, 2);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Solve_SimpleVerticalPath_ReturnsCorrectPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(3, 3);
            
            maze.BottomWalls[0, 0] = false;
            maze.BottomWalls[1, 0] = false;

            var start = new Point(0, 0);
            var end = new Point(2, 0);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Solve_NoPath_ReturnsEmptyList()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(3, 3);
            
            // Все стены остаются на месте, путь невозможен
            var start = new Point(0, 0);
            var end = new Point(2, 2);

            var result = solver.Solve(maze, start, end);

            Assert.Empty(result);
        }

        [Fact]
        public void Solve_ComplexPath_ReturnsValidPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(5, 5);
            
            maze.RightWalls[0, 0] = false; 
            maze.BottomWalls[0, 1] = false;
            maze.RightWalls[1, 1] = false;
            maze.BottomWalls[1, 2] = false;

            var start = new Point(0, 0);
            var end = new Point(2, 2);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
            
            for (int i = 0; i < result.Count - 1; i++)
            {
                var current = result[i];
                var next = result[i + 1];
                
                int rowDiff = Math.Abs(current.Row - next.Row);
                int colDiff = Math.Abs(current.Col - next.Col);
                
                Assert.True((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1));
            }
        }

        [Fact]
        public void Solve_PathFromCornerToCorner_ReturnsValidPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(4, 4);
            
            for (int i = 0; i < maze.Rows; i++)
            {
                for (int j = 0; j < maze.Cols - 1; j++)
                {
                    maze.RightWalls[i, j] = false;
                }
            }
            for (int i = 0; i < maze.Rows - 1; i++)
            {
                for (int j = 0; j < maze.Cols; j++)
                {
                    maze.BottomWalls[i, j] = false;
                }
            }

            var start = new Point(0, 0);
            var end = new Point(3, 3);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
        }

        [Fact]
        public void Solve_PathWithMultipleRoutes_ReturnsOnePath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(3, 3);
            
            maze.RightWalls[0, 0] = false;
            maze.RightWalls[0, 1] = false;
            maze.BottomWalls[0, 2] = false;
            maze.BottomWalls[1, 2] = false;
            
            maze.BottomWalls[0, 0] = false;
            maze.BottomWalls[1, 0] = false;
            maze.RightWalls[2, 0] = false;
            maze.RightWalls[2, 1] = false;

            var start = new Point(0, 0);
            var end = new Point(2, 2);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
        }

        [Fact]
        public void Solve_LargeMaze_ReturnsValidPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(10, 10);
            
            for (int i = 0; i < maze.Rows; i++)
            {
                for (int j = 0; j < maze.Cols - 1; j++)
                {
                    maze.RightWalls[i, j] = false;
                }
            }
            for (int i = 0; i < maze.Rows - 1; i++)
            {
                for (int j = 0; j < maze.Cols; j++)
                {
                    maze.BottomWalls[i, j] = false;
                }
            }

            var start = new Point(0, 0);
            var end = new Point(9, 9);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
        }

        [Fact]
        public void Solve_PathDoesNotGoThroughWalls()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(2, 2);
            maze.RightWalls[0, 0] = true;

            var result = solver.Solve(maze, new Point(0, 0), new Point(0, 1));

            Assert.Empty(result);
        }

        [Fact]
        public void Solve_PathThroughCenter_ReturnsValidPath()
        {
            var solver = new MazeSolver();
            var maze = new Core.Maze(5, 5);
            
            for (int j = 0; j < 3; j++)
            {
                maze.RightWalls[0, j] = false;
            }
            for (int i = 0; i < 3; i++)
            {
                maze.BottomWalls[i, 2] = false;
            }
            for (int j = 2; j < 5; j++)
            {
                maze.RightWalls[2, j] = false;
            }
            for (int i = 2; i < 5; i++)
            {
                maze.BottomWalls[i, 4] = false;
            }

            var start = new Point(0, 0);
            var end = new Point(4, 4);

            var result = solver.Solve(maze, start, end);

            Assert.NotEmpty(result);
            Assert.Equal(start, result[0]);
            Assert.Equal(end, result[result.Count - 1]);
        }
    }
}
