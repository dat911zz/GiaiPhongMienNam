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

        public event CannonPrimedEventHandler CannonPrimedEventHandler;
        private MazeGenerator mazeGenerator;

        /// <summary>
        /// The possible directions to travel.
        /// </summary>
        public enum Direction
        {
            North, South, West, East, None
        }

        /// <summary>
        /// Creates a maze with the specified dimensions
        /// </summary>
        /// <param name="rows">The number of rows of the maze.</param>
        /// <param name="cols"></param>
        public Maze(int rows, int cols)
        {
            random = new Random();
            mazeGenerator = new MazeGenerator(rows, cols);

            this.rows = rows;
            this.cols = cols;

        }

        /// <summary>
        /// Creates the maze for the game.
        /// </summary>
        public void InitializeMaze()
        {
            mazeGenerator = new MazeGenerator(rows, cols);
            maze = mazeGenerator.GenerateMaze();
            InitializeActor();
            SetEndPosition();
        }

        /// <summary>
        /// Returns whether the actor
        /// occupies the cell designated
        /// as the end cell.
        /// </summary>
        /// <returns></returns>
        public bool MazeSolved()
        {
            return actor.Cell.Row == goal.Row && actor.Cell.Col == goal.Col;
        }

        /// <summary>
        /// Sets the end position of the maze. Will continually
        /// find a random cell until the end position is not equal
        /// with the start position.
        /// </summary>
        private void SetEndPosition()
        {
            do          
                goal = maze[random.Next(rows), random.Next(cols)];
            while (goal == actor.Cell);
        }

        /// <summary>
        /// Set the actor
        /// </summary>
        private void InitializeActor()
        {
            actor = new Tank(maze[random.Next(0, cols - 1),random.Next(0, rows - 1)]);
            moveHistory = new List<Cell>();
            moveHistory.Add(actor.Cell);
        }


        /// <summary>
        /// Directs the cannon in one of the enumerated
        /// directions of the maze. Used to prime the 
        /// cannon before shooting a wall.
        /// </summary>
        /// <param name="direction"></param>
        public void AimCannon(Direction direction)
        {
            if(actor.NumberOfShells > 0)
            {
                actor.ShotDirection = direction;

                if (CannonPrimedEventHandler != null)
                    CannonPrimedEventHandler();
            }
   
        }

        /// <summary>
        /// Returns the actor.
        /// </summary>
        public Tank Actor
        {
            get { return actor; }
            set { actor = value; }
        }

        /// <summary>
        /// Fire the tank's cannon to destroy
        /// the wall in the direction the cannon
        /// is aimed towards.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Destroys a target wall
        /// </summary>
        /// <param name="sourceCell"></param>
        /// <param name="direction"></param>
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

        /// <summary>
        /// Destroy the wall south of the actor's
        /// current cell.
        /// </summary>
        /// <param name="sourceCell"></param>
        private void DestroyNorthWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].TopWall = false;
            if (sourceCell.Row > 0)
                maze[sourceCell.Row - 1, sourceCell.Col].BottomWall = false;
        }

        /// <summary>
        /// Destroy the wall north of the actor's
        /// current cell.
        /// </summary>
        /// <param name="sourceCell"></param>
        private void DestroySouthWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].BottomWall = false;
            if (sourceCell.Row < rows - 1)
                maze[sourceCell.Row + 1, sourceCell.Col].TopWall = false;
        }

        /// <summary>
        /// Destroy the wall west of the actor's
        /// current cell.
        /// </summary>
        /// <param name="sourceCell"></param>
        private void DestroyWestWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].LeftWall = false;
            if (sourceCell.Col > 0)
                maze[sourceCell.Row, sourceCell.Col - 1].RightWall = false;
        }

        /// <summary>
        /// Destroy the wall east of the 
        /// actor's cell.
        /// </summary>
        /// <param name="sourceCell"></param>
        private void DestroyEastWall(Cell sourceCell)
        {
            maze[sourceCell.Row, sourceCell.Col].RightWall = false;
            if (sourceCell.Col < cols - 1)
                maze[sourceCell.Row, sourceCell.Col + 1].LeftWall = false;
        }

        /// <summary>
        /// Attempt to move the actor in the supplied
        /// direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        /// <returns>Whether the actor has moved</returns>
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

        /// <summary>
        /// Checks to see if the actor can move to
        /// the given row or column by checking if 
        /// it is a valid cell and if the wall configuration
        /// will allow the actor to travel there.
        /// </summary>
        /// <param name="row">The destination row.</param>
        /// <param name="col">The destination column.</param>
        /// <param name="direction">The direction of the destination from source.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the array of cells.
        /// </summary>
        /// <returns></returns>
        public Cell[,] GetMaze()
        {
            return maze;
        }

        /// <summary>
        /// This cell represents
        /// the target cell for 
        /// the actor to reach.
        /// </summary>
        public Cell EndPosition
        {
            get { return goal; }
            set { goal = value; }
        }

        /// <summary>
        /// Returns the history of actor moves.
        /// </summary>
        public List<Cell> MoveHistory
        {
            /// TODO: Implement a return of a generic list.
            /// http://stackoverflow.com/questions/16806786/dont-expose-generic-list-why-to-use-collectiont-instead-of-listt-in-meth
            get { return moveHistory; }
        }
    }
}
