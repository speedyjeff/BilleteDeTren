using engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BilleteDeTren
{
    // scores
    //  1=1, 2=2, 3=4, 4=7, 5=10, 6=15
    // cards
    //  white/black/yellow/green/orange/blue/red/pink=12
    //  wild=14
    // rules
    //  setup - 4 cards, 3 destinations (choose at least 2), 5 cards to choose from
    //  gameplay (choose 1) - draw 2 cards (from the faceup or draw pile), draw 3 destination (choose at least 1), place trains
    //    5 cards must have <2 wilds, can only take 1 wild from the faceup, if there are 3+ wilds (reset)

    enum BdtAction { UpdateCell, UpdateRow };

    class BdtBoard
    {
        public BdtBoard(int numPlayers)
        {
            // init
            State = StateMachineStates.Setup;

            // setup and add the special rows
            Elements = new List<List<TrenElement>>();
            Elements.Insert((int)SpecialRows.Cities, new List<TrenElement>());
            Elements.Insert((int)SpecialRows.Buttons, new List<TrenElement>());
            Elements.Insert((int)SpecialRows.Destinations, new List<TrenElement>());

            // add the buttons
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.DestinationDraw});
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenBlindDraw });
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenDraw1 });
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenDraw2 });
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenDraw3 });
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenDraw4 });
            Elements[(int)SpecialRows.Buttons].Add(new TrenElement() { Type = TrenElementType.Button, ButtonType = TrenElementButtonType.TrenDraw5 });

            // create the deck
            Deck = new List<TrenColor>();
            foreach(var color in new TrenColor[] { TrenColor.Black, TrenColor.Blue, TrenColor.Green, TrenColor.Orange, TrenColor.Pink, TrenColor.Red, TrenColor.White, TrenColor.Yellow, TrenColor.Wild})
            {
                for (int i = 0; i < (color == TrenColor.Wild ? CountWildCards : CountPerCard); i++) Deck.Add(color);
            }
            // shuffle
            var rand = new Random();
            Deck = Deck.OrderBy(c => rand.Next()).ToList();

            // init players
            Players = new List<BdtPlayer>();
            for (int i = 0; i < numPlayers; i++)
            {
                var player = new BdtPlayer();
                Players.Add(player);
                for (int j = 0; j < InitialCardCount; j++)
                {
                    if (Deck.Count == 0) throw new Exception("Ran out of cards");
                    player.Hand.Add(Deck[0]);
                    Deck.RemoveAt(0);
                }
            }
        }


        //
        // Setup
        //
        public TrenElement AddCity(string cityName)
        {
            if (State != StateMachineStates.Setup) throw new Exception("Must be in setup state");

            // create a city
            var row = (int)SpecialRows.Cities;
            var city = new TrenElement() { Type = TrenElementType.City, Name = cityName, Row = row, Column = Elements[row].Count };
            Elements[row].Add(city);
            return city;
        }

        public TrenElement AddTrack(TrenColor color, TrackPosition position, int length, TrenElement[] cities)
        {
            if (State != StateMachineStates.Setup) throw new Exception("Must be in setup state");
            if (cities == null || cities.Length == 0) throw new Exception("Must provide a valid list of cities");

            // create a track
            var row = Elements.Count;
            Elements.Add(new List<TrenElement>());
            var track = new TrenElement() { Type = TrenElementType.Track, Color = color, Position = position, Length = length , Row = row, Column = Elements[row].Count, InUse = true };
            // add the connecting cities
            for(int i=0; i<cities.Length; i++)
            {
                if (cities[i].Type != TrenElementType.City) throw new Exception("Must provide valid cities");
                track.Connections.Add(cities[i]);
            }
            // add
            Elements[row].Add(track);
            return track;
        }

        public TrenElement AddDestination(TrenElement city1, TrenElement city2)
        {
            if (State != StateMachineStates.Setup) throw new Exception("Must be in setup state");
            if (city1 == null || city1.Type != TrenElementType.City
                || city2 == null || city2.Type != TrenElementType.City) throw new Exception("Must have two valid cities");

            // create a new destination
            var elem = new TrenElement() { Type = TrenElementType.Destination };
            elem.Connections.Add(city1);
            elem.Connections.Add(city2);
            Elements[(int)SpecialRows.Destinations].Add(elem);

            return elem;
        }

        //
        // Access
        //
        public int GetLength()
        {
            return Elements.Count;
        }

        public int GetLength(int row)
        {
            if (row < 0 || row >= Elements.Count) throw new Exception("row out of range");

            // check if this is a track
            if (Elements[row][0].Type == TrenElementType.Track) return Elements[row][0].Length;

            // else return the count
            return Elements[row].Count;
        }

        public TrenElement GetAt(int row, int col)
        {
            // validate row
            if (row < 0 || row >= Elements.Count) throw new Exception("row out of range");

            // if the first element is a track, then regardless of the col return that element
            if (Elements[row][0].Type == TrenElementType.Track) return Elements[row][0];

            // validate col
            if (col < 0 || col >= Elements[row].Count) throw new Exception("col out of range");
            return Elements[row][col];
        }

        //
        // Game play
        //
        public void BeginSetup()
        {
            if (State != StateMachineStates.Setup) throw new Exception("Must be in setup state");
        }

        public void EndSetup()
        {
            if (State != StateMachineStates.Setup) throw new Exception("Must be in setup state");
            State = StateMachineStates.PickDestinations;
        }

        public BdtAction Next(int row, int col)
        {
            // todo
            return BdtAction.UpdateCell;
        }

        #region privates
        private enum SpecialRows { Cities = 0, Buttons = 1, Destinations = 2 };
        private List<List<TrenElement>> Elements;
        private List<TrenColor> Deck;
        private List<BdtPlayer> Players;

        private const int CountPerCard = 12;
        private const int CountWildCards = 14;
        private const int InitialCardCount = 5;

        private enum StateMachineStates { Setup, PickDestinations };
        private StateMachineStates State;

        #endregion
    }
}
