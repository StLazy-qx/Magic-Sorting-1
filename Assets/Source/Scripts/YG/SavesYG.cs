using Assets.Source.Scripts.YG;
using System.Collections.Generic;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private const float BeginVolume = 0.5f;

        public int MainPoints;
        public int ActualRoundNumber;
        public float MasterVolume = BeginVolume;
        public float AmbientVolume = BeginVolume;
        public float EffectVolume = BeginVolume;
        public string EquippedItem;
        public List<string> ItemIDs = new();

        private ScoreSection _score;
        private RoundSection _round;
        private AudioSettingsSection _audio;
        private InventorySection _inventory;

        public ScoreSection Score => _score ??= new ScoreSection(this);
        public RoundSection Round => _round ??= new RoundSection(this);
        public AudioSettingsSection Audio => _audio ??= new AudioSettingsSection(this);
        public InventorySection Inventory => _inventory ??= new InventorySection(this);

        public int Points => Score.Points;
        public string EquippedItemID => Inventory.EquippedItemID;

        public void EnableAutoSaveOnExit()
        {
            Application.quitting += OnApplicationQuit;
        }

        public void DisableAutoSaveOnExit()
        {
            Application.quitting -= OnApplicationQuit;
        }

        public void SaveScore(int points) 
            => Score.SaveScore(points);

        public void DecreaseScore(int points) 
            => Score.DecreaseScore(points);

        public void AddItem(string itemID) 
            => Inventory.AddItem(itemID);

        public void SaveEquippedItem(string itemID) 
            => Inventory.SaveEquippedItem(itemID);

        public IReadOnlyList<string> GetPurchasedItems() 
            => Inventory.GetPurchasedItems();

        public void SaveRoundNumber(int number) 
            => Round.SaveRoundNumber(number);

        public int GetRoundNumber() 
            => Round.GetRoundNumber();

        public void SaveMasterVolume(float value) 
            => Audio.SaveMasterVolume(value);

        public void SaveAmbientVolume(float value) 
            => Audio.SaveAmbientVolume(value);

        public void SaveEffectVolume(float value)
            => Audio.SaveEffectVolume(value);

        public void ForceSaveAudio()
           => Audio.ForceSaver();

        private void OnApplicationQuit()
        {
            Audio.ForceSaver();
            YG2.SaveProgress();
        }
    }
}