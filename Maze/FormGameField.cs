using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GameComponents;

namespace Maze
{
    public partial class FormGameField : Form
    {

        private GameComponents.Maze maze;
        private Renderer renderer;
        public static readonly int NumberOfCells = 10;
        public event MazeSolvedEventHandler Solved;
        public event CannonPrimedEventHandler CannonAimed;

        private static readonly string WinMessage = "Chúc mừng bạn đã giải phóng miền Nam!";

        public FormGameField()
        {

            maze = new GameComponents.Maze(NumberOfCells, NumberOfCells);
            
            CannonAimed = new CannonPrimedEventHandler(CannonPrimed);
            Solved = new MazeSolvedEventHandler(DisplayWin);

            maze.CannonPrimedEventHandler += CannonAimed;

            InitializeComponent();
            CreateMaze();
            InitializeRenderer();
            SetMazeSolvedEventListener();
            SetFormSize();
        }

        private void SetFormSize()
        {
            this.Height = (NumberOfCells + 1) * Renderer.CellWallHeight + Renderer.EmptyTopSpaceHeight;
            this.Width = (NumberOfCells + 1) * Renderer.CellWallWidth;
        }

        /// Initialize the graphics renderer.

        private void InitializeRenderer()
        {
            renderer = new Renderer();
        }


        /// Set the event listener.

        private void SetMazeSolvedEventListener()
        {
            Solved += new MazeSolvedEventHandler(CannonPrimed);
        }


        /// Creates the maze.

        private void CreateMaze()
        {
            maze.InitializeMaze();
        }


        /// Handles pressing keys, will move the actor throughout the maze.

        private void frmGameField_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool actorMoved = false;

            switch(e.KeyChar.ToString().ToUpper())
            {
                case "S":
                    actorMoved = maze.MoveActor(GameComponents.Maze.Direction.South);
                    break;
                case "W":
                    actorMoved = maze.MoveActor(GameComponents.Maze.Direction.North);
                    break;
                case "A":
                    actorMoved = maze.MoveActor(GameComponents.Maze.Direction.West);
                    break;
                case "D":
                    actorMoved = maze.MoveActor(GameComponents.Maze.Direction.East);
                    break;

                case " ":
                    actorMoved = maze.BlastWall();
                    break;
                case "6":
                    maze.AimCannon(GameComponents.Maze.Direction.East);
                    break;
                case "4":
                    maze.AimCannon(GameComponents.Maze.Direction.West);
                    break;
                case "8":
                    maze.AimCannon(GameComponents.Maze.Direction.North);
                    break;
                case "2":
                    maze.AimCannon(GameComponents.Maze.Direction.South);
                    break;

                case "R":
                    CreateMaze();
                    actorMoved = true;
                    break;

                case "H":
                    maze.HintThePath();
                    break;
            }

            if(actorMoved)           
            Invalidate();

            if(maze.MazeSolved())
            {
                if(Solved != null)
                {
                    Solved();
                }

                CreateMaze();
                Invalidate();
            }
        }


        /// Event listener for window painting. Fires when changes to the painted area occur.

        private void frmGameField_Paint(object sender, PaintEventArgs e)
        {
            renderer.DrawMaze(e.Graphics, maze.GetMaze());
            renderer.DrawHint(e.Graphics, maze.HintPath);
            renderer.DrawMoveHistory(e.Graphics, maze.MoveHistory);
            renderer.DrawActor(e.Graphics, maze.Actor);
            renderer.DrawEndPosition(e.Graphics, maze.EndPosition);

            if (maze.MazeSolved())
            {
                Renderer.DrawWin(e.Graphics, WinMessage);
            }
            else
            {
                Renderer.DisplayHintStatus(e.Graphics, maze.Actor);
                Renderer.DisplayCannonStatus(e.Graphics, maze.Actor);
                Renderer.DisplayCostStatus(e.Graphics, maze.Actor);
            }

        }

        private void CannonPrimed()
        {
            Refresh();
        }

        private void DisplayWin()
        {
            Refresh();
            Thread.Sleep(1000);
        }
    }
}
