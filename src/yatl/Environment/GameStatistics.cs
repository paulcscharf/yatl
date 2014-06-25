using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace yatl.Environment
{
    sealed class GameStatistics
    {
        public GameState.GameOverState GameOverState { get; set; }
        public bool DynamicMusic { get; set; }
        public double TimeInLight { get; set; }
        public double TimeInDarkness { get; set; }
        public double TotalDamageTaken { get; set; }

        public GameStatistics()
        {
            this.DynamicMusic = Settings.Game.DynamicMusic;
            this.GameOverState = GameState.GameOverState.Undetermined;
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists("stats"))
                    Directory.CreateDirectory("stats");

                var filename = "stats/" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.ff") + ".json";

                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Converters.Add(new StringEnumConverter());

                using (var file = new StreamWriter(filename))
                {
                    serializer.Serialize(file, this);
                }

                Console.WriteLine("saved statistics at: {0}", filename);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save statistics: {0}", e.Message);
                throw;
            }
        }
    }
}
