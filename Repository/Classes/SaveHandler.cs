﻿using Model.Game.Classes;
using Model.Game.Enums;
using Model.Game.Interfaces;
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

        public IGameModel LoadSave(string saveName)
        {
            IGameModel loadedGame = new GameModel();

            string saveFileName = $"{SAVE_FOLDER}\\{saveName}.json";

            if (!File.Exists(saveFileName))
            {
                throw new NoSaveException($"Save {saveName} not exists!");
            }
            else
            {
                // Read json file
                string json = File.ReadAllText(saveFileName);

                // Deserialize json to object
                JObject jsonObj = JObject.Parse(json);

                // Inventory current capacity
                int inventoryCurrentCapacity = (int)jsonObj["inventoryCurrentCapacity"];

                // Inventory items
                JArray inventoryArray = (JArray)jsonObj["inventory"];
                loadedGame.Player = new PlayerModel();
                InventoryModel inventory = new InventoryModel();
                inventory.Items = new Dictionary<int, ICollectibleItem>();

                foreach (JObject itemObj in inventoryArray)
                {
                    JObject item = (JObject)itemObj["item"];
                    string itemTypeStr = (string)item["itemType"];
                    ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), itemTypeStr);
                    int itemQuantity = (int)item["itemQuantity"];
                    int itemId = (int)item["itemId"];
                    inventory.Items.Add(itemId, new CollectibleItemModel() { Id = itemId, Quantity = itemQuantity, ItemType = itemType });
                }

                // Coins
                int coins = (int)jsonObj["coins"];

                // XP
                int xp = (int)jsonObj["xp"];

                loadedGame.Player.Inventory = inventory;
                loadedGame.Player.CurrentCoins = coins;
                loadedGame.Player.CurrentXP = xp;
                loadedGame.Player.Inventory.Capacity = inventoryCurrentCapacity;
                loadedGame.Player.Name = saveName;
            }

            return loadedGame;
        }

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

                    // Save inventory current capacity
                    saveObject["inventoryCurrentCapacity"] = gameModel.Player.Inventory.Capacity;

                    // Save inventory items
                    var inventory = gameModel.Player.Inventory.Items;
                    JArray inventoryItems = new JArray();
                    foreach (var item in inventory)
                    {
                        // Create JObject with itemType and itemAmount
                        JObject itemObject = new JObject(new JProperty("itemType", item.Value.ItemType.ToString()), new JProperty("itemQuantity", item.Value.Quantity), new JProperty("itemId", item.Value.Id));

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
