using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Core;
using Infrasructure;

namespace Maze.Gui
{
    public partial class MainWindow : Window
    {
        private const int MazeImageSize = 500;
        private const int WallThickness = 2;
        private const int SolutionLineThickness = 2;
        private const int BitmapDpi = 96;

        private Core.Maze _currentMaze;
        private readonly MazeFileService _mazeFileService;
        private readonly Core.MazeSolver _mazeSolver;
        
        private Core.Point? _startPoint;
        private Core.Point? _endPoint;
        private List<Core.Point> _solutionPath;
        private bool _isSelectingStartPoint = false;
        private bool _isSelectingEndPoint = false;

        public MainWindow()
        {
            InitializeComponent();
            _mazeFileService = new MazeFileService();
            _mazeSolver = new Core.MazeSolver();
            _solutionPath = new List<Core.Point>();
            GenerateNewMaze();
            
            MazeImage.MouseLeftButtonDown += MazeImage_MouseLeftButtonDown;
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            int rows = (int)rows_slider.Value;
            int cols = (int)cols_slider.Value;
            GenerateNewMaze(rows, cols);
        }

        private void GenerateNewMaze()
        {
            int rows = (int)rows_slider.Value;
            int cols = (int)cols_slider.Value;
            GenerateNewMaze(rows, cols);
        }

        private void GenerateNewMaze(int rows, int cols)
        {
            _currentMaze = new Core.Maze(rows, cols);
            Random rand = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (j == cols - 1)
                        _currentMaze.RightWalls[i, j] = true; // правый край - всегда стена
                    else
                        _currentMaze.RightWalls[i, j] = rand.Next(2) == 0;

                    if (i == rows - 1)
                        _currentMaze.BottomWalls[i, j] = true; // нижний край - всегда стена
                    else
                        _currentMaze.BottomWalls[i, j] = rand.Next(2) == 0;
                }
            }

            _startPoint = null;
            _endPoint = null;
            _solutionPath.Clear();
            _isSelectingStartPoint = false;
            _isSelectingEndPoint = false;

