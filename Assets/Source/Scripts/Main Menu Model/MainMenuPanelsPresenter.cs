using Assets.Source.Scripts.UI.Main_Menu;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Main_Menu_Model
{
    class MainMenuPanelsPresenter : MonoBehaviour
    {
        [SerializeField] private TutorialGameModePanel _tutorialGameMode;
        [SerializeField] private EasyGameModePanel _easyGameMode;
        [SerializeField] private MediumGameModePanel _mediumGameMode;
        [SerializeField] private HardGameModePanel _hardGameMode;
        [SerializeField] private FreeGameModePanel _freeGameMode;


    }
}
