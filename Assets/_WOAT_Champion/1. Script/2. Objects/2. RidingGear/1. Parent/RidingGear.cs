using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 라이딩기어의 최상위 부모 클래스
    // 기본적으로 상속 방식이며, 구성 방식에 따라 캡슐화할 수 있는 것은 하되
    // 반드시 오버라이드가 필요한 메서드는 여기에 작성하도록 한다
    // 이때 오버라이드가 필요한 메서드는 탑승, 하차 로직과 대리자
    public class RidingGear : MonoBehaviour
    {
        // 필드
        #region Variables
        protected bool? isBoard = null;         // 타입 판정

        protected Control control;             // 컨트롤 획득

        protected Rigidbody rbGear;             // 라이딩기어 Rigidbody
        protected Rigidbody rbPlayer;           // 플레이어 Rigidbody

        protected Collider playerCollider;      // 플레이어 오브젝트의 콜라이더
        protected Collider gearCollider;        // 라이딩기어 오브젝트의 콜라이더
        protected Collider partsCollider;       // 라이딩기어 부품 오브젝트의 콜라이더
        protected ConfigurableJoint joint;      // 플레이어에 부여할 조인트

        protected Player player;                // 플레이어 오브젝트
        #endregion

        // 라이프 사이클
        #region Life Cycle
        // Start is called before the first frame update
        protected virtual void Start()
        {
            // 탑승 전까지 비활성화
            control.RidingGear.Disable();

            // 컴포넌트 초기화
            rbGear = GetComponent<Rigidbody>();
            gearCollider = this.transform.GetComponent<Collider>();
            partsCollider = this.transform.GetComponentInChildren<Collider>();
        }

        protected virtual void Awake()
        {
            control = new Control(); // 컨트롤 시스템 초기화
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnRideOffPerformed(InputAction.CallbackContext _) => RideOff(); // 하차 처리 메서드 호출
        #endregion

        // 대리자
        #region Delegate
        //대리자 선언
        protected delegate void StanceSwitch(); // 스탠스 변경 대리자 선언

        //이벤트 선언
        protected event StanceSwitch OnStanceChanged; // 스탠스 변경 이벤트 선언

        //메서드 : 이벤트가 발생할 때 실행할 기능
        protected virtual void StanceCheck()
        {
            if (joint != null)
            {
                // 라이딩기어의 컨트롤 Enable
                control.RidingGear.Enable();

                // 라이딩기어의 변수 전달
                player.SetThisGear(this, this.transform, isBoard);
            }
            else
            {
                // 플레이어와 라이딩기어의 연결 해제
                player.SetThisGear(null, null, isBoard);

                // 라이딩기어의 컨트롤 Disable
                control.RidingGear.Disable();
            }
        }
        #endregion

        // 메서드
        #region Methods
        public virtual void SetPlayer(Player player)
        {
            // 우회용 변수
            // this.player = player; 코드를 바로 작성하면 player == null 일 경우
            // 구독한 메서드의 유연한 해제가 불가능하기 때문에 이 변수를 사용한다.
            Player nullPlayer = null;

            // 플레이어를 참조하여 각 컴포넌트 초기화
            nullPlayer = player;

            if (nullPlayer != null)
            {
                this.player = player;

                rbPlayer = this.player.GetComponent<Rigidbody>();
                playerCollider = this.player.GetComponent<Collider>();

                // 플레이어의 컨트롤 체크 메서드 구독
                OnStanceChanged += this.player.ControlCheck;
            }
            else
            {
                // 플레이어의 컨트롤 체크 메서드 구독 해제
                OnStanceChanged -= this.player.ControlCheck;

                playerCollider = null;
                rbPlayer = null;

                this.player = player;
            }
        }

        public virtual void RideOn()
        {
            // 쓰러진 라이딩기어를 바르게 놓기
            float yRotation = this.gameObject.transform.localRotation.eulerAngles.y;
            this.gameObject.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

            // 원활한 운전을 위해 리지드바디 회전 고정
            rbGear.constraints = RigidbodyConstraints.FreezeRotation;

            if (isBoard == null && joint == null)
            {
                // 고정 조인트를 플레이어와 연결
                joint = player.gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = rbGear;

                // XYZ축 이동을 고정
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.xMotion = ConfigurableJointMotion.Locked;

                // 회전도 고정
                joint.angularXMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Locked;
            }

            // joint 상태가 변했으므로 이벤트 호출
            OnStanceChanged?.Invoke();
        }

        protected virtual void RideOff()
        {
            if (isBoard != null && joint != null)
            {
                Destroy(joint);             // 고정 조인트 제거
                joint = null;               // 조인트 상태를 null로 초기화

                // 라이딩기어 옆으로 내리기
                rbPlayer.AddForce(OffDirection() * 500, ForceMode.Impulse);
            }

            // joint 상태가 변했으므로 이벤트 호출
            OnStanceChanged?.Invoke();
        }

        public void CrashAccident()
        {
            // 조인트 파괴 (찌꺼기 제거)
            if (joint != null)
                joint = null;

            // joint 상태가 변했으므로 이벤트 호출
            OnStanceChanged?.Invoke();

            // 플레이어가 사고로 하차했으므로 라이딩기어는 이동 및 회전 제약 해제
            rbGear.constraints = RigidbodyConstraints.None;
            // 보드와 부츠는 각각 오버라이드 메서드에 작성
        }
        #endregion

        // 기타 유틸리티
        #region Utilities
        // 하차 방향 정하기
        protected virtual Vector3 OffDirection()
        {
            // RidingBoard와 RidingBoots에서 반드시 Override 할 것
            return Vector3.up; // 기본 하차 방향은 위쪽
        }
        #endregion
    }
}