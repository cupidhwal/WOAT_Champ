using UnityEngine;

namespace WOAT
{
    // 잔여 전기를 충전하는 차세대 전지, 펄을 관장하는 클래스
    public class Pearl : MonoBehaviour, IInteractable
    {
        // 필드
        #region Variables
        // 이 아이템의 기본 정보
        private ItemKey itemData;

        // 아이템 데이터베이스 참조
        public ItemDatabase itemDatabase;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Awake()
        {
            // 최초에 아이템 데이터베이스로부터 자신을 정의
            itemData = itemDatabase.itemList[1];
        }
        #endregion

        // 메서드
        #region Methods
        public ItemKey GetItemData()
        {
            return itemData;
        }

        public void UseItemData()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}