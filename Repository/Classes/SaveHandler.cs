using Model.Game.Classes;
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
            if (exists)
            {
                return Directory.GetFiles(SAVE_FOLDER).Select(Path.GetFileNameWithoutExtension).ToArray();
            }
            else
            {
                throw new NoSaveException("Save folder not exists!");
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

        public void Save(string saveName, IGameModel gameModel)
        {
            // Check if saves folder exists
            bool exists = Directory.Exists(SAVE_FOLDER);

            if (exists)
            {
                // Check if save exists
                if (File.Exists(SAVE_FOLDER + "/" + saveName + ".json"))
                {
                    // Load save
                    JObject saveObject = JObject.Parse(File.ReadAllText(SAVE_FOLDER + "/" + saveName + ".json"));

                    // Save player name
                    saveObject["player"] = saveName;

                    // Save inventory items
                    var inventory = gameModel.Player.Inventory.Items;
                    JArray inventoryItems = new JArray();
                    foreach (var item in inventory)
                    {
                        // Create JObject with itemType and itemAmount
                        JObject itemObject = new JObject(new JProperty("itemType", item.Value.ItemType.ToString()), new JProperty("itemQuantity", item.Value.Quantity));

                        // Add JObject named as item to JArray
                        inventoryItems.Add(new JObject(new JProperty("item", itemObject)));
                    }
                    saveObject["inventory"] = inventoryItems;

                    // Save coins
                    saveObject["coins"] = gameModel.Player.CurrentCoins;

                    // Save XP level
                    saveObject["xp"] = gameModel.Player.CurrentXP;

                    //// Save K/D
                    //saveObject["kd"] = gameModel.Player.Kills - gameModel.Player.Deaths;

                    // Save
                    File.WriteAllText(SAVE_FOLDER + "/" + saveName + ".json", saveObject.ToString());
                }
                else
                {
                    throw new NoSaveException("Save not exists!");
                }
            }
            else
            {
                throw new NoSaveException("Save folder not exists!");
            }
        }
    }
}
