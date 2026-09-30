using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.UI.CanvasOption;
using Assets.Source.Scripts.UI.SoundView;
using Assets.Source.Scripts.Factory;
using UnityEngine;
using System;
using YG;
using Assets.Source.Scripts.UI.StoreView;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.EntryPoint
{
    public abstract class BasePlatformAdapter : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] protected CanvasMobileSetter MobileCanvas;
        [SerializeField] protected CanvasDesktopSetter DesktopCanvas;
        [Header("Store installation")]
        [SerializeField] protected StoreItemFactory ItemFactory;
        [SerializeField] protected Transform DesktopContent;
        [SerializeField] protected Transform MobileContent;
        [SerializeField] protected ItemSelectionHandler DesktopSelectItemPresenter;
        [SerializeField] protected ItemSelectionHandler MobileSelectItemPresenter;
        [Header("Audio panels installation")]
        [SerializeField] private SoundSetter _soundSetter;
        [SerializeField] private VolumeSliderViewHandler _mobileAudioViewHandler;
        [SerializeField] private VolumeSliderViewHandler _desktopAudioViewHandler;

        protected void InitializeBase()
        {
            ValidateRequiredObjects();
            MobileCanvas.Disable();
            DesktopCanvas.Disable();

            if (YG2.envir.isMobile)
            {
                UseMobileMode();
                Guard.NotNull(_mobileAudioViewHandler, 
                    nameof(_mobileAudioViewHandler));
                ItemFactory.Initialize(MobileContent, 
                    MobileSelectItemPresenter);
                _soundSetter.ApplyAudioHandler(_mobileAudioViewHandler);
                OnMobileSelected();
            }
            else
            {
                UseDesktopMode();
                Guard.NotNull(_desktopAudioViewHandler, 
                    nameof(_desktopAudioViewHandler));
                ItemFactory.Initialize(DesktopContent, 
                    DesktopSelectItemPresenter);
                _soundSetter.ApplyAudioHandler(_desktopAudioViewHandler);
                OnDesktopSelected();
            }
        }

        protected virtual void UseMobileMode()
        {
            MobileCanvas.Enable();
        }

        protected virtual void UseDesktopMode()
        {
            DesktopCanvas.Enable();
        }

        protected abstract void OnMobileSelected();

        protected abstract void OnDesktopSelected();

        private void ValidateRequiredObjects()
        {
            Guard.NotNull(MobileCanvas, nameof(MobileCanvas));
            Guard.NotNull(DesktopCanvas, nameof(DesktopCanvas));
            Guard.NotNull(ItemFactory, nameof(ItemFactory));
            Guard.NotNull(MobileContent, nameof(MobileContent));
            Guard.NotNull(DesktopContent, nameof(DesktopContent));
            Guard.NotNull(DesktopSelectItemPresenter, 
                nameof(DesktopSelectItemPresenter));
            Guard.NotNull(MobileSelectItemPresenter, 
                nameof(MobileSelectItemPresenter));
            Guard.NotNull(_soundSetter, nameof(_soundSetter));
        }
    }
}
