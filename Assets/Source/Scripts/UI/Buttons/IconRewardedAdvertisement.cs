using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class IconRewardedAdvertisement : MonoBehaviour
    {
        private const float DisableAlpha = 0.0f;
        private const float EnableAlpha = 1f;

        [SerializeField] private Image _backGroundImage;
        [SerializeField] private Image _rewardImage;

        public void Disable()
        {
            SetImageAlpha(_backGroundImage, DisableAlpha);
            SetImageAlpha(_rewardImage, DisableAlpha);
        }

        public void Enable()
        {
            SetImageAlpha(_backGroundImage, EnableAlpha);
            SetImageAlpha(_rewardImage, EnableAlpha);
        }

        private void SetImageAlpha(Image image, float alpha)
        {
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                alpha);
        }
    }
}