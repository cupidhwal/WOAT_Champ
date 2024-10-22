using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 플레이어의 탑승 제어를 관리하는 클래스
    public class PlayerRide
    {
        // 필드
        #region Variables
        // 복합 변수
        public List<RidingGear> ridableGears;

        // 클래스 컴포넌트
        private readonly Player player;
        #endregion

        // 생성자
        #region Constructor
        public PlayerRide(Player player)
        {
            this.player = player;
            ridableGears = new List<RidingGear>();
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnRideOnStarted(InputAction.CallbackContext _) => player.ridingGear.RideOn();
        #endregion

        // 메서드
        #region Methods
        // 이벤트 핸들러 스위치
        public void SwitchRideOn()
        {
            if (ridableGears.Count > 0)
                player.control.Player.RideOn.Enable();

            else
                player.control.Player.RideOn.Disable();
        }

        // 관심 있는 라이딩기어
        public void RideInteraction(Collider other)
        {
            // 임시 변수
            RidingGear lookGear = other.gameObject.transform.GetComponent<RidingGear>();

            // 해당 라이딩기어에 플레이어를 인식
            lookGear.SetPlayer(player);

            // 해당 라이딩기어를 리스트에 등록
            ridableGears.Add(lookGear);

            // 리스트를 확인하고 탑승 기능 활성화
            SwitchRideOn();

            // 탑승할 라이딩기어는 첫 번째에 확인한 것
            player.ridingGear = ridableGears[0];
        }

        // 안중에도 없는 라이딩기어
        public void NullInteraction(Collider other)
        {
            // 임시 변수
            RidingGear nullGear = other.gameObject.transform.GetComponent<RidingGear>();

            // 관심 없는 라이딩기어는 리스트에서 삭제
            ridableGears.Remove(nullGear);

            // 해당 라이딩기어는 플레이어 인식 해제
            nullGear.SetPlayer(null);

            // 탑승할 라이딩기어로 그 다음 것
            if (ridableGears.Count > 0)
                player.ridingGear = ridableGears[0];

            else
            {
                // 리스트를 확인하고 탑승 기능 비활성화
                SwitchRideOn();

                if (player.playerStates.isBoard != null) return;
                player.ridingGear = null;
            }
        }
        #endregion
    }
}