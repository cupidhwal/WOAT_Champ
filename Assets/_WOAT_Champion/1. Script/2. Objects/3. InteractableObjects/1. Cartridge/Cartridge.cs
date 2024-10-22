using UnityEngine;

namespace WOAT
{
    // 부츠 타입 라이딩기어 전용 소비재, 카트리지의 기능을 담당하는 클래스
    public class Cartridge : MonoBehaviour, IInteractable, IUsable
    {
        // 기본 정보
        private readonly int itemID = 0;
        private ItemKey itemData;
        public ItemDatabase itemDatabase;

        // 필드
        #region Variables
        // 단순 변수
        private readonly float maxPropellant = 1000f;
        [SerializeField] private float currentPropellant;

        // 불리언 변수
        private bool isUsed = false;        // 카트리지 사용 여부를 확인하는 불리언

        // 클래스 컴포넌트
        private Player player;
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
            itemData = itemDatabase.itemList[itemID];

            // 초기화
            currentPropellant = maxPropellant;
        }
        #endregion

        // 메서드
        #region Methods
        public void UseCartridge(float propellant)
        {
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

        public bool CanUse(Player player)
        {
            this.player = player;

            if (player.playerStates.isBoard == false) return true;
            else
            {
                Debug.Log("카트리지는 부츠 타입 라이딩기어를 착용한 상태에서만 쓸 수 있어.");
                return false;
            }
        }

        public void UseItem()
        {
            RidingBoots boots = (RidingBoots)player.ridingGear;
            GameObject newCartridge = itemDatabase.itemList[itemID].itemPrefabAddOn;
            boots.bootsCombine.ChangeCartridge(newCartridge);

            Debug.Log($"{player.ridingGear.name}에 카트리지를 장착했어.");

            if (!isUsed) isUsed = true;
        }
        #endregion
    }
}