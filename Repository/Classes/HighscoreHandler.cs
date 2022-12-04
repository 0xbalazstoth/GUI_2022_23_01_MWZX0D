using Model.Game.Classes;
using Newtonsoft.Json.Linq;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Classes
{
    public class HighscoreHandler : IHighscoreHandler
    {
        private const string SAVE_FOLDER = "Saves";

        public List<HighscoreModel> LoadHighscoreStats()
        {
            List<HighscoreModel> highscores = new List<HighscoreModel>();

            bool exists = Directory.Exists(SAVE_FOLDER);
            if (exists)
            {
                var saveFiles = Directory.GetFiles(SAVE_FOLDER);

                foreach (var saveFile in saveFiles)
                {
                    HighscoreModel highscoreModel = new HighscoreModel();

                    // Read json file
                    string json = File.ReadAllText(saveFile);

                    // Deserialize json to object
                    JObject jsonObj = JObject.Parse(json);

                    // Player
                    string player = (string)jsonObj["player"];

                    int xp = 0;

                    if (jsonObj.ContainsKey("xp"))
                    {
                        xp = (int)jsonObj["xp"];
                    }

                    highscoreModel.Player = player;
                    highscoreModel.Highscore = xp;

                    highscores.Add(highscoreModel);
                }
            }

            return highscores;
        }
    }
}
