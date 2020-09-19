using engine.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteDeTren
{
    enum TrenColor { White, Black, Yellow, Green, Orange, Blue, Red, Pink, Wild, Neutral };
    enum TrackPosition { Middle, Top, Bottom };
    enum TrenElementType { City, Track, Destination, Button };
    enum TrenElementButtonType { DestinationDraw, TrenBlindDraw, TrenDraw1, TrenDraw2, TrenDraw3, TrenDraw4, TrenDraw5 };

    class TrenElement
    {
        public TrenElement()
        {
            Connections = new List<TrenElement>();
        }

        // base information
        public TrenElementType Type { get; internal set; }
        public int Row { get; internal set; }
        public int Column { get; internal set; }

        // TrenElementType.City
        public string Name { get; internal set; }

        // TrenElementType.Track && TrenElementType.Destination
        public TrenColor Color { get; internal set; }
        public TrackPosition Position { get; internal set; }
        public int Length { get; internal set; }
        public bool InUse { get; internal set; }
        public List<TrenElement> Connections { get; private set; }

        // TrenElementType.Button
        public TrenElementButtonType ButtonType { get; internal set; }

        // ui data
        public BoardCell Dimension { get; set; }
    }
}
