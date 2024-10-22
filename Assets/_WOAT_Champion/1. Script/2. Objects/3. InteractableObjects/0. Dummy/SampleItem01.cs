using UnityEngine;

namespace WOAT
{
    public class SampleItem01 : MonoBehaviour, IInteractable
    {
        private int itemID = 2; // 고유한 ID로 데이터베이스 내 아이템 참조
        private ItemKey itemData;
        public ItemDatabase itemDatabase; // 데이터베이스 참조

        private void Start()
        {
            // ID를 사용해서 ItemDatabase에서 아이템 데이터를 가져옴
            itemData = itemDatabase.itemList.Find(item => item.itemID == itemID);
        }

        public ItemKey GetItemData()
        {
            return itemData;
        }
    }
}