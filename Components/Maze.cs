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
        private Hint hint;
        private Cell[,] maze;
        private Cell[,] mazecopy;
        private Cell goal; 
        private int rows;
        private int cols;
        private Random random;
        private List<Cell> moveHistory;
        private List<Cell> hintPath;
        private List<Cell> freePath;

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
            foreach (Cell cell in maze)
                cell.Visited = false;
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

        private void InitializeHintEntity()
        {
            hint = new Hint(MoveHistory.Last());
            hintPath = new List<Cell>();
            hintPath.Add(hint.Cell);
            hint.Cell.Visited = true;
            hint.Cell.Heuristic = Math.Abs(hint.Cell.Row - goal.Row) + Math.Abs(hint.Cell.Col - goal.Col);
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
                actor.ShellsUsed++;
            }

            return true;
        }
        public Cell[,] DeepCoppy(Cell[,] maze)
        {
            int n = maze.Length / 10;
            Cell[,] tmpMaze = new Cell[n, n];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tmpMaze[i, j] = new Cell(maze[i, j]);
                }
            }
            return tmpMaze;
        }

        /// Draw the hint path.

        public bool HintThePath()
        {
            //Xử lý thuật toán ở đây theo các bước:
            //1. Nhận biết vị trí hiện tại
            if (actor.NumberOfHints > 0)
            {
                InitializeHintEntity();
                mazecopy = DeepCoppy(maze);//Using deep coppy
                //mazecopy[0, 2].Heuristic = -1;
                SolveMaze();

                actor.NumberOfHints--;
                maze = DeepCoppy(mazecopy);//Not using deep coppy


            }
            return true;
        }
        //Find the path or check all possible path by using A* Algorithm
        public List<Cell> FindPossiblePath(List<Cell> freePath)
        {

            do {
                //Check possible direction and add to OpenCells List
                if (IsPathFree(hint.Cell.Row, hint.Cell.Col, Direction.North) && maze[hint.Cell.Row - 1, hint.Cell.Col].Visited == false)
                {
                    freePath.Add(maze[hint.Cell.Row - 1, hint.Cell.Col]);
                    freePath.Last().Heuristic = Math.Abs(freePath.Last().Row - goal.Row) + Math.Abs(freePath.Last().Col - goal.Col);
                }

                if (IsPathFree(hint.Cell.Row, hint.Cell.Col, Direction.South) && maze[hint.Cell.Row + 1, hint.Cell.Col].Visited == false)
                {
                    freePath.Add(maze[hint.Cell.Row + 1, hint.Cell.Col]);
                    freePath.Last().Heuristic = Math.Abs(freePath.Last().Row - goal.Row) + Math.Abs(freePath.Last().Col - goal.Col);
                }

                if (IsPathFree(hint.Cell.Row, hint.Cell.Col, Direction.West) && maze[hint.Cell.Row, hint.Cell.Col - 1].Visited == false)
                {
                    freePath.Add(maze[hint.Cell.Row, hint.Cell.Col - 1]);
                    freePath.Last().Heuristic = Math.Abs(freePath.Last().Row - goal.Row) + Math.Abs(freePath.Last().Col - goal.Col);
                }

                if (IsPathFree(hint.Cell.Row, hint.Cell.Col, Direction.East) && maze[hint.Cell.Row, hint.Cell.Col + 1].Visited == false)
                {
                    freePath.Add(maze[hint.Cell.Row, hint.Cell.Col + 1]);
                    freePath.Last().Heuristic = Math.Abs(freePath.Last().Row - goal.Row) + Math.Abs(freePath.Last().Col - goal.Col);
                }

                freePath = freePath.OrderBy(x => x.Heuristic).ToList();

                //Move hint to Cell which has min Heuristic in freePath but hasn't visited and set visited = true
                if (freePath.Any(temp => temp.Row == goal.Row && temp.Col == goal.Col))
                {
                    hint.Cell = freePath.First(temp => temp.Row == goal.Row && temp.Col == goal.Col);
                }
                else if (!freePath.All(temp => temp.Visited))
                {
                    hint.Cell = freePath.First(temp => temp.Visited == false);
                    hint.Cell.Visited = true;
                }

            } while (!freePath.All(temp => temp.Visited) && !freePath.Any(temp => temp.Row == goal.Row && temp.Col == goal.Col)) ;

            return freePath;
        }

        //Auto destroy wall
        public void AutoDestroyWall()
        {
            if (freePath.Count != 0)
            {
                freePath = freePath.OrderBy(x => x.Heuristic).ToList();
                hint.Cell = freePath.First();
            }
            //Check where is the best direction to destroy wall
            if(hint.Cell.Row - 1 == goal.Row && hint.Cell.Col == goal.Col)
                hint.ShotDirection = Direction.North;
            else if(hint.Cell.Row + 1 == goal.Row && hint.Cell.Col == goal.Col)
                hint.ShotDirection = Direction.South;
            else if(hint.Cell.Row == goal.Row && hint.Cell.Col - 1 == goal.Col)
                hint.ShotDirection = Direction.West;
            else if(hint.Cell.Row == goal.Row && hint.Cell.Col + 1 == goal.Col)
                hint.ShotDirection = Direction.East;
            else
            {
                //North
                if (hint.Cell.Row > 0 && maze[hint.Cell.Row - 1, hint.Cell.Col].Heuristic < hint.Cell.Heuristic)
                {
                    hint.ShotDirection = Direction.North;
                }
                //South
                else if (hint.Cell.Row < 9 && maze[hint.Cell.Row + 1, hint.Cell.Col].Heuristic < hint.Cell.Heuristic)
                {
                    hint.ShotDirection = Direction.South;
                }
                //West
                else if (hint.Cell.Col > 0 && maze[hint.Cell.Row, hint.Cell.Col - 1].Heuristic < hint.Cell.Heuristic)
                {
                    hint.ShotDirection = Direction.West;
                }
                //East
                else if (hint.Cell.Col < 9 && maze[hint.Cell.Row, hint.Cell.Col + 1].Heuristic < hint.Cell.Heuristic)
                {
                    hint.ShotDirection = Direction.East;
                }
            }

            //Shoot
            CannonPrimedEventHandler();
            DestroyWall(hint.Cell, hint.ShotDirection);
            hint.ShotDirection = Direction.None;
            actor.NumberOfShells--;
            actor.ShellsUsed++;
        }

        // Let the Hint find the way
        public bool SolveMaze()
        {
            //Save current state
            Cell currentPosition = hint.Cell;
            freePath = new List<Cell>();


            do {    //Keep destroy wall and find (calculate) the path till there's no any free path left
                freePath = FindPossiblePath(freePath);
                
                if(!freePath.Any(temp => temp.Row == goal.Row && temp.Col == goal.Col))
                    AutoDestroyWall();

                foreach (Cell cell in freePath)
                    cell.Visited = false;

            } while (!freePath.Any(temp => temp.Row == goal.Row && temp.Col == goal.Col));

            hint.Cell = currentPosition;
            hintPath = FindPossiblePath(hintPath);

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
                    actor.NumberOfSteps -= 1;
                }


                else
                {
                    moveHistory.Add(actor.Cell);
                    actor.NumberOfSteps += 1;
                }
                
            }

            return moved;
        }

        public bool MoveHint(Direction direction)
        {
            bool moved;

            if (moved = IsPathFree(hint.Cell.Row, hint.Cell.Col, direction))
            {
                switch (direction)
                {
                    case Direction.North:
                        hint.Cell = maze[hint.Cell.Row - 1, hint.Cell.Col];
                        break;
                    case Direction.South:
                        hint.Cell = maze[hint.Cell.Row + 1, hint.Cell.Col];
                        break;
                    case Direction.West:
                        hint.Cell = maze[hint.Cell.Row, hint.Cell.Col - 1];
                        break;
                    case Direction.East:
                        hint.Cell = maze[hint.Cell.Row, hint.Cell.Col + 1];
                        break;
                    default:
                        break;
                }

                if (hintPath.Contains(hint.Cell))
                {
                    int firstTempPosition = hintPath.IndexOf(hintPath.First(number => number == hint.Cell));
                    int lastPosition = hintPath.Count - hintPath.IndexOf(hintPath.Last(number => number == hint.Cell));
                    hintPath.RemoveRange(firstTempPosition, lastPosition);
                    hintPath.Add(hint.Cell);
                }
                else
                    hintPath.Add(hint.Cell);

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