            DrawMaze();
        }

        private void DrawMaze()
        {
            if (_currentMaze == null) return;

            int rows = _currentMaze.Rows;
            int cols = _currentMaze.Cols;

            double scaleX = (double)MazeImageSize / cols;
            double scaleY = (double)MazeImageSize / rows;
            double scale = Math.Min(scaleX, scaleY);

            double cellSize = scale;

            double mazeWidth = cols * cellSize;
            double mazeHeight = rows * cellSize;
            double offsetX = (MazeImageSize - mazeWidth) / 2;
            double offsetY = (MazeImageSize - mazeHeight) / 2;

            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, MazeImageSize, MazeImageSize));

                Pen wallPen = new Pen(Brushes.Black, WallThickness);
                wallPen.StartLineCap = PenLineCap.Flat;
                wallPen.EndLineCap = PenLineCap.Flat;

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        double leftX = offsetX + col * cellSize + WallThickness / 2.0;
                        double rightX = offsetX + (col + 1) * cellSize - WallThickness / 2.0;
                        double topY = offsetY + row * cellSize + WallThickness / 2.0;
                        double bottomY = offsetY + (row + 1) * cellSize - WallThickness / 2.0;

                        if (_currentMaze.RightWalls[row, col])
                        {
                            double x = offsetX + (col + 1) * cellSize - WallThickness / 2.0;
                            dc.DrawLine(wallPen, new System.Windows.Point(x, topY), new System.Windows.Point(x, bottomY));
                        }

                        if (_currentMaze.BottomWalls[row, col])
                        {
                            double y = offsetY + (row + 1) * cellSize - WallThickness / 2.0;
                            dc.DrawLine(wallPen, new System.Windows.Point(leftX, y), new System.Windows.Point(rightX, y));
                        }
                    }
                }

                double leftBorderX = offsetX + WallThickness / 2.0;
                for (int row = 0; row < rows; row++)
                {
                    double topY = offsetY + row * cellSize + WallThickness / 2.0;
                    double bottomY = offsetY + (row + 1) * cellSize - WallThickness / 2.0;
                    dc.DrawLine(wallPen, new System.Windows.Point(leftBorderX, topY), new System.Windows.Point(leftBorderX, bottomY));
                }

                double topBorderY = offsetY + WallThickness / 2.0;
                for (int col = 0; col < cols; col++)
                {
                    double leftX = offsetX + col * cellSize + WallThickness / 2.0;
                    double rightX = offsetX + (col + 1) * cellSize - WallThickness / 2.0;
                    dc.DrawLine(wallPen, new System.Windows.Point(leftX, topBorderY), new System.Windows.Point(rightX, topBorderY));
                }

                if (_solutionPath != null && _solutionPath.Count > 1)
                {
                    Pen solutionPen = new Pen(Brushes.Red, SolutionLineThickness);
                    solutionPen.StartLineCap = PenLineCap.Round;
                    solutionPen.EndLineCap = PenLineCap.Round;

                    for (int i = 0; i < _solutionPath.Count - 1; i++)
                    {
                        var current = _solutionPath[i];
                        var next = _solutionPath[i + 1];

                        double currentX = offsetX + current.Col * cellSize + cellSize / 2.0;
                        double currentY = offsetY + current.Row * cellSize + cellSize / 2.0;
                        double nextX = offsetX + next.Col * cellSize + cellSize / 2.0;
                        double nextY = offsetY + next.Row * cellSize + cellSize / 2.0;

                        dc.DrawLine(solutionPen, new System.Windows.Point(currentX, currentY), 
                                   new System.Windows.Point(nextX, nextY));
                    }
                }

                if (_startPoint.HasValue)
                {
                    var start = _startPoint.Value;
                    double startX = offsetX + start.Col * cellSize + cellSize / 2.0;
                    double startY = offsetY + start.Row * cellSize + cellSize / 2.0;
                    double startRadius = cellSize / 4.0;
                    dc.DrawEllipse(Brushes.Green, null, 
                                  new System.Windows.Point(startX, startY), startRadius, startRadius);
                }

                if (_endPoint.HasValue)
                {
                    var end = _endPoint.Value;
                    double endX = offsetX + end.Col * cellSize + cellSize / 2.0;
                    double endY = offsetY + end.Row * cellSize + cellSize / 2.0;
                    double endRadius = cellSize / 4.0;
                    dc.DrawEllipse(Brushes.Blue, null, 
                                  new System.Windows.Point(endX, endY), endRadius, endRadius);
                }
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(MazeImageSize, MazeImageSize, BitmapDpi, BitmapDpi, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            MazeImage.Source = bitmap;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog 
            { 
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Load Maze"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _currentMaze = _mazeFileService.Load(dialog.FileName);
                    
                    _startPoint = null; 
                    _endPoint = null;
                    _solutionPath.Clear();
                    _isSelectingStartPoint = false;
                    _isSelectingEndPoint = false;
                    
                    DrawMaze();

                    rows_slider.Value = _currentMaze.Rows;
                    cols_slider.Value = _currentMaze.Cols;

                    MessageBox.Show("Maze loaded successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading maze: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null)
            {
                MessageBox.Show("No maze to save. Please generate or load a maze first.", "Warning",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog 
            { 
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = "maze.txt",
                Title = "Save Maze"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _mazeFileService.Save(dialog.FileName, _currentMaze);
                    MessageBox.Show("Maze saved successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving maze: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null)
            {
                MessageBox.Show("No maze to solve. Please generate or load a maze first.", "Warning",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_startPoint.HasValue || !_endPoint.HasValue)
            {
                MessageBox.Show("Please select start and end points first.\nClick 'Select Start' then click on the maze, then 'Select End' and click again.", "Warning",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _solutionPath = _mazeSolver.Solve(_currentMaze, _startPoint.Value, _endPoint.Value);
                
                if (_solutionPath.Count == 0)
                {
                    MessageBox.Show("No path found between start and end points.", "No Solution",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    DrawMaze();
                    MessageBox.Show($"Path found! Solution length: {_solutionPath.Count} cells.", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error solving maze: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectStartButton_Click(object sender, RoutedEventArgs e)
        {
            _isSelectingStartPoint = true;
            _isSelectingEndPoint = false;
            MessageBox.Show("Click on the maze to select the start point (green circle).", "Select Start Point",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectEndButton_Click(object sender, RoutedEventArgs e)
        {
            _isSelectingEndPoint = true;
            _isSelectingStartPoint = false;
            MessageBox.Show("Click on the maze to select the end point (blue circle).", "Select End Point",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MazeImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_currentMaze == null) return;

            var position = e.GetPosition(MazeImage);
            
            int rows = _currentMaze.Rows;
            int cols = _currentMaze.Cols;
            double scaleX = (double)MazeImageSize / cols;
            double scaleY = (double)MazeImageSize / rows;
            double scale = Math.Min(scaleX, scaleY);
            double cellSize = scale;
            double mazeWidth = cols * cellSize;
            double mazeHeight = rows * cellSize;
            double offsetX = (MazeImageSize - mazeWidth) / 2;
            double offsetY = (MazeImageSize - mazeHeight) / 2;

            if (position.X < offsetX || position.X > offsetX + mazeWidth ||
                position.Y < offsetY || position.Y > offsetY + mazeHeight)
            {
                return;
            }

            int col = (int)((position.X - offsetX) / cellSize);
            int row = (int)((position.Y - offsetY) / cellSize);

            if (row < 0 || row >= rows || col < 0 || col >= cols)
                return;

            var point = new Core.Point(row, col);

            if (_isSelectingStartPoint)
            {
                _startPoint = point;
                _isSelectingStartPoint = false;
                _solutionPath.Clear(); 
                DrawMaze();
            }
            else if (_isSelectingEndPoint)
            {
                _endPoint = point;
                _isSelectingEndPoint = false;
                _solutionPath.Clear(); 
                DrawMaze();
            }
        }
    }
}