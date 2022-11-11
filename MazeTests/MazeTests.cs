using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameComponents;

namespace GameTests
{
    [TestClass]
    public class MazeTests
    {

        Random r = new Random();
        Maze.Direction[] allDirections = {   Maze.Direction.East, 
                                              Maze.Direction.South, 
                                              Maze.Direction.North, 
                                              Maze.Direction.West };
        [TestMethod]
        public void MazeConstructionTest()
        {
            int rows = 5;
            int cols = 5;
            Maze maze = new Maze(rows, cols);
        }

        [TestMethod]
        public void MazeSolvingTest()
        {
            int rows = 5;
            int cols = 5;
            Maze maze = new Maze(rows, cols);
            maze.InitializeMaze();
            maze.Actor.Cell = maze.EndPosition;
            Assert.IsTrue(maze.MazeSolved());

        }

        /// <summary>
        /// Test the generation of the move history used
        /// to mark the correct path.
        /// </summary>
        [TestMethod]
        public void TestMoveHistoryGeneration()
        {
            int rows = 5;
            int cols = 5;

            Maze maze = new Maze(rows, cols);
            maze.InitializeMaze();

            Assert.IsTrue(maze.MoveHistory.Count == 1);

            while (!maze.MoveActor(allDirections[r.Next(0, allDirections.Length)]))
            {
                /// Keep trying moves until we find one.
            }

            Assert.IsTrue(maze.MoveHistory.Count == 2);

        }

        /// <summary>
        /// Test the generation of the move history used
        /// to mark the correct path.
        /// </summary>
        [TestMethod]
        public void TestMovingActorThroughWalls()
        {
            int rows = 1;
            int cols = 2;

            Maze maze = new Maze(rows, cols);
            maze.InitializeMaze();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    maze.GetMaze()[i, j].TopWall = true;
                    maze.GetMaze()[i, j].LeftWall = true;
                    maze.GetMaze()[i, j].RightWall = true;
                    maze.GetMaze()[i, j].BottomWall = true;
                }

            Assert.IsTrue(!maze.MoveActor(Maze.Direction.South));
            Assert.IsTrue(!maze.MoveActor(Maze.Direction.North));
            Assert.IsTrue(!maze.MoveActor(Maze.Direction.West));
            Assert.IsTrue(!maze.MoveActor(Maze.Direction.East));

            maze.Actor.NumberOfShells = 10;

            if (maze.Actor.Cell.Col == 0)
                maze.Actor.ShotDirection = Maze.Direction.East;
            else
                maze.Actor.ShotDirection = Maze.Direction.West;

            Assert.IsTrue(maze.BlastWall());
            Assert.IsTrue(maze.MoveActor(maze.Actor.Cell.Col == 0 ? Maze.Direction.East : Maze.Direction.West));

        }

        /// <summary>
        /// Tests the ability of the tank to blast
        /// through blocked walls.
        /// </summary>
        [TestMethod]
        public void TestBlastingThroughWalls()
        {
            int rows = 1;
            int cols = 2;

            Maze maze = new Maze(rows, cols);
            maze.InitializeMaze();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    maze.GetMaze()[i, j].TopWall = true;
                    maze.GetMaze()[i, j].LeftWall = true;
                    maze.GetMaze()[i, j].RightWall = true;
                    maze.GetMaze()[i, j].BottomWall = true;
                }

            maze.Actor.NumberOfShells = 10;

            if (maze.Actor.Cell.Col == 0)
                maze.Actor.ShotDirection = Maze.Direction.East;
            else
                maze.Actor.ShotDirection = Maze.Direction.West;

            Assert.IsTrue(maze.BlastWall());
            Assert.IsTrue(maze.MoveActor(maze.Actor.Cell.Col == 0 ? Maze.Direction.East : Maze.Direction.West));

        }




    }






}

