using engine.Common;
using engine.Winforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilleteDeTren
{
    public partial class BilleteDeTren : Form
    {
        public BilleteDeTren()
        {
            InitializeComponent();

            this.Width = 1024;
            this.Height = 800;
            this.Text = "Billete De Tren";
            // setting a double buffer eliminates the flicker
            this.DoubleBuffered = true;

            // create dimensions
            BdtBoardGenerator.Generate(Width, Height, numPlayers: 2, out BdtBoard, out Cells);

            // creat the new board
            Board = new engine.Common.Board(new BoardConfiguration()
            {
                Width = Width,
                Height = Height,
                Rows = 1,
                Columns = Cells.Length,
                EdgeAngle = 30,
                Background = RGBA.White,
                Cells = Cells
            });
            Board.OnCellClicked += Board_OnCellClicked;

            // link to this control
            UI = new UIHookup(this, Board);

            // paint the board
            for (int row = 0; row < Cells.Length; row++)
            {
                for (int col = 0; col < Cells[row].Length; col++) UpdateCell(row, col);
            }
        }

        #region private
        private UIHookup UI;
        private engine.Common.Board Board;
        private BdtBoard BdtBoard;
        private BoardCell[][] Cells;

        private void Board_OnCellClicked(int row, int col, float x, float y)
        {
            System.Diagnostics.Debug.WriteLine($"Click {row},{col} {x},{y}");
            // todo update the board
            BdtBoard.Next(row, col);
        }

        private void UpdateCell(int row, int col)
        {
            var cell = Cells[row][col];
            var elem = BdtBoard.GetAt(row, col);

            Board.UpdateCell(row, col, (img) =>
            {
                img.Graphics.Clear(RGBA.White);
                img.MakeTransparent(RGBA.White);
                var color = RGBA.White;
                switch (elem.Type)
                {
                    case TrenElementType.City: color = RGBA.Black; break;
                    case TrenElementType.Track: 
                        if (elem.InUse)
                        {
                            switch(elem.Color)
                            {
                                case TrenColor.Black: color = RGBA.Black; break;
                                case TrenColor.Blue: color = new RGBA() { B = 255, A = 255 }; break;
                                case TrenColor.Green: color = new RGBA() { G = 255, A = 255 }; break;
                                case TrenColor.Orange: color = new RGBA() { R = 255, G = 150, A = 255 }; break;
                                case TrenColor.Pink: color = new RGBA() { R = 200, G = 50, B = 50, A = 255 }; break;
                                case TrenColor.Red: color = new RGBA() { R = 255, A = 255 }; break;
                                case TrenColor.White: color = RGBA.White; break;
                                case TrenColor.Yellow: color = new RGBA() { R = 255, G = 255, A = 255 }; break;
                                default: throw new Exception("Unknown track color : " + elem.Color);
                            }
                        }
                        else color = new RGBA() { R = 150, G = 150, B = 150, A = 255 };
                        break;
                    case TrenElementType.Button: color = RGBA.Black; break;
                    case TrenElementType.Destination: break;
                    default: throw new Exception("Unknow tren type : " + elem.Type);
                }

                img.Graphics.Polygon(color, cell.NormalizedPoints, fill: true, border: true);
            });
        }

        #endregion
    }
}
