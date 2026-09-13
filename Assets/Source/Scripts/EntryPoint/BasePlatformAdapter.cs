using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.UI.CanvasOption;
using Assets.Source.Scripts.UI.SoundView;
using Assets.Source.Scripts.Factory;
using UnityEngine;
using System;
using YG;
using Assets.Source.Scripts.UI.StoreView;
using Assets.Source.Scripts.Pool;

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
                ItemFactory.Initialize(MobileContent, 
                    MobileSelectItemPresenter);
                _soundSetter.ApplyAudioHandler(_mobileAudioViewHandler);
                OnMobileSelected();
            }
            else
            {
                UseDesktopMode();
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
            if (MobileCanvas == null)
                throw new ArgumentNullException(nameof(MobileCanvas));

            if (DesktopCanvas == null)
                throw new ArgumentNullException(nameof(DesktopCanvas));

            if (ItemFactory == null)
                throw new ArgumentNullException(nameof(ItemFactory));

            if (MobileContent == null)
                throw new ArgumentNullException(nameof(MobileContent));

            if (DesktopContent == null)
                throw new ArgumentNullException(nameof(DesktopContent));

            if (DesktopSelectItemPresenter == null)
                throw new ArgumentNullException(nameof(DesktopSelectItemPresenter));

            if (MobileSelectItemPresenter == null)
                throw new ArgumentNullException(nameof(MobileSelectItemPresenter));
        }
    }
}
