﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Extensions = yatl.Environment.Tilemap.Hexagon.Extensions;
using GeneratingTile = yatl.Environment.Tilemap.Hexagon.Tile<yatl.Environment.Level.Generation.GeneratingTileInfo>;

namespace yatl.Environment.Level.Generation
{
    sealed class LevelGenerator
    {

        public static LevelGenerator NewDefault { get { return new LevelGenerator().WithDefaultSettings; } }

        public int Radius { get; set; }
        public float Openness { get; set; }

        public bool LogGeneration { get; set; }

        public LevelGenerator WithDefaultSettings
        {
            get
            {
                this.Radius = Settings.Game.Level.Radius;
                this.Openness = 0.4f;

                return this;
            }
        }

        public LevelGenerator Verbose
        {
            get { this.LogGeneration = true; return this; }
        }

        public Tilemap<TileInfo> Generate()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("Generating level with radius {0} ({1} tiles)...", this.Radius, Extensions.TileCountForRadius(this.Radius));

            var timer = SteppedStopwatch.StartNew();
            timer.Mute = !this.LogGeneration;

            #region initialise

            var tempMap = new Tilemap<GeneratingTileInfo>(this.Radius);

            timer.WriteStepToConsole("Made mutable tilemap ......... {0}");

            var tiles = tempMap.ToList();


            timer.WriteStepToConsole("Enumerated tiles ............. {0}");

            //var tilesSpiral = tempMap.TilesSpiralOutward.ToList();
            //var tilesDistance = tiles.OrderBy(
            //    t => (Settings.Game.Level.TileToPosition * t.Xy).LengthSquared
            //    ).ToList();

            //var infos = new List<GeneratingTileInfo>(tiles.Count);

            foreach (var tile in tiles)
            {
                var info = new GeneratingTileInfo();
                //infos.Add(info);
                tempMap[tile] = info;
            }
            timer.WriteStepToConsole("Filled mutable tiles ......... {0}");

            #endregion

            #region open walls

            new GeneratingTile(tempMap, 0, 0).OpenRandomSpanningTree();

            timer.WriteStepToConsole("Opened random spanning tree .. {0}");

            #endregion

            #region build

            var result = tempMap.Build();

            timer.WriteStepToConsole("Build immutable tilemap ...... {0}");

            #endregion
            
            timer.Mute = false;
            timer.WriteTotalToConsole("Generated level in {0}");
            Console.WriteLine("------------------------------");

            return result;
        }

    }
}
