using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WOAT
{
    // 플레이어의 퀵슬롯을 관리하는 클래스
    public class QuickSlot : MonoBehaviour
    {
        // 필드
        #region Variables
        // 클래스 컴포넌트
        private InventoryManager inventoryManager;

        // 퀵슬롯 라인렌더러 변수
        /*[SerializeField] private float width = 75;
        [SerializeField] private float radius = 200;
        [SerializeField] private float alpha = 0.2f;
        [SerializeField] private int pointCount = 30;
        [SerializeField] private Color color = Color.black;*/

        // 퀵슬롯 상태 필드
        private TextMeshProUGUI[] quickSlotsCountTexts;
        public Button[] quickSlots;
        public Dictionary<KeyValuePair<ItemKey, ItemValue>, int> quickDict = new();
        public GameObject quickSlotPanel;

        // 초기화용 필드
        [SerializeField]
        private Sprite emptySlotSprite;
        private const float initialAlphaValue = 0f;
        private const float alphaValue = 0.9f;        // 인벤토리에 담긴 아이템의 알파값
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            // 컴포넌트 초기화
            inventoryManager = GetComponentInParent<InventoryManager>();

            // 퀵슬롯 초기화
            quickSlots = GetComponentsInChildren<Button>();
            quickSlotsCountTexts = GetComponentsInChildren<TextMeshProUGUI>();
        }
        #endregion

        // 메서드
        #region Methods
        // 아이템 정보를 받는 메서드
        public void QuickFromInven(KeyValuePair<ItemKey, ItemValue> _)
        {
            CountItem();
        }

        // 퀵슬롯을 선택하는 메서드
        private void SelectQuickSlot(KeyValuePair<ItemKey, ItemValue> pair, int quickIndex)
        {
            // 해당 슬롯이 빈 상태라면 return
            if (!quickDict.ContainsValue(quickIndex)) return;
            inventoryManager.UseQuick(pair);
        }

        // 퀵슬롯의 등록 정보를 재배치하는 메서드
        public void ReArrQuickSlot(KeyValuePair<ItemKey, ItemValue> pair)
        {
            int targetIndex = FindSlotIndex(inventoryManager.thisSlot);
            var itemToSelect = CollectionUtility.FirstOrDefault(quickDict, kvp => kvp.Key.Key == pair.Key);
            var itemToRemove = CollectionUtility.FirstOrDefault(quickDict, kvp => kvp.Value == targetIndex);

            if (targetIndex != -1)
            {
                // 처음 등록하는 아이템이라면
                if (!quickDict.ContainsKey(pair))
                {
                    ClearQuickSlot(itemToRemove);
                    UpdateQuickSlot(pair, targetIndex);
                }

                // 이미 등록된 아이템이라면
                else
                {
                    if (!quickDict.ContainsKey(itemToRemove.Key))
                    {
                        ClearQuickSlot(itemToSelect);
                        UpdateQuickSlot(itemToSelect.Key, targetIndex);
                    }

                    else SwapQuickSlots(quickDict[pair], targetIndex);
                }
            }

            // 등록을 해제하는 경우
            else ClearQuickSlot(itemToSelect);

            AssignQuickSlots();
            CountItem();
        }

        // 두 퀵슬롯의 등록 정보를 교환하는 메서드
        private void SwapQuickSlots(int firstIndex, int secondIndex)
        {
            if (firstIndex == secondIndex)
                return;

            // 기존 아이템 참조 저장
            var firstItem = CollectionUtility.FirstOrDefault(quickDict, kvp => kvp.Value == firstIndex).Key;
            var secondItem = CollectionUtility.FirstOrDefault(quickDict, kvp => kvp.Value == secondIndex).Key;

            // 첫 번째 슬롯과 두 번째 슬롯의 참조를 교환
            if (firstItem.Key != null)
            {
                quickDict.Remove(firstItem);
                UpdateQuickSlot(firstItem, secondIndex);
            }

            if (secondItem.Key != null)
            {
                quickDict.Remove(secondItem);
                UpdateQuickSlot(secondItem, firstIndex);
            }
        }

        // 퀵슬롯 업데이트 메서드 (아이템 추가)
        private void UpdateQuickSlot(KeyValuePair<ItemKey, ItemValue> pair, int slotIndex)
        {
            quickSlots[slotIndex].image.overrideSprite = pair.Key.itemImage;
            ColorUtility.SetAlpha(quickSlots[slotIndex].image, alphaValue);

            quickDict.Add(pair, slotIndex);
        }

        // 퀵슬롯 업데이트 메서드 (아이템 제거)
        private void ClearQuickSlot(KeyValuePair<KeyValuePair<ItemKey, ItemValue>, int> kvp)
        {
            if (!quickDict.ContainsKey(kvp.Key)) return;

            quickSlotsCountTexts[kvp.Value].text = "";
            quickSlots[kvp.Value].image.overrideSprite = emptySlotSprite;
            ColorUtility.SetAlpha(quickSlots[kvp.Value].image, initialAlphaValue);

            quickDict.Remove(kvp.Key);
        }

        // 각 버튼의 번호에 맞는 인덱스를 자동으로 할당하는 메서드
        private void AssignQuickSlots()
        {
            foreach (var pair in quickDict)
            {
                int index = pair.Value;  // 이 변수를 반드시 따로 선언해줘야 람다 함수 안에서 올바르게 작동

                quickSlots[index].onClick.RemoveAllListeners();
                quickSlots[index].onClick.AddListener(() => SelectQuickSlot(pair.Key, index));
            }
        }

        // 슬롯의 인덱스 찾기
        private int FindSlotIndex(Button targetSlot)
        {
            for (int i = 0; i < quickSlots.Length; i++)
                if (quickSlots[i] == targetSlot)
                    return i;
            return -1;
        }

        // 퀵슬롯 카운트 메서드
        private void CountItem()
        {
            foreach (var pair in quickDict)
                quickSlotsCountTexts[pair.Value].text = pair.Key.Value.ItemCount;
        }

        public void SetPanelAlpha(bool isOpen)
        {
            Image panelImage = quickSlotPanel.GetComponent<Image>();

            switch (isOpen)
            {
                case true:
                    ColorUtility.SetAlpha(panelImage, 0.4f);
                    break;

                case false:
                    ColorUtility.SetAlpha(panelImage, 0);
                    break;
            }
        }

        /*public void ShowLine(bool isOpen)
        {
            UILineRenderer quickLine = ColorUtility.DrawUILine(this.gameObject, SetPoints(Vector2.zero, radius, pointCount), width, color, alpha);

            switch (isOpen)
            {
                case true:
                    quickLine.enabled = true;
                    break;

                case false:
                    quickLine.enabled = false;
                    break;
            }
        }

        public List<Vector2> SetPoints(Vector2 center, float r, int pointCount)
        {
            List<Vector2> points = new();

            float angle = (Mathf.PI / (2 * pointCount));
            float angleOffset = (Mathf.PI / 12);

            for (int i = 0; i < pointCount; i++)
            {
                points.Add(MathUtility.GetCirclePos(center, r, angleOffset + angle * i));
            }

            return points;
        }*/
        #endregion
    }
}