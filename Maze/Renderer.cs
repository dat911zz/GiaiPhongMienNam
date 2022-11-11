using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameComponents;
using System.Resources;
using System.Media;

namespace Maze
{
    public class Renderer
    {
        /// Variables to Size the Various Elements
        public static readonly int CellWallHeight = 50; // the height of a single cell
        public static readonly int CellWallThickness = 10;
        public static readonly int CellWallWidth = 50; // the width of a single cell
        private static readonly int ActorCellPadding = 5 + CellWallThickness; // the amount of space between the actor and the wall
        public static readonly int EmptyTopSpaceHeight = 100; // the amount of space between the top of the screen
      
        /// Image File Names
        private static readonly string HorizontalBrickFile = "GrayHorizontalBrickTexture";
        private static readonly string VerticalBrickFileName = "GrayVerticalBrickTexture";
        private static readonly string EndPositionIconFileName = "Flag";
        private static readonly string ActorIconFileName = "Tank";
        private static readonly string VisistedIconFileName = "TankTread";
   
        /// Images to Render Maze
        private Image horizontalWallTexture;
        private Image verticalWallTexture;
        private Image actorImage;
        private Image endPositionImage;
        private Image visitedCellImage;
                              
        /// <summary>
        /// Used to render the maze components
        /// onto the form's graphics. 
        /// </summary>
        /// <param name="cols">The number of columns in the maze</param>
        /// <param name="rows">The number of rows in the maze</param>
        public Renderer()
        {
            SetImages();
        }

        /// <summary>
        /// Assigns the appropriate images
        /// to the image variables used within
        /// the renderer. Used to avoid having
        /// to repeatedly call out to the resource
        /// manager to get the appropriate resource.
        /// </summary>
        private void SetImages()
        {
            horizontalWallTexture = GetImage(HorizontalBrickFile);
            verticalWallTexture = GetImage(VerticalBrickFileName);
            visitedCellImage = GetImage(VisistedIconFileName);
            actorImage = GetImage(ActorIconFileName);
            endPositionImage = GetImage(EndPositionIconFileName);
            visitedCellImage = GetImage(VisistedIconFileName);
        }

        /// <summary>
        /// Returns an image from resources.
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static Image GetImage(String imageName)
        {
            ResourceManager rm = Properties.Resources.ResourceManager;
            // Get a Handle on Resources
            Image image = (Image)rm.GetObject(imageName);             // Get the resource by String
            return image;
        }

        /// <summary>
        /// Draws the maze on the form using
        /// the graphics.
        /// </summary>
        /// <param name="graphics">The form's graphics</param>
        /// <param name="maze">The array of cells representing the maze</param>
        public void DrawMaze(Graphics graphics, GameComponents.Cell[,] maze)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    Rectangle area = new Rectangle(j * CellWallWidth, i * CellWallHeight + EmptyTopSpaceHeight, CellWallWidth, CellWallHeight);                  
                    DrawCell(graphics, maze[i, j], area);
                }
            }
        }

        /// <summary>
        /// Draws the visited marker within each cell contained
        /// within the moveHistory. Used to show the visisted 
        /// locations on the maze.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="moveHistory">The collection of cells to draw.</param>
        public void DrawMoveHistory(Graphics graphics, List<Cell> moveHistory)
        {
            if(graphics != null && moveHistory != null)
            {
                foreach (Cell cell in moveHistory)
                {
                    DrawVisitedMarker(graphics, cell);
                }
            }

        }

        /// <summary>
        /// Draws a cell on the form.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="cell">The cell to draw.</param>
        private void DrawCell(Graphics graphics, Cell cell, Rectangle area)
        {
            DrawWalls(graphics, cell, area);
        }

        /// <summary>
        /// Draws the end position on the form.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="cell">The end position cell.</param>
        public void DrawEndPosition(Graphics graphics, Cell cell)
        {
            if(cell != null && graphics != null)
            {
                Rectangle area = new Rectangle(cell.Col * CellWallWidth, cell.Row * CellWallHeight + EmptyTopSpaceHeight, CellWallWidth, CellWallHeight);

                /// Padding so that image displays
                /// within the bounds of the cell.
                area.Width -= CellWallThickness;
                area.Height -= CellWallThickness;
                area.X += CellWallThickness;
                area.Y += CellWallThickness;

                graphics.DrawImage(endPositionImage, area);
            }

     
        }

        /// <summary>
        /// Displays the image used to indicate a visited
        /// location on the form.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="cell">The visited cell.</param>
        private void DrawVisitedMarker(Graphics graphics, Cell cell)
        {
            int visistedXCoordinate = cell.Col * CellWallWidth + ActorCellPadding;
            int visistedYCoordinate = cell.Row * CellWallHeight + ActorCellPadding + EmptyTopSpaceHeight;

            graphics.DrawImage(visitedCellImage, new Rectangle(visistedXCoordinate, visistedYCoordinate, 
                                                 CellWallWidth / 2, CellWallHeight / 2)); 
        }

        /// <summary>
        /// Renders the actor onto the game form. Will
        /// rotatate the actor according to the last
        /// direction traveled.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="actor">The actor to render.</param>
        public void DrawActor(Graphics graphics, Actor actor)
        {
            if(actor != null && graphics != null)
            {
                int actorXCoordinate = actor.Cell.Col * CellWallWidth + ActorCellPadding;
                int actorYCoordinate = actor.Cell.Row * CellWallHeight + ActorCellPadding + EmptyTopSpaceHeight;

                graphics.DrawImage(GetRotatedImage(actorImage, actor.LastDirectionMoved), new Rectangle(actorXCoordinate, actorYCoordinate,
                                                 CellWallWidth / 2, CellWallHeight / 2));    

            }
        }

        /// <summary>
        /// Draws the walls of a given cell on
        /// the form.
        /// </summary>
        /// <param name="graphics">The graphics of the form.</param>
        /// <param name="cell">The cell to be rendered.</param>
        private void DrawWalls(Graphics graphics, Cell cell, Rectangle area)
        {
            Rectangle brick;

            if (cell.LeftWall)
            {
                brick = new Rectangle(area.X, area.Y, CellWallThickness, CellWallHeight);
                graphics.DrawImage(verticalWallTexture, brick);
            }

            if (cell.TopWall)
            {
                brick = new Rectangle(area.X, area.Y, CellWallWidth, CellWallThickness);
                graphics.DrawImage(horizontalWallTexture, brick);

            }

            if (cell.RightWall)
            {
                brick = new Rectangle(area.X + area.Width, area.Y, CellWallThickness, CellWallHeight);
                graphics.DrawImage(verticalWallTexture, brick);
   
            }
 
            if(cell.BottomWall)
            {
                brick = new Rectangle(area.X, area.Y + area.Height, CellWallWidth, CellWallThickness);
                graphics.DrawImage(horizontalWallTexture, brick);
            }

        }

        /// <summary>
        /// Handles rotating the image according the 
        /// direction traveled. Used to draw the actor
        /// in a "moving" position on the screen.
        /// </summary>
        /// <param name="sourceImage">The source image</param>
        /// <param name="directionTravelled"></param>
        /// <returns></returns>
        public static Image GetRotatedImage(Image sourceImage, GameComponents.Maze.Direction directionTravelled)
        {
            Image rotatedImage = new Bitmap(sourceImage);

            switch (directionTravelled)
            {
                case GameComponents.Maze.Direction.North:
                    rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipY);
                    break;
                case GameComponents.Maze.Direction.South:
                    rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case GameComponents.Maze.Direction.West:
                    rotatedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                default:
                    break;
            }

            return rotatedImage;
        }

        /// <summary>
        /// Will display a message at the top of the screen
        /// in the defaul upper left hand corner displaying
        /// the message defined.
        /// </summary>
        /// <param name="graphics">The form's graphics.</param>
        /// <param name="message">The message to display.</param>
        public static void DrawWin(Graphics graphics, String message)
        {
            if(graphics != null && message != null)
            {
                Font winDrawFont = new Font("Consolas", 16);
                SolidBrush winDrawBrush = new SolidBrush(Color.White);
                PointF winDrawPoint = new PointF(0, EmptyTopSpaceHeight / 2);
                graphics.DrawString(message, winDrawFont, winDrawBrush, winDrawPoint);
            }

        }


        /// <summary>
        /// TODO: Move the string display and conditional
        /// cannon information outside of the renderer. 
        /// This is a piece of logic that doesn't belong here.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="actor"></param>
        public static  void DisplayCannonStatus(Graphics graphics, GameComponents.Tank actor)
        {

            Font winDrawFont = new Font("Consolas", 16);
            SolidBrush winDrawBrush = new SolidBrush(Color.White);
            PointF winDrawPoint = new PointF(0, 0);

            if(graphics != null && actor != null)
            {

                if (actor.NumberOfShells > 0 && actor.ShotDirection != GameComponents.Maze.Direction.None)
                {
                    graphics.DrawString("Pháo đã sẵn sàng!", winDrawFont, winDrawBrush, winDrawPoint);
                }
                else
                {
                    if (actor.NumberOfShells > 0)
                    {
                        graphics.DrawString(actor.NumberOfShells.ToString() + " viên đạn còn lại! ", winDrawFont, winDrawBrush, winDrawPoint);
                    }
                    else
                    {
                        graphics.DrawString("Hết đạn! Bấm R để làm lại cuộc đời.", winDrawFont, winDrawBrush, winDrawPoint);
                    }
                }
            }


       
        }

    }
}
