using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

namespace WOAT
{
    // 플레이어의 인벤토리와 퀵슬롯을 운영하는 클래스
    public class InventoryManager : MonoBehaviour
    {
        // 필드
        #region Variables
        // 허상 아이템 관련 필드
        private GameObject itemPhantom;
        private IEnumerator phantomCor;
        private Vector3 cursorPosition = Vector3.zero;

        // 단순 변수
        private int? selectedSlotIndex = null;

        // 불리언 변수
        private bool isQuick = false;     // 슬롯 판정 변수
        private bool isSelected = false;        // 아이템 선택에 필요한 필드
        private bool isOpenInventory = false;

        // 컴포넌트
        public Button thisSlot;
        private EventSystem eventSystem;
        private GraphicRaycaster raycaster;

        // 클래스 컴포넌트
        private Player player;
        private PlayerUse playerUse;
        private QuickSlot quickSlot;
        private Inventory inventory;
        #endregion

        // 속성
        #region Properties
        public int? SelectedSlotIndex => selectedSlotIndex;
        public bool IsSelected => isSelected;
        public bool IsOpenInventory
        {
            get { return isOpenInventory; }
            set { isOpenInventory = value; }
        }
        public QuickSlot QuickSlot => quickSlot;
        public Inventory Inventory => inventory;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Awake()
        {
            // 초기화
            eventSystem = FindAnyObjectByType<EventSystem>();
            raycaster = GetComponentInChildren<GraphicRaycaster>();
            quickSlot = GetComponentInChildren<QuickSlot>();
            inventory = GetComponentInChildren<Inventory>();
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnCursorClickStarted(InputAction.CallbackContext _) => WhichSelect();

        public void OnCursorClickCanceled(InputAction.CallbackContext _) => isSelected = false;
        #endregion

        // 메서드
        #region Methods
        // 플레이어 세팅
        public void SetPlayer(Player player, PlayerUse playerUse)
        {
            this.player = player;
            this.playerUse = playerUse;
        }

        public void UseQuick(KeyValuePair<ItemKey, ItemValue> pair)
        {
            playerUse.UseItem(pair);
        }

        // 아이템 정보를 캡슐화
        private KeyValuePair<ItemKey, ItemValue> ItemData(ItemKey itemKey)
        {
            return new KeyValuePair<ItemKey, ItemValue>(itemKey, inventory.invenDict[itemKey]);
        }

        // 아이템 캡슐을 퀵슬롯으로 넘겨주는 메서드
        private void InvenToQuick(KeyValuePair<ItemKey, ItemValue> pair)
        {
            // 나중에는 인벤토리도 퀵슬롯과 동일한 계층으로 다루어
            // 정보 갱신을 함께 처리하는 것이 좋을 듯

            // 퀵슬롯에 해당 아이템의 참조 정보가 있다면
            if (quickSlot.quickDict.ContainsKey(pair))
            {
                // 현재 인벤에도 아이템이 남아 있다면
                if (inventory.invenDict.ContainsKey(pair.Key))
                {
                    // 퀵슬롯으로 아이템 정보 넘겨주기
                    quickSlot.QuickFromInven(pair);
                }

                // 인벤에는 아이템이 없다면
                else
                {
                    // 퀵슬롯의 해당 버튼 비활성화
                    Debug.Log(
                        "아직 퀵슬롯의 해당 버튼을 비활성화하는 기능을 구현하지 않았어.\n" +
                        "지금은 그냥 메서드를 종료할게.");
                }

                // 메서드 종료
                return;
            }

            // 퀵슬롯에 해당 아이템의 참조 정보가 없다면 메서드 종료
            else return;
        }

        // 아이템 운용으로 인벤토리가 비게 되면 호출되는 메서드
        public void EmptySignal() => ShowItem(IsOpenInventory = !IsOpenInventory);

        // 신개념 인벤토리
        public void ShowItem(bool isOpen)
        {   
            if (isOpen)
            {
                if (inventory.invenDict.Count == 0)
                {
                    Debug.Log("가지고 있는 아이템이 없어.");
                    return;
                }

                player.cursorUtility.CursorSwitch(isOpen);
                MathUtility.ReArrObjects(100, 150, 80, inventory.thinkItems);
            }

            else
                player.cursorUtility.CursorSwitch(isOpen);

            foreach (GameObject obj in inventory.thinkItems)
                obj.SetActive(isOpen);

            quickSlot.SetPanelAlpha(isOpen);
            //quickSlot.ShowLine(isOpen);
        }

        // PlayerInteraction 클래스로부터 획득한 아이템을 인벤토리에 저장하는 메서드
        public void AddItem(ItemKey itemKey)
        {
            // 현재 인벤토리에 해당 아이템이 있으면
            if (inventory.invenDict.ContainsKey(itemKey))
            {
                // 개수를 늘린다
                inventory.invenDict[itemKey].Count(1);
                inventory.CountItem(itemKey, inventory.invenDict[itemKey].itemIndex);
            }

            // 현재 인벤토리에 해당 아이템이 없으면
            else
            {
                // 습득한 아이템의 itemValue를 인스턴스로 만들고
                ItemValue itemValue = new();

                // 만약 퀵슬롯에 등록했던 아이템이라면 갱신
                foreach (KeyValuePair<ItemKey, ItemValue> pair in quickSlot.quickDict.Keys)
                {
                    if (pair.Key == itemKey)
                    {
                        itemValue = pair.Value;
                        itemValue.Count(1);
                        break;
                    }
                }

                // 새 슬롯을 생성해서 아이템 할당
                inventory.MakeSlot(itemKey, itemValue);
            }

            // 퀵슬롯으로 정보 넘기기
            InvenToQuick(ItemData(itemKey));
        }

        // 아이템을 떨어뜨리는 메서드
        public void DropItem(ItemKey selectedItemKey, int invenIndex)
        {
            // 먼저 허상 아이템을 제거한 뒤
            Destroy(itemPhantom);

            // 퀵슬롯으로 옮기거나 퀵슬롯에서 빼는 경우 중단
            if (thisSlot != null || isQuick == true)
            {
                isQuick = false;
                QuickItem(selectedItemKey);
                return;
            }

            // 허상 아이템의 위치에 진짜 아이템을 생성하고
            Instantiate(selectedItemKey.itemPrefab, itemPhantom.transform.position, Quaternion.identity);

            // 허상 아이템 저장 변수를 비운 다음
            itemPhantom = null;

            // 수량 갱신
            CountItem(selectedItemKey, invenIndex);
        }

        // 아이템 수량 관리 메서드
        public void CountItem(ItemKey selectedItemKey, int invenIndex)
        {
            // 아이템의 수량을 감소시킨다
            inventory.invenDict[selectedItemKey].Count(-1);

            // 퀵슬롯으로 정보 넘기기
            InvenToQuick(ItemData(selectedItemKey));

            // 아이템이 아직 사용 가능하다면 수량만 감소시키고
            if (inventory.invenDict[selectedItemKey].IsUsable())
                inventory.CountItem(selectedItemKey, invenIndex);

            // 더 이상 아이템이 없다면
            else
                // 슬롯 제거
                inventory.TakeSlot(selectedItemKey);
        }

        // 아이템 참조 정보를 퀵슬롯에 할당하는 메서드
        private void QuickItem(ItemKey selectedItemKey)
        {
            KeyValuePair<ItemKey, ItemValue> pair = default;

            // 유효성 검사
            var key = CollectionUtility.FirstOrNull(inventory.invenDict.Keys, key => key == selectedItemKey);
            pair = (key == null) ?
                CollectionUtility.FirstOrDefault(quickSlot.quickDict.Keys, kvp => kvp.Key == selectedItemKey) : 
                CollectionUtility.FirstOrDefault(inventory.invenDict, pair => pair.Key == selectedItemKey);

            quickSlot.ReArrQuickSlot(pair);
        }

        #region SelectSlot
        // 어떤 슬롯을 선택했는지 확인하는 메서드
        private void WhichSelect()
        {
            // 아이템 선택 플래그를 true
            isSelected = true;

            StartCoroutine(DetectQuickSlot());

            if (thisSlot != null)
            {
                // 슬롯이 인벤토리에 있는 경우
                if (inventory.thinkItems.Contains(thisSlot.gameObject))
                {
                    selectedSlotIndex = inventory.thinkItems.IndexOf(thisSlot.gameObject);
                    SelectInven((int)selectedSlotIndex);
                }

                // 슬롯이 퀵슬롯에 있는 경우
                else
                    for (int i = 0; i < quickSlot.quickSlots.Length; i++)
                        if (quickSlot.quickSlots[i] == thisSlot)
                        {
                            selectedSlotIndex = i;
                            SelectQuick((int)selectedSlotIndex);
                            break;
                        }

                selectedSlotIndex = null;
            }

            else
            {
                StopCoroutine(DetectQuickSlot());
                return;
            }
        }

        // WhichSelect에서 인벤토리로 확인했을 때 호출되는 메서드
        private void SelectInven(int invenIndex)
        {
            // 선택한 아이템의 키를 읽을 변수
            ItemKey selectedItemKey = null;

            // 선택한 아이템과 일치하는 키를 찾고
            selectedItemKey = CollectionUtility.FirstOrDefault(inventory.invenDict, pair => pair.Value.itemIndex == invenIndex).Key;

            // 허상 아이템 생성
            GenItemPhantom(selectedItemKey, invenIndex);
        }

        // WhichSelect에서 퀵슬롯으로 확인했을 때 호출되는 메서드
        private void SelectQuick(int quickIndex)
        {
            isQuick = true;

            // 선택한 아이템의 키를 읽을 변수
            ItemKey selectedItemKey = null;

            // 선택한 아이템과 일치하는 키를 찾고
            selectedItemKey = CollectionUtility.FirstOrDefault(quickSlot.quickDict, kvp => kvp.Value == quickIndex).Key.Key;

            // 허상 아이템 생성
            GenItemPhantom(selectedItemKey, quickIndex);
        }        

        // 허상 아이템을 생성하는 메서드
        private void GenItemPhantom(ItemKey selectedItemKey, int slotIndex)
        {
            // 해당 키가 존재할 때에만
            if (selectedItemKey == null) return;

            // 허상 아이템을 생성하여 드랍할 위치를 선정하는 반복기 호출
            itemPhantom = Instantiate(selectedItemKey.itemPhantomPrefab, cursorPosition, Quaternion.identity);

            phantomCor = CursorUpdate(selectedItemKey, slotIndex);
            StartCoroutine(phantomCor);
        }
        #endregion
        #endregion

        // 기타 유틸리티
        #region Utilities
        // 허상 아이템을 잡아두는 반복기
        public IEnumerator CursorUpdate(ItemKey selectedItemKey, int slotIndex)
        {
            while (isSelected)
            {
                // cursorUtility의 이벤트 핸들러를 통해 Vector2 CursorPosition을 Vector3 변수에 입력
                Vector3 mousePosition = player.cursorUtility.CursorPosition;

                // 적절한 깊이값 설정
                mousePosition.z = 1f;

                // 마우스 포인터의 위치를 월드 좌표로 변환
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                // 허상 아이템의 transform.position을 해당 좌표와 동기화
                itemPhantom.transform.position = worldPosition;

                // 본 반복기를 매 프레임 실행
                yield return null;
            }

            // 플레이어가 적절한 위치를 클릭하면 해당 아이템을 실체화
            DropItem(selectedItemKey, slotIndex);

            // 이 반복기를 기억하는 변수를 비우고
            phantomCor = null;

            // 작동 중인 코루틴 정지
            yield break;
        }

        // 슬롯을 감지하는 반복기
        public IEnumerator DetectQuickSlot()
        {
            // 다시 클릭할 때까지 계속 반복
            while (IsSelected)
            {
                // EventSystem으로부터 포인터 정보를 받고
                PointerEventData pointerData = new(eventSystem)
                {
                    position = Mouse.current.position.ReadValue()
                };

                // 그를 기반으로 GraphicRaycaster 시행
                List<RaycastResult> results = new();
                raycaster.Raycast(pointerData, results);

                // 선택할 퀵슬롯을 비워두고
                thisSlot = null;   // InventoryManager의 참조는 속성으로

                // 현재의 슬롯을 감지
                foreach (RaycastResult result in results)
                {
                    // 감지한 UGUI가 텍스트박스라면 무시
                    if (result.gameObject.GetComponent<TextMeshProUGUI>())
                        continue;

                    // Button UI 획득을 시도해보고 잡히면 선택
                    if (result.gameObject.TryGetComponent<Button>(out var slot))
                        thisSlot = slot;
                }

                // 이 반복기를 매 프레임 반복하고
                yield return null;
            }

            // 인벤토리에서 제자리에 돌려놓는 건 안 돼
            foreach (GameObject obj in inventory.thinkItems)
            {
                if (thisSlot == null) break;

                else if (obj == thisSlot.gameObject)
                {
                    thisSlot = null;
                    break;
                }
            }

            // 역할이 끝나면 종료
            yield break;
        }
        #endregion
    }
}