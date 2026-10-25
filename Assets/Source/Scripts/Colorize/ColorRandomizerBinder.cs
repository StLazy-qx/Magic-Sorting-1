using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.UI.Color_Mix_View;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRandomizerBinder : MonoBehaviour
    {
        [SerializeField] private ColorColumnDistributor _distributor;
        [SerializeField] private ColorMixView _view;

        private void Awake()
        {
            Guard.NotNull(_distributor, nameof(_distributor));
            Guard.NotNull(_view, nameof(_view));
        }

        //public void Configure(int countMixColor)
        //{
        //    Guard.Positive(countMixColor, nameof(countMixColor));

        //    if (_distributor.HasTarget == false)
        //        return;

        //    _view.Apply(_distributor.TargetColor, countMixColor);
        //}
    }
}
