using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameComponents
{
    /// <summary>
    /// Used to create a maze to
    /// be used in the maze game.
    /// </summary>
    class MazeGenerator
    {       
        private Cell[,] maze; /// the maze
        private int rows;
        private int columns;
        private Random random;
        private int numberOfPassagesToSeal;

       /// <summary>
        /// Used to generate a maze with a random
        /// correct path from a start position
        /// to an end position.
        /// </summary>
        /// <param name="rows">The height of the maze</param>
        /// <param name="cols">The width of the maze</param>
        public MazeGenerator(int rows, int cols)
        {
            this.rows = rows;
            this.columns = cols;

            numberOfPassagesToSeal = rows * cols / 4;

            maze = new Cell[this.rows, this.columns];

            InitializeMaze();
            ClearVisisted();
        }


        /// <summary>
        /// Initializes all positions within the
        /// array of cells to new cells.
        /// </summary>
        public void InitializeMaze()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    maze[i, j] = new Cell(i, j);
        }

        /// <summary>
        /// Generates a maze with a random path
        /// from a random starting position to 
        /// a random end position.
        /// </summary>
        /// <returns></returns>
        public Cell[,] GenerateMaze()
        {
            random = new Random();
            Stack<Cell> mazePathStack = new Stack<Cell>(); /// used to hold maze position
            Cell firstMazePosition = GetFirstPosition(); /// the position to start from
            List <Cell> createdPath = new List<Cell>();
                                                        
            mazePathStack.Push(firstMazePosition);
            Maze.Direction currentDirection = Maze.Direction.None;

            while (mazePathStack.Count != 0)
            {
                mazePathStack.Peek().Visited = true; 
                int curRow = mazePathStack.Peek().Row;
                int curCol = mazePathStack.Peek().Col;


                if ((currentDirection = GetNextRandomDirection(this.maze, curRow, curCol)) == Maze.Direction.None)
      
                    mazePathStack.Pop();
                
                else
                {
                    int nextRow = curRow;
                    int nextCol = curCol;

                    switch (currentDirection)
                    {
                        case Maze.Direction.North:
                            nextRow--;
                            break;
                        case Maze.Direction.South:
                            nextRow++;
                            break;
                        case Maze.Direction.West:
                            nextCol--;
                            break;
                        case Maze.Direction.East:
                            nextCol++;
                            break;
                    }

                    mazePathStack.Push(maze[nextRow, nextCol]);
                    createdPath.Add(maze[nextRow, nextCol]);
                    SetWallsByDirection(maze[curRow, curCol], maze[nextRow, nextCol], currentDirection); // mark the walls appropriately.
                }

            }

            SealPassages(createdPath);

            return maze;
        }


        /// <summary>
        /// Randomly seals passages through the maze to 
        /// force the player to have to use their cannon
        /// to break down walls to move on to the next level.
        /// </summary>
        private void SealPassages(List<Cell> createdPathCells)
        {
            int passagesToPlace = numberOfPassagesToSeal;

            while (passagesToPlace != 0)
            {
                Cell pathCell = createdPathCells[random.Next(createdPathCells.Count)];

                passagesToPlace--;

                if(!pathCell.TopWall)
                {
                    maze[pathCell.Row, pathCell.Col].TopWall = true;

                    if(pathCell.Row > 0)                  
                        maze[pathCell.Row - 1, pathCell.Col].BottomWall = true;
                 
                    continue;
                }


                if (!pathCell.BottomWall)
                {
                    maze[pathCell.Row, pathCell.Col].BottomWall = true;

                    if (pathCell.Row < rows - 1)
                        maze[pathCell.Row + 1, pathCell.Col].TopWall = true;

                    continue;
                }

                if (!pathCell.LeftWall)
                {
                    maze[pathCell.Row, pathCell.Col].LeftWall = true;

                    if (pathCell.Col > 0)
                        maze[pathCell.Row, pathCell.Col - 1].RightWall = true;

                    continue;

                }
     
                if (!pathCell.RightWall)
                {
                    maze[pathCell.Row, pathCell.Col].RightWall = true;

                    if (pathCell.Col < 1)
                        maze[pathCell.Row, pathCell.Col + 1].LeftWall = true;

                    continue;
                }

                createdPathCells.Remove(pathCell);

            }
        }

        /// <summary>
        /// Marks all cell positions within
        /// the array of cells to visisted.
        /// Used to clear the visisted status
        /// after generating the maze pattern.
        /// </summary>
        private void ClearVisisted()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    maze[i, j].Visited = false;
        }

        /// <summary>
        /// Sets the walls of the maze cell
        /// appropriately depending on the 
        /// direction from the source cell 
        /// to the destination cell.
        /// </summary>
        /// <param name="source">The source cell.</param>
        /// <param name="destination">The destination cell.</param>
        /// <param name="direction">The direction moved.</param>
        public static void SetWallsByDirection(Cell source, Cell destination, Maze.Direction direction)
        {
            switch (direction)
            {
                case Maze.Direction.North:
                    source.TopWall = false;
                    destination.BottomWall = false;
                    break;
                case Maze.Direction.South:
                    source.BottomWall = false;
                    destination.TopWall = false;
                    break;
                case Maze.Direction.West:
                    source.LeftWall = false;
                    destination.RightWall = false;
                    break;
                case Maze.Direction.East:
                    source.RightWall = false;
                    destination.LeftWall = false;
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Returns a random available direction that 
        /// has not been visisted in the array of maze
        /// cells. Used in the recursize backtracker 
        /// maze method.
        /// </summary>
        /// <param name="row">The source row.</param>
        /// <param name="column">The source col.</param>
        /// <returns>The direction of the next available free cell.</returns>
        public Maze.Direction GetNextRandomDirection(Cell[,] mazeToCheck, int row, int column)
        {
            List<Maze.Direction> availablePosition = new List<Maze.Direction>();

            if (row > 0 && !mazeToCheck[row - 1, column].Visited)
                availablePosition.Add(Maze.Direction.North);

            if (row < rows - 1 && !mazeToCheck[row + 1, column].Visited)
                availablePosition.Add(Maze.Direction.South);

            if (column < columns - 1 && !mazeToCheck[row, column + 1].Visited)
                availablePosition.Add(Maze.Direction.East);

            if (column > 0 && !mazeToCheck[row, column - 1].Visited)
                availablePosition.Add(Maze.Direction.West);

            return availablePosition.Count == 0 ?
                Maze.Direction.None : availablePosition[random.Next(0, availablePosition.Count)];
        }

        /// <summary>
        /// Gets a random starting position for use
        /// in the backtracking algorithm.
        /// </summary>
        /// <param name="rows">The number of rows in the maze.</param>
        /// <param name="columns">The number of columns in the maze.</param>
        /// <returns></returns>
        private Cell GetFirstPosition()
        {
            Cell firstPosition = maze[random.Next(0, rows - 1), random.Next(0, columns - 1)]; 
            return firstPosition;
        }
       
    }
}
