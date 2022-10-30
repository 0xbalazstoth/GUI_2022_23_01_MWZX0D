﻿using Model.Game.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.Exceptions;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Classes
{
    public class SaveHandler : ISaveHandler
    {

        private const string SAVE_FOLDER = "Saves";

        public string[] LoadSaves()
        {
            bool exists = Directory.Exists(SAVE_FOLDER);

            if (!exists)
            {
                string[] getfiles = Directory.GetFiles(SAVE_FOLDER);
                throw new NoSaveException("No save to load");

            }
            
            var files = Directory.GetFiles(SAVE_FOLDER);

            if (files.Length>0)
            {
                return files;
            }
            else
            {
                throw new NoSaveException("No save to load");
            }

        }

        public void NewGame(string saveName)
        {
            // Check if saves folder exists
            bool exists = Directory.Exists(SAVE_FOLDER);

            if (!exists)
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }

            // Create json file
            if (!String.IsNullOrEmpty(saveName) && saveName.Length >= 3 && saveName != "")
            {
                if (!File.Exists(SAVE_FOLDER + "/" + saveName + ".json"))
                {
                    JObject saveObject = new JObject(new JProperty("player", saveName));

                    //string json = JsonConvert.SerializeObject(saveName);
                    File.WriteAllText(SAVE_FOLDER + "/" + saveName + ".json", saveObject.ToString());
                }
                else
                {
                    throw new SaveAlreadyExistsException("Save already exists!");
                }
            }
            else
            {
                throw new SaveAlreadyExistsException("Username should be longer than 3 characters!");
            }
        }

        public void Save(string saveName, GameModel gameModel)
        {
            // Name
            // Inventory items
            // Gun
            // Coins
            // HP
            // K/D = Kills - Deaths ratio

            // 1. Create save window
            // 2. Get saves from folder
            // 3. Load save
        }
    }
}
