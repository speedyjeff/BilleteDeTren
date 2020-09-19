using engine.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteDeTren
{
    static class TrenElementExtensions
    {
        public static TrenElement SetCoordinates(this TrenElement tren, int top, int left, int width, int height)
        {
            // construct points
            tren.Dimension = new BoardCell(new Point[]
            {
                new engine.Common.Point(x:left, y:top, z:0),
                new engine.Common.Point(x:left, y:top + height, z:0),
                new engine.Common.Point(x:left + width, y:top + height, z:0),
                new engine.Common.Point(x:left + width, y:top, z:0)
            });
            return tren;
        }

        public static TrenElement SetCoordinates(this TrenElement tren, Point[] points)
        {
            tren.Dimension = new BoardCell(points);
            return tren;
        }
    }

    static class BdtBoardGenerator
    {
        public static void Generate(int width, int height, int numPlayers, out BdtBoard board, out BoardCell[][] cells)
        {
            // create the board
            board = new BdtBoard(numPlayers);

            board.BeginSetup();
            {
                // cities
                var miami = board.AddCity("Miami").SetCoordinates(top: 100, left: 100, width: 100, height: 100);
                var olympia = board.AddCity("Olympia").SetCoordinates(top: 100, left: 400, width: 100, height: 100);
                var austin = board.AddCity("Austin").SetCoordinates(top: 400, left: 400, width: 100, height: 100);
                var kirkland = board.AddCity("Kirland").SetCoordinates(top: 400, left: 100, width: 100, height: 100);

                // tracks
                board.AddTrack(color: TrenColor.Red, position: TrackPosition.Middle, length: 3, new TrenElement[] { miami, olympia });
                board.AddTrack(color: TrenColor.Green, position: TrackPosition.Middle, length: 4, new TrenElement[] { olympia, austin });
                board.AddTrack(color: TrenColor.Yellow, position: TrackPosition.Middle, length: 3, new TrenElement[] { austin, kirkland });
                board.AddTrack(color: TrenColor.Pink, position: TrackPosition.Middle, length: 4, new TrenElement[] { kirkland, miami });

                // destinations
                board.AddDestination(miami, austin);
                board.AddDestination(miami, olympia);
                board.AddDestination(miami, kirkland);
                board.AddDestination(olympia, austin);
                board.AddDestination(olympia, kirkland);
                board.AddDestination(austin, kirkland);
            }
            board.EndSetup();

            // button locations
            var left = 10;
            var top = height - 100;
            var bwidth = 100;
            var bheight = 50;
            var buttons = new Dictionary<TrenElementButtonType, BoardCell>()
            {
                { TrenElementButtonType.DestinationDraw, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*0)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*0)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*0)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*0)) + bwidth, y:top, z:0)
            }) }, 
                { TrenElementButtonType.TrenBlindDraw, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*1.1f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*1.1f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*1.1f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*1.1f)) + bwidth, y:top, z:0)
            }) }, 
                { TrenElementButtonType.TrenDraw1, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*2.2f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*2.2f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*2.2f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*2.2f)) + bwidth, y:top, z:0)
            }) },
                { TrenElementButtonType.TrenDraw2, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*3.3f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*3.3f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*3.3f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*3.3f)) + bwidth, y:top, z:0)
            }) },
                { TrenElementButtonType.TrenDraw3, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*4.4f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*4.4f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*4.4f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*4.4f)) + bwidth, y:top, z:0)
            }) },
                { TrenElementButtonType.TrenDraw4, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*5.5f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*5.5f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*5.5f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*5.5f)) + bwidth, y:top, z:0)
            }) },
                { TrenElementButtonType.TrenDraw5, new BoardCell(new Point[]
            {
                new engine.Common.Point(x:(left + (bwidth*6.6f)), y:top, z:0),
                new engine.Common.Point(x:(left + (bwidth*6.6f)), y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*6.6f)) + bwidth, y:top + bheight, z:0),
                new engine.Common.Point(x:(left + (bwidth*6.6f)) + bwidth, y:top, z:0)
            }) }
            };

            // convert to cells
            cells = Generate(board, buttons);
        }

        #region private
        private static BoardCell[][] Generate(BdtBoard board, Dictionary<TrenElementButtonType, BoardCell> buttonDimensions )
        { 
            // convert to cells
            var cells = new BoardCell[ board.GetLength() ][];

            for (int row = 0; row < cells.Length; row++)
            {
                // create column
                cells[row] = new BoardCell[board.GetLength(row)];

                for (int col = 0; col < cells[row].Length; col++)
                {
                    var elem = board.GetAt(row, col);
                    BoardCell dim = null;

                    // create the array
                    switch (elem.Type)
                    {
                        case TrenElementType.City:
                            dim = elem.Dimension;
                            break;
                        case TrenElementType.Track:
                            // construct the track that connects the two cities

                            // get the bounds
                            var top = Single.MaxValue;
                            var endTop = 0f;
                            var left = Single.MaxValue;
                            var endLeft = 0f;
                            var elemWidth = 0f;
                            var elemHeight = 0f;
                            foreach (var city in elem.Connections)
                            {
                                top = Math.Min(top, city.Dimension.Top);
                                endTop = Math.Max(endTop, city.Dimension.Top);
                                left = Math.Min(left, city.Dimension.Left);
                                endLeft = Math.Max(endLeft, city.Dimension.Left);
                                elemWidth += city.Dimension.Width;
                                elemHeight += city.Dimension.Height;
                            }
                            elemWidth /= ((float)elem.Connections.Count);
                            elemHeight /= ((float)elem.Connections.Count);

                            // add tracks between
                            var xgap = (endLeft - left) / cells[row].Length;
                            var ygap = (endTop - top) / cells[row].Length;
                            var trackWidth = 10f;
                            var trackHeight = 10f;

                            // adjust to the edge and column number
                            left += (elemWidth) + (xgap * col);
                            top += (elemHeight) + (ygap * col);


                            dim = new BoardCell(
                                points: new engine.Common.Point[]
                                {
                                    new engine.Common.Point(x:left, y:top, z:0),
                                    new engine.Common.Point(x:left, y:top + trackHeight, z:0),
                                    new engine.Common.Point(x:left + trackWidth, y:top + trackHeight, z:0),
                                    new engine.Common.Point(x:left + trackWidth, y:top, z:0)
                                }
                            );
                            break;

                        case TrenElementType.Destination:
                        case TrenElementType.Button:
                            if (!buttonDimensions.TryGetValue(elem.ButtonType, out dim)) throw new Exception("Unknow button type : " + elem.ButtonType);
                            break;

                        default:
                            throw new Exception("Unknown TrenElementType : " + elem.Type);
                    } // case

                    // set
                    if (dim == null) throw new Exception("Failed to determine dimensions");
                    cells[row][col] = dim;
                } // for
            } // for

            return cells;
        }
        #endregion
    }
}
