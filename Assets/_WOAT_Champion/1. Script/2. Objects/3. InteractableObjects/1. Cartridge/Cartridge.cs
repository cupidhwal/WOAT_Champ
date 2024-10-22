using UnityEngine;

namespace WOAT
{
    // 부츠 타입 라이딩기어 전용 소비재, 카트리지의 기능을 담당하는 클래스
    public class Cartridge : MonoBehaviour, IInteractable, IUsable
    {
        // 기본 정보
        private ItemKey itemData;
        public ItemDatabase itemDatabase;

        // 필드
        #region Variables
        // 단순 변수
        private readonly float maxPropellant = 1000f;
        [SerializeField] private float currentPropellant;

        // 불리언 변수
        private bool isUsed = false;        // 카트리지 사용 여부를 확인하는 불리언
        #endregion

        // 속성
        #region Properties
        public float CurrentPropellant => currentPropellant;
        public bool IsUsed => isUsed;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            // 최초에 아이템 데이터베이스로부터 자신을 정의
            itemData = itemDatabase.itemList[0];

            // 초기화
            currentPropellant = maxPropellant;
        }
        #endregion

        // 메서드
        #region Methods
        public void UseCartridge(float propellant)
        {
            if (!isUsed) isUsed = true;

            if (currentPropellant < propellant)
            {
                Debug.Log("추진제가 다 떨어졌어. 새 카트리지로 교체해야 해.");
            }

            else
                currentPropellant -= propellant;
        }

        public float ShowCartridge()
        {
            Debug.Log($"추진제가 {(currentPropellant / maxPropellant) * 100}% 남았어.");
            return currentPropellant;
        }
        #endregion

        // 인터페이스 메서드
        #region Interface Methods
        public ItemKey GetItemData()
        {
            return itemData;
        }

        public void UseItem(Player player)
        {
            return;
        }
        #endregion
    }
}

/*


        // 사용할 아이템이 카트리지라면
        private bool IfCartridge(IUsable usableItem)
        {
            if (player.playerStates.isBoard == false)
            {
                RidingBoots boots = (RidingBoots)player.ridingGear;
                GameObject newCartridge = TakeOutItem(0);
                boots.ChangeCartridge(newCartridge);

                Debug.Log($"{player.ridingGear.name}에 카트리지를 장착했어.");
                return true;
            }

            else
            {
                Debug.Log("카트리지는 부츠 타입 라이딩기어에 탑승 중일 때에만 사용할 수 있어.");
                return false;
            }
        }

        private GameObject TakeOutItem(int itemID)
        {
            ItemKey thisItem = CollectionUtility.FirstOrDefault<ItemKey>(itemDatabase.itemList, key => key.itemID == itemDatabase.itemList[itemID].itemID);

            if (thisItem.itemPrefabAddOn != null) return thisItem.itemPrefabAddOn;
            else return thisItem.itemPrefab;
        }
 */