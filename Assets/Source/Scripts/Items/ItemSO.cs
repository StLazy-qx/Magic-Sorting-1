using Assets.Source.Scripts.UI.StoreView;
using UnityEngine;

namespace Assets.Source.Scripts.Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Store/Item")]
    public class ItemSO : ScriptableObject
    {
        [SerializeField] private string _id = System.Guid.NewGuid().ToString();
        [SerializeField] private ItemView _prefabView;
        [SerializeField] private Item _item;
        [SerializeField] private int _price;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Texture _texture;

        public string ID => _id;
        public ItemView ItemView => _prefabView;
        public Item Item => _item;
        public Sprite Icon => _icon;
        public int Price => _price;
        public Texture Skin => _texture;
    }
}