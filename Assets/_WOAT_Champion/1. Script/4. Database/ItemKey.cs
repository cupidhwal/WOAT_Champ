using UnityEngine;

namespace WOAT
{
    // 아이템의 형식을 지정하는 클래스
    // 본 클래스는 인벤토리 딕셔너리의 Key를 관장하므로
    // 반드시 고정 속성만 취급한다
    [System.Serializable]
    public class ItemKey
    {
        public int itemID;
        public string itemName;
        public string itemDescription;
        public Sprite itemImage;
        public GameObject itemPrefab;
        public GameObject itemPrefabAddOn;
        public GameObject itemPhantomPrefab;
    }
}