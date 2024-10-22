using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace WOAT
{
    // 플레이어의 아이템 사용을 관장하는 클래스
    public class PlayerUse : MonoBehaviour
    {
        // 필드
        #region Variables
        // 인벤토리 클래스
        public ItemDatabase itemDatabase;
        public InventoryManager inventoryManager;
        private QuickSlot quickSlot;

        // 퀵슬롯
        private Button[] quickSlots;

        // 클래스 컴포넌트
        private Player player;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            // 클래스 컴포넌트 초기화
            player = GetComponent<Player>();
            quickSlot = inventoryManager.GetComponentInChildren<QuickSlot>();
            inventoryManager.SetPlayer(player, this);

            // 사용할 퀵슬롯을 모두 가져오기
            quickSlots = quickSlot.GetComponentsInChildren<Button>();
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnInventoryStarted(InputAction.CallbackContext _) => inventoryManager.ShowItem(inventoryManager.IsOpenInventory = !inventoryManager.IsOpenInventory);
        public void OnQuickSlot1DownStarted(InputAction.CallbackContext _) => quickSlots[0].onClick.Invoke();
        public void OnQuickSlot2DownStarted(InputAction.CallbackContext _) => quickSlots[1].onClick.Invoke();
        public void OnQuickSlot3DownStarted(InputAction.CallbackContext _) => quickSlots[2].onClick.Invoke();
        #endregion

        // 메서드
        #region Methods
        // 아이템을 사용하는 메서드
        public void UseItem(KeyValuePair<ItemKey, ItemValue> pair)
        {
            // 인벤토리에 아이템이 없다면 메서드 종료
            if (!inventoryManager.Inventory.invenDict.ContainsKey(pair.Key)) return;

            // 우선 입력된 아이템 정보를 찾고
            ItemKey thisItem = CollectionUtility.FirstOrDefault(inventoryManager.Inventory.invenDict, kvp => pair.Key == kvp.Key).Key;

            // 해당 아이템이 사용 가능한지 여부를 확인한 뒤
            if (thisItem.itemPrefab.TryGetComponent<IUsable>(out var usableItem))
            {
                // 아이템을 사용
                usableItem.UseItem(player);

                // 수량 갱신
                inventoryManager.CountItem(pair.Key, pair.Value.itemIndex);
            }

            // 사용할 수 없는 아이템이라면 메서드 종료
            else return;
        }
        #endregion
    }
}