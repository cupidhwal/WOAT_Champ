using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace WOAT
{
    // 플레이어의 상태를 관리하는 클래스
    // 플레이어의 각종 불리언 변수와 스탠스를 관리한다
    public class PlayerStates
    {
        // 필드
        #region Variables
        // 불리언 변수
        public bool isGrounded = true;      // 지면 판정
        public bool isKeepGoing;            // KeepGoing 기능 활성화 플래그
        public bool isClimbing;             // 등반 판정
        public bool? isBoard;               // 라이딩기어 탑승 및 타입 판정 nullable 플래그, true = 보드, false = 부츠, null = 기본

        // 클래스 컴포넌트
        private readonly Player player;
        #endregion

        // 생성자
        #region Constructor
        public PlayerStates(Player player)
        {
            this.player = player;
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnRestartStarted(InputAction.CallbackContext _) => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        #endregion

        // 메서드
        #region Methods
        public void SetThisGear(bool? isBoard)
        {
            this.isBoard = isBoard;
        }

        public void SwitchTrigger(bool flag)
        {
            // 트리거 콜라이더를 제어하기 위한 변수
            SphereCollider Trigger = player.transform.Find("RidingSense").GetComponent<SphereCollider>();

            if (flag == false)
            {
                DisableTrigger(Trigger);
            }
            else
            {
                // 트리거를 활성화 할 땐 OnTriggerEnter 메서드가 자동으로 호출되므로 트리거를 켜기만 하면 된다
                Trigger.enabled = true;
            }
        }

        // 트리거가 비활성화 될 때 필요한 로직
        private void DisableTrigger(SphereCollider collider)
        {
            // 트리거 비활성화
            collider.enabled = false;

            // 리스트의 모든 라이딩기어 Player == null
            foreach (RidingGear gear in player.playerRide.ridableGears)
            {
                // 플레이어가 기본 상태라면
                if (isBoard == null)
                    gear.SetPlayer(null);

                // 라이딩기어에 탑승한 상태라면 해당 라이딩기어만 상호작용 유지
                else if (gear == player.ridingGear)
                    gear.SetPlayer(player);

                // 나머지는 모두 null
                else
                    gear.SetPlayer(null);
            }

            // 리스트 초기화
            player.playerRide.ridableGears.Clear();

            if (player.playerRide.ridableGears.Count == 0)
            {
                // 리스트를 확인하고 탑승 기능 비활성화
                player.playerRide.SwitchRideOn();

                // 주목하던 라이딩기어 null, 탑승했다면 유지
                if (isBoard != null) return;
                player.ridingGear = null;
            }
        }
        #endregion
    }
}