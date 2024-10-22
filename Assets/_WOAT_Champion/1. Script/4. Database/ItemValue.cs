using UnityEngine;

namespace WOAT
{
    // 아이템의 상태를 저장하는 클래스
    // 본 클래스는 인벤토리 딕셔너리의 Value를 관장하므로
    // 반드시 변동 속성만 취급한다
    public class ItemValue
    {
        // 필드
        #region Variables
        public int itemIndex = 0;
        public int itemCount = 1;
        #endregion

        // 속성
        #region Properties
        public string ItemCount => itemCount.ToString();
        #endregion

        // 메서드
        #region Mothods
        #region AboutItemCount
        // 아이템 수량을 갱신하는 메서드
        public void Count(int amount)
        {
            itemCount += amount;
            itemCount = Mathf.Clamp(itemCount, 0, 99);  // 수량을 0에서 99 사이로 제한
        }

        // 아이템이 더 사용 가능한지 체크하는 메서드
        public bool IsUsable()
        {
            return itemCount > 0;
        }
        #endregion
        #endregion
    }
}