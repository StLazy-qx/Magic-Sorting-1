using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
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
        public int SavedGameMode = -1;
        public int SavedDifficultyLevel = -1;
        public float MasterVolume = BeginVolume;
        public float AmbientVolume = BeginVolume;
        public float EffectVolume = BeginVolume;
        public string EquippedItem;
        public string SavedLanguage;
        public List<string> ItemIDs = new();

        private ScoreSection _score;
        private RoundSection _round;
        private AudioSettingsSection _audio;
        private InventorySection _inventory;
        private LanguageSection _language;
        private GameModeSection _mode;

        public GameModeSection Mode => _mode ??= new GameModeSection(this);
        public ScoreSection Score => _score ??= new ScoreSection(this);
        public RoundSection Round => _round ??= new RoundSection(this);
        public AudioSettingsSection Audio => _audio ??= new AudioSettingsSection(this);
        public InventorySection Inventory => _inventory ??= new InventorySection(this);
        public LanguageSection Language => _language ??= new LanguageSection(this);

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

        public void SaveLanguage(string language)
        => Language.SaveLanguage(language);

        public void SaveMode(GameMode mode, DifficultyLevel level)
            => Mode.SaveMode(mode, level);

        private void OnApplicationQuit()
        {
            Audio.ForceSaver();
            YG2.SaveProgress();
        }
    }
}