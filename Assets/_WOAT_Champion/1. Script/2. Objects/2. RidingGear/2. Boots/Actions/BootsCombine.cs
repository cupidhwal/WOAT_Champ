using UnityEngine;

namespace WOAT
{
    // 카트리지 장착과 사용을 관리하는 클래스
    public class BootsCombine
    {
        // 필드
        #region Variables
        // 카트리지
        private Cartridge cartridge;
        private GameObject currentCartridge;

        // 클래스 컴포넌트
        private RidingBoots boots;
        #endregion

        // 속성
        #region Properties
        public bool IsSet { get; set; }
        public Cartridge Cartridge => cartridge;
        #endregion

        // 생성자
        #region Constructor
        public BootsCombine(RidingBoots boots) { this.boots = boots; }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        #endregion

        // 메서드
        #region Methods
        // 실제로 이벤트 핸들러에 작성하는 메서드
        public bool CheckCartridge(float propellant)
        {
            if (!IsSet)
            {
                Debug.Log("카트리지를 장착하지 않으면 라이딩을 할 수 없어.");
                return false;
            }

            if (cartridge.CurrentPropellant < propellant)
            {
                Debug.Log("카트리지를 다 썼어. 교체해야 돼.");
                return false;
            }

            return true;
        }

        // 카트리지를 교체하는 메서드
        public void ChangeCartridge(GameObject newCartridge)
        {
            // 이미 착용 중인 건 파괴하고 초기화
            if (currentCartridge != null)
            {
                Factory.Instance.Destroy(currentCartridge);
                currentCartridge = null;
                this.cartridge = null;
                IsSet = false;
            }

            currentCartridge = Factory.Instance.Instantiate(newCartridge, boots.transform);
            this.cartridge = currentCartridge.transform.GetComponent<Cartridge>();
            ReadCartridge(cartridge);
            IsSet = true;
        }

        public void ReadCartridge(Cartridge cartridge)
        {
            this.cartridge = cartridge;
        }
        #endregion
    }
}