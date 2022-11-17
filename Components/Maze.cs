using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameComponents;
using System.Drawing;

namespace GameComponents
{

    public class Maze
    {
        private Tank actor; 
        private Cell[,] maze;
        private Cell goal; 
        private int rows;
        private int cols;
        private Random random;
        private List<Cell> moveHistory;
        private List<Cell> hintPath;

        public event CannonPrimedEventHandler CannonPrimedEventHandler;
        private MazeGenerator mazeGenerator;


        /// The possible directions to travel.

        public enum Direction
        {
            North, South, West, East, None
        }


        /// Creates a maze with the specified dimensions

        public Maze(int rows, int cols)
        {
            random = new Random();
            mazeGenerator = new MazeGenerator(rows, cols);

            this.rows = rows;
            this.cols = cols;

        }


        /// Creates the maze for the game.

        public void InitializeMaze()
        {
            mazeGenerator = new MazeGenerator(rows, cols);
            maze = mazeGenerator.GenerateMaze();
            InitializeActor();
            SetEndPosition();
        }

        /// Returns whether the actor occupies the cell designated as the end cell.

        public bool MazeSolved()
        {
            return actor.Cell.Row == goal.Row && actor.Cell.Col == goal.Col;
        }

        /// Sets the end position of the maze. Will continually find a random cell until the end position is not equal with the start position.
        
        private void SetEndPosition()
        {
            do          
                goal = maze[random.Next(rows), random.Next(cols)];
            while (goal == actor.Cell);
        }


        /// Set the actor

        private void InitializeActor()
        {
            actor = new Tank(maze[random.Next(0, cols - 1),random.Next(0, rows - 1)]);
            moveHistory = new List<Cell>();
            moveHistory.Add(actor.Cell);
        }


        /// Directs the cannon in one of the enumerated directions of the maze. Used to prime the cannon before shooting a wall.
        
        public void AimCannon(Direction direction)
        {
            if(actor.NumberOfShells > 0)
            {
                actor.ShotDirection = direction;

                if (CannonPrimedEventHandler != null)
                    CannonPrimedEventHandler();
            }
   
        }


        /// Returns the actor.

        public Tank Actor
        {
            get { return actor; }
            set { actor = value; }
        }


        /// Fire the tank's cannon to destroy the wall in the direction the cannon is aimed towards.

        public bool BlastWall()
        {
            if (actor.NumberOfShells > 0 && actor.ShotDirection != Direction.None)
            {
                DestroyWall(actor.Cell, actor.ShotDirection);
                actor.ShotDirection = Direction.None;
                actor.NumberOfShells--;
            }

            return true;
        }

        /// Draw the hint path.

        public bool HintThePath()
        {
            //Xử lý thuật toán ở đây theo các bước:
            //1. Nhận biết vị trí hiện tại
            //2. Lưu đường đi
            //3. Vẽ đường đi
            if (actor.NumberOfHints > 0)
            {
                DestroyWall(actor.Cell, actor.ShotDirection);

                actor.NumberOfHints--;
            }

            return true;
        }


        /// Destroys a target wall

        private void DestroyWall(Cell sourceCell, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    DestroyNorthWall(sourceCell);
                    break;             
                case Direction.South:
                    DestroySouthWall(sourceCell);
                    break;
                case Direction.West:
                    DestroyWestWall(sourceCell);
                    break; 
                case Direction.East:
                    DestroyEastWall(sourceCell);
                    break;
                case Direction.None:
                    break;
                default:
                    break;
            }
        }


        /// Destroy the wall south of the actor's current cell.

        private void DestroyNorthWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].TopWall = false;
            if (sourceCell.Row > 0)
                maze[sourceCell.Row - 1, sourceCell.Col].BottomWall = false;
        }

        /// Destroy the wall north of the actor's current cell.
        
        private void DestroySouthWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].BottomWall = false;
            if (sourceCell.Row < rows - 1)
                maze[sourceCell.Row + 1, sourceCell.Col].TopWall = false;
        }


        /// Destroy the wall west of the actor's current cell.
        
        private void DestroyWestWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].LeftWall = false;
            if (sourceCell.Col > 0)
                maze[sourceCell.Row, sourceCell.Col - 1].RightWall = false;
        }


        /// Destroy the wall east of the actor's cell.

        private void DestroyEastWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].RightWall = false;
            if (sourceCell.Col < cols - 1)
                maze[sourceCell.Row, sourceCell.Col + 1].LeftWall = false;
        }


        /// Attempt to move the actor in the supplied direction.

        public bool MoveActor(Direction direction)
        {
            bool moved;

            if (moved = IsPathFree(actor.Cell.Row, actor.Cell.Col, direction))
            {
   
               switch (direction)
                {
                    case Direction.North:
                        actor.Cell = maze[actor.Cell.Row - 1, actor.Cell.Col];
                        break;
                    case Direction.South:
                        actor.Cell = maze[actor.Cell.Row + 1, actor.Cell.Col];
                        break;
                    case Direction.West:
                        actor.Cell = maze[actor.Cell.Row, actor.Cell.Col - 1];
                        break;
                    case Direction.East:
                        actor.Cell = maze[actor.Cell.Row, actor.Cell.Col + 1];
                        break;
                    default:
                        break;
                }

                actor.LastDirectionMoved = direction;

                if (moveHistory.Contains(actor.Cell))
                {
                    int firstTempPosition = moveHistory.IndexOf(moveHistory.First(number => number == actor.Cell));
                    int lastPosition = moveHistory.Count - moveHistory.IndexOf(moveHistory.Last(number => number == actor.Cell));
                    moveHistory.RemoveRange(firstTempPosition, lastPosition);
                    moveHistory.Add(actor.Cell);              
                }
                  

                else
                    moveHistory.Add(actor.Cell);
                
            }

            return moved;
        }


        /// Checks to see if the actor can move to the given row or column by checking if it is a valid cell and if the wall configuration will allow the actor to travel there.
        
        private bool IsPathFree(int row, int col, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (row > 0 && !maze[row - 1, col].BottomWall);
                case Direction.South:
                    return (row < rows - 1 && !maze[row + 1, col].TopWall);
                case Direction.East:
                    return (col < cols - 1 && !maze[row, col + 1].LeftWall);
                case Direction.West:
                    return (col > 0 && !maze[row, col - 1].RightWall);
                default:
                    return false;
            }
        }


        /// Returns the array of cells.
        
        public Cell[,] GetMaze()
        {
            return maze;
        }


        /// This cell represents the target cell for the actor to reach.
        
        public Cell EndPosition
        {
            get { return goal; }
            set { goal = value; }
        }


        /// Returns the history of actor moves.

        public List<Cell> MoveHistory
        {
            /// TODO: Implement a return of a generic list.
            /// http://stackoverflow.com/questions/16806786/dont-expose-generic-list-why-to-use-collectiont-instead-of-listt-in-meth
            get { return moveHistory; }
        }

        public List<Cell> HintPath
        {
            get => hintPath;
        }
    }
}
