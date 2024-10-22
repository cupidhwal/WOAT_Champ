using Unity.VisualScripting;
using UnityEngine;

namespace WOAT
{
    // RidingGear 클래스를 상속받은 부츠 타입 라이딩기어의 중심 클래스
    // 탑승, 하차 로직과 대리자를 오버라이드하고, 부츠 타입에 필수적인 기능 클래스의 매개변수와 구동 방식 로직을 갖는다.
    public class RidingBoots : RidingGear
    {
        // 필드
        #region Variables
        // 단순 변수
        protected int dashSpeed;
        protected int jumpForce;

        // 클래스 컴포넌트
        public BootsCombine bootsCombine;
        public BootsMove bootsMove;
        public BootsGS bootsGS;
        public BootsUI bootsUI;
        #endregion

        // 속성
        #region Properties
        public bool OnEnhance { get; private set; }
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void FixedUpdate()
        {
            if (OnEnhance) bootsGS.EnhanceMode();
        }

        private void OnEnable()
        {
            // Control에 하차 입력 구독
            control.RidingGear.RideOff.performed += OnRideOffPerformed;

            // 부츠 타입 라이딩기어의 점프 이벤트 연결
            control.RidingGear.Jump.started += bootsMove.OnJumpStarted;

            // 부츠 타입 라이딩기어의 대시 이벤트 연결
            control.RidingGear.Dash.started += bootsMove.OnDashStarted;

            // 대리자 이벤트에 메서드 구독
            OnStanceChanged += StanceCheck;

            // 라이딩기어 컨트롤 활성화
            control.RidingGear.Enable();
        }

        private void OnDisable()
        {
            // 라이딩기어 컨트롤 비활성화
            control.RidingGear.Disable();

            // 대리자 이벤트에 메서드 구독 해제
            OnStanceChanged -= StanceCheck;

            // 부츠 타입 라이딩기어의 대시 이벤트 연결 해제
            control.RidingGear.Dash.started -= bootsMove.OnDashStarted;

            // 부츠 타입 라이딩기어의 점프 이벤트 연결 해제
            control.RidingGear.Jump.started -= bootsMove.OnJumpStarted;

            // Control에 하차 입력 구독 해제
            control.RidingGear.RideOff.performed -= OnRideOffPerformed;
        }
        #endregion

        // 메서드
        #region Methods
        // 리지드바디 세팅 메서드
        private void SetRigidbody()
        {
            if (joint == null)
            {
                rbGear = this.AddComponent<Rigidbody>();
                rbGear.mass = 2f;
                rbGear.linearDamping = 0;
                rbGear.angularDamping = 0;
                rbGear.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        #endregion

        // 오버라이드
        #region Override
        // 탑승 시 Player 클래스 참조를 기억하고 각 기능 클래스에 기억을 뿌리는 메서드 오버라이드
        public override void SetPlayer(Player player)
        {
            base.SetPlayer(player);
            bootsMove.SetPlayer(player);
            bootsGS.SetPlayer(player);
        }

        // 탑승 시의 타입 스탠스 오버라이드
        protected override void StanceCheck()
        {
            if (joint != null)
            {
                // 타입 판정 플래그
                isBoard = false;
            }
            else
            {
                // 하차
                isBoard = null;
            }

            base.StanceCheck();
        }

        // 탑승 로직 오버라이드
        public override void RideOn()
        {
            // 플레이어와 라이딩기어의 충돌 무시
            Physics.IgnoreCollision(playerCollider, gearCollider, true);
            Physics.IgnoreCollision(playerCollider, partsCollider, true);

            // 라이딩기어의 로컬 좌표 (0, 0, 0)를 월드 좌표로 변환
            Vector3 targetPosition = this.transform.TransformPoint(new Vector3(0, 0, 0));

            // 플레이어의 로컬 좌표 (0, -0.55f, 0)를 월드 좌표로 변환하고 그 차이를 계산
            Vector3 offset = player.transform.TransformPoint(new Vector3(0, -0.55f, 0)) - player.transform.position;

            // 플레이어의 월드 좌표를 보드의 해당 위치로 이동
            player.transform.position = targetPosition - offset;

            base.RideOn();

            // 라이딩기어의 리지드바디를 제거
            rbPlayer.mass += rbGear.mass;
            Destroy(rbGear);
            rbGear = null;

            // 부츠 타입 라이딩기어는 플레이어의 자식 오브젝트로 들어가게 되므로 플레이어가 아니라 부츠를 동기화한다.
            this.gameObject.transform.localRotation = player.transform.rotation;

            joint.autoConfigureConnectedAnchor = false;         // 조인트 오토 해제
            joint.anchor = new Vector3(0, -0.55f, 0);           // 조인트 주체 위치
            joint.connectedAnchor = new Vector3(0, 0, 0);       // 조인트 표적 위치

            joint.breakForce = 100000f;                         // 조인트 파괴 강도를 무한대로 설정
            joint.breakTorque = 100000f;                        // 조인트 파괴 회전력을 무한대로 설정

            this.transform.SetParent(player.transform);
        }

        // 하차 로직 오버라이드
        protected override void RideOff()
        {
            this.transform.SetParent(null);

            base.RideOff();

            // 리지드바디 재생성
            SetRigidbody();
            rbPlayer.mass -= rbGear.mass;

            rbGear.constraints = RigidbodyConstraints.None;

            // 플레이어와 라이딩기어의 충돌 활성화
            Physics.IgnoreCollision(playerCollider, gearCollider, false);
            Physics.IgnoreCollision(playerCollider, partsCollider, false);
        }

        // 하차 방향
        protected override Vector3 OffDirection()
        {
            float dir = (player.playerMove.MoveInput == Vector2.zero) ? 1f : player.playerMove.MoveInput.x;

            Vector3 direction = (Vector3.up + new Vector3(dir, 0, 0).normalized).normalized;
            return direction;
        }
        #endregion

        // 이벤트 메서드
        #region Event Methods
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // 등반 시작
                player.playerStates.isClimbing = true;

                // 실제 중력 제거
                rbPlayer.useGravity = false;

                // 속도 초기화
                rbPlayer.linearVelocity = Vector3.zero;

                // 인핸스 모드 시작
                OnEnhance = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
                // 인핸스 모드 가동 중이라면 LastMoveDirection 갱신
                player.playerMove.LastMoveDirection = player.playerMove.MoveInput;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // 인핸스 모드 끝
                OnEnhance = false;

                // 실제 중력 생성
                rbPlayer.useGravity = true;

                // 등반 끝
                player.playerStates.isClimbing = false;
            }
        }
        #endregion
    }
}