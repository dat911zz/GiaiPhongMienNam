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
        private static readonly string HintIconFileName = "Egg";
   
        /// Images to Render Maze
        private Image horizontalWallTexture;
        private Image verticalWallTexture;
        private Image actorImage;
        private Image endPositionImage;
        private Image visitedCellImage;
        private Image hintImage;
                              

        /// Used to render the maze components onto the form's graphics. 

        public Renderer()
        {
            SetImages();
        }

        /// Assigns the appropriate images to the image variables used within the renderer. Used to avoid having to repeatedly call out to the resource manager to get the appropriate resource.
        private void SetImages()
        {
            horizontalWallTexture = GetImage(HorizontalBrickFile);
            verticalWallTexture = GetImage(VerticalBrickFileName);
            visitedCellImage = GetImage(VisistedIconFileName);
            actorImage = GetImage(ActorIconFileName);
            hintImage = GetImage(HintIconFileName);
            endPositionImage = GetImage(EndPositionIconFileName);
        }

        /// Returns an image from resources.
        public static Image GetImage(String imageName)
        {
            ResourceManager rm = Properties.Resources.ResourceManager;
            // Get a Handle on Resources
            Image image = (Image)rm.GetObject(imageName);             // Get the resource by String
            return image;
        }

        
        /// Draws the maze on the form using the graphics.

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


        /// Draws the visited marker within each cell contained within the moveHistory. Used to show the visisted  locations on the maze.

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

        /// Draws path to go to the flag.

        public void DrawHint(Graphics graphics, List<Cell> move)
        {
            if (graphics != null && move != null)
            {
                foreach (Cell cell in move)
                {
                    DrawHintPath(graphics, cell);
                }
            }
        }

        /// Draws a cell on the form.

        private void DrawCell(Graphics graphics, Cell cell, Rectangle area)
        {
            DrawWalls(graphics, cell, area);
        }

        
        /// Draws the end position on the form.
        
        public void DrawEndPosition(Graphics graphics, Cell cell)
        {
            if(cell != null && graphics != null)
            {
                Rectangle area = new Rectangle(cell.Col * CellWallWidth, cell.Row * CellWallHeight + EmptyTopSpaceHeight, CellWallWidth, CellWallHeight);

                /// Padding so that image displays within the bounds of the cell.
                area.Width -= CellWallThickness;
                area.Height -= CellWallThickness;
                area.X += CellWallThickness;
                area.Y += CellWallThickness;

                graphics.DrawImage(endPositionImage, area);
            }

     
        }


        /// Displays the image used to indicate a visited location on the form.
        
        private void DrawVisitedMarker(Graphics graphics, Cell cell)
        {
            int visistedXCoordinate = cell.Col * CellWallWidth + ActorCellPadding;
            int visistedYCoordinate = cell.Row * CellWallHeight + ActorCellPadding + EmptyTopSpaceHeight;

            graphics.DrawImage(visitedCellImage, new Rectangle(visistedXCoordinate, visistedYCoordinate, 
                                                 CellWallWidth / 2, CellWallHeight / 2)); 
        }

        /// Displays the image used to indicate a visited location on the form.

        private void DrawHintPath(Graphics graphics, Cell cell)
        {
            int visistedXCoordinate = cell.Col * CellWallWidth + ActorCellPadding;
            int visistedYCoordinate = cell.Row * CellWallHeight + ActorCellPadding + EmptyTopSpaceHeight;

            graphics.DrawImage(hintImage, new Rectangle(visistedXCoordinate, visistedYCoordinate,
                                                 CellWallWidth / 2, CellWallHeight / 2));
        }

        /// Renders the actor onto the game form. Will rotatate the actor according to the last direction traveled.

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


        /// Draws the walls of a given cell on the form.

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


        /// Handles rotating the image according the  direction traveled. Used to draw the actor in a "moving" position on the screen.

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

        /// Will display a message at the top of the screen in the defaul upper left hand corner displaying the message defined.
        
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


        /// TODO: Move the string display and conditional cannon information outside of the renderer. 
        /// This is a piece of logic that doesn't belong here.

        public static void DisplayCannonStatus(Graphics graphics, GameComponents.Tank actor)
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
                        
                        graphics.DrawString(actor.ShellsUsed.ToString() + " viên đạn đã sử dụng! ", winDrawFont, winDrawBrush, winDrawPoint);
                    }
                    else
                    {
                        graphics.DrawString("Hết đạn! Bấm R để làm lại cuộc đời.", winDrawFont, winDrawBrush, winDrawPoint);
                    }
                }
            }
        }

        public static void DisplayHintStatus(Graphics graphics, GameComponents.Tank actor)
        {

            Font winDrawFont = new Font("Consolas", 16);
            SolidBrush winDrawBrush = new SolidBrush(Color.White);
            PointF winDrawPoint = new PointF(0, 40);

            if (graphics != null && actor != null)
            {
                if (actor.NumberOfHints > 0)
                {
                    graphics.DrawString("Nhấn H để sử dụng gợi ý đường đi!", winDrawFont, winDrawBrush, winDrawPoint);
                }
                else
                {
                    graphics.DrawString("Bạn không thể sử dụng gợi ý nữa!", winDrawFont, winDrawBrush, winDrawPoint);
                }
            }
        }
    }
}
