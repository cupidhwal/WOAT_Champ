using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WOAT
{
    // 플레이어의 인벤토리를 관리하는 클래스
    public class Inventory : MonoBehaviour
    {
        // 필드
        #region Variables
        private const float alphaValue = 0.9f;      // 인벤토리에 담긴 아이템의 알파값
        
        // 인벤토리 상태 필드
        public Dictionary<ItemKey, ItemValue> invenDict = new();

        // 클래스 컴포넌트
        private InventoryManager inventoryManager;

        // 인벤토리 요소
        private GameObject invenPanel;
        public GameObject thinkItem;                // 프리팹
        public List<GameObject> thinkItems = new(); // 새로운 인벤토리 시스템을 위한 리스트
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            // 컴포넌트 초기화
            inventoryManager = GetComponentInParent<InventoryManager>();
            invenPanel = GetComponentInChildren<RectTransform>().gameObject;
        }
        #endregion

        // 메서드
        #region Methods
        // 인벤토리에 새 아이템이 추가될 때 새 슬롯을 생성하는 메서드
        public void MakeSlot(ItemKey itemKey, ItemValue itemValue)
        {
            // 아이템을 추가하고
            invenDict.Add(itemKey, itemValue);

            // 습득한 아이템의 슬롯 인덱스를 지정
            itemValue.itemIndex = invenDict.Count - 1;

            // 새 슬롯 UI 오브젝트를 생성
            thinkItems.Add(Instantiate(thinkItem, invenPanel.transform));

            // 인벤토리 상태 업데이트
            UpdateSlot(itemKey, itemValue.itemIndex);

            // 카트리지나 펄이라면 인덱스를 0, 1로 갱신
            SortSlot(itemKey, itemValue);

            // 인벤토리 재배치
            MathUtility.ReArrObjects(100, 150, 80, thinkItems);

            // 슬롯 활성화 상태 통일
            CheckSlot(itemValue.itemIndex);
        }

        // 슬롯을 삭제하는 메서드
        public void TakeSlot(ItemKey selectedItemKey)
        {
            foreach (KeyValuePair<ItemKey, ItemValue> pair in invenDict)
            {
                if (pair.Key == selectedItemKey)
                {
                    // 슬롯 제거
                    Destroy(thinkItems[pair.Value.itemIndex]);
                    thinkItems.RemoveAt(pair.Value.itemIndex);
                }

                else if (pair.Value.itemIndex > invenDict[selectedItemKey].itemIndex)
                    pair.Value.itemIndex--;
            }

            // 해당 Element 제거
            invenDict.Remove(selectedItemKey);

            // 만약 아이템을 전부 다 썼다면 인벤토리를 자동으로 비활성화
            if (invenDict.Count == 0)
                inventoryManager.EmptySignal();
        }

        // 습득한 아이템이 카트리지, 펄이라면 인덱스를 0, 1로 수정하는 메서드
        public void SortSlot(ItemKey itemKey, ItemValue itemValue)
        {
            if (invenDict.Count < 2) return;

            else
            {
                var initialZero = CollectionUtility.FirstOrDefault(invenDict, pair => pair.Value.itemIndex == 0);
                var initialOne = CollectionUtility.FirstOrDefault(invenDict, pair => pair.Value.itemIndex == 1);

                // 습득한 아이템의 ID가 0 또는 1인지 확인하고 교환
                if (itemKey.itemID == 0 && itemValue.itemIndex != 0)
                {
                    (itemValue.itemIndex, initialZero.Value.itemIndex) = (initialZero.Value.itemIndex, itemValue.itemIndex);
                    CollectionUtility.SwapListElements(thinkItems, itemValue.itemIndex, initialZero.Value.itemIndex);
                }

                else if (itemKey.itemID == 1 && itemValue.itemIndex != 1)
                {
                    (itemValue.itemIndex, initialOne.Value.itemIndex) = (initialOne.Value.itemIndex, itemValue.itemIndex);
                    CollectionUtility.SwapListElements(thinkItems, itemValue.itemIndex, initialOne.Value.itemIndex);
                }

                else return;
            }
        }

        // 인벤토리를 업데이트하는 메서드
        public void UpdateSlot(ItemKey itemKey, int slotIndex)
        {
            // 퀵슬롯 리스트의 해당 인덱스에 아이콘 업데이트
            Image image = thinkItems[slotIndex].GetComponent<Image>();
            image.overrideSprite = itemKey.itemImage;

            // 업데이트 된 아이콘의 알파값 조정
            ColorUtility.SetAlpha(image, alphaValue);

            CountItem(itemKey, slotIndex);
        }

        // 슬롯 개폐 상태를 통일하는 메서드
        private void CheckSlot(int index)
        {
            thinkItems[index].SetActive(false);

            bool isAnyOpen = CollectionUtility.FirstOrNull(thinkItems, obj => obj.activeSelf) != null;

            foreach (GameObject obj in thinkItems)
                obj.SetActive(isAnyOpen);
        }

        // 아이템 수량을 표기하는 메서드
        public void CountItem(ItemKey itemKey, int index)
        {
            // 아이템의 수량이 2개 이상이라면 개수를 표기하고
            if (invenDict[itemKey].itemCount > 1)
                thinkItems[index].GetComponentInChildren<TextMeshProUGUI>().text = invenDict[itemKey].ItemCount;

            // 1개라면 표기 안 함
            else
                thinkItems[index].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        #endregion
    }
}