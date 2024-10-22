using UnityEngine;

namespace WOAT
{
    // RidingGear 클래스를 상속받은 보드 타입 라이딩기어의 중심 클래스
    // RidingGear 클래스의 탑승, 하차 로직과 대리자를 오버라이드 하고
    // 보드 타입에 필수적인 기능 클래스의 매개변수와 구동 방식 로직을 갖는다
    public class RidingBoard : RidingGear
    {
        // 필드
        #region Variables
        private bool isGrounded = true;      // 지면 판정
        private bool isRight = false;        // 탑승 직전 위치 판정 ? 플레이어가 보드의 오른쪽 : 왼쪽

        protected int maxSpeed;
        protected int reverseSpeed;
        protected int turnSpeed;
        protected int tiltSpeed;
        protected int initialForce;
        protected int moveForce;
        protected int downForce;
        protected float brakeCoefficient;

        protected BoardDrive boardDrive;
        #endregion

        // 속성
        #region Properties
        public bool IsRight => isRight;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void OnEnable()
        {
            // Control에 하차 입력 구독
            control.RidingGear.RideOff.performed += OnRideOffPerformed;

            // 보드 타입 라이딩기어의 이벤트 연결
            control.RidingGear.Drive.performed += boardDrive.OnDrivePerformed;
            control.RidingGear.Drive.canceled += boardDrive.OnDriveCanceled;

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

            // 보드 타입 라이딩기어의 운전 이벤트 연결 해제
            control.RidingGear.Drive.performed -= boardDrive.OnDrivePerformed;
            control.RidingGear.Drive.canceled -= boardDrive.OnDriveCanceled;

            // Control에 하차 입력 구독 해제
            control.RidingGear.RideOff.performed -= OnRideOffPerformed;
        }

        private void FixedUpdate()
        {
            if (isBoard != true) return;

            if (isGrounded)
            {
                if (boardDrive.MoveInput.y > 0)
                {
                    boardDrive.Drive();
                    boardDrive.Turn();
                    boardDrive.Tilt();
                }
                else
                {
                    boardDrive.Drive();
                    boardDrive.Tilt();
                }
            }
        }
        #endregion

        // 메서드
        #region Methods
        private bool WhereIsPlayer()
        {
            Vector3 playerLocalPos = this.transform.InverseTransformPoint(player.transform.position);

            if (playerLocalPos.x > 0)
                return true;    // 플레이어가 보드의 오른쪽에 있음

            else
                return false;   // 플레이어가 보드의 왼쪽에 있음
        }
        #endregion

        // 오버라이드
        #region Override
        protected override void StanceCheck()
        {
            if (joint != null)
            {
                // 타입 판정 플래그
                isBoard = true;
            }
            else
            {
                // 하차
                isBoard = null;
            }

            base.StanceCheck();
        }

        public override void RideOn()
        {
            // 탑승 직전, 플레이어가 보드의 ? 오른쪽 : 왼쪽
            isRight = WhereIsPlayer();

            // 라이딩기어의 로컬 좌표 (0, 1, 0)를 월드 좌표로 변환
            Vector3 targetPosition = this.transform.TransformPoint(new Vector3(0, 1, 0));

            // 플레이어의 로컬 좌표 (0, -2, 0)를 월드 좌표로 변환하고 그 차이를 계산
            Vector3 offset = player.transform.TransformPoint(new Vector3(0, -2, 0)) - player.transform.position;

            // 플레이어의 월드 좌표를 보드의 해당 위치로 이동
            player.transform.position = targetPosition - offset;

            base.RideOn();

            // X축 이동을 제한 해제 (이동 범위를 설정)
            joint.xMotion = ConfigurableJointMotion.Limited;

            // 리니어 리미트를 설정
            SoftJointLimit limit = new() { limit = 0.5f };  // 리미트 거리 
            joint.linearLimit = limit;

            // 제한 범위를 설정 (양축에서 움직일 수 있는 범위 설정)
            JointDrive xDrive = new()
            {
                positionSpring = 10000f,
                positionDamper = 100f,
                maximumForce = Mathf.Infinity
            };

            joint.xDrive = xDrive;

            joint.autoConfigureConnectedAnchor = false;     // 조인트 오토 해제
            joint.anchor = new Vector3(0, -2, 0);           // 조인트 주체 위치
            joint.connectedAnchor = new Vector3(0, 1, 0);   // 조인트 표적 위치

            joint.breakForce = 40000f;                      // 조인트 파괴 강도를 무한대로 설정
            joint.breakTorque = 10000f;                     // 조인트 파괴 회전력을 무한대로 설정
        }

        protected override void RideOff()
        {
            base.RideOff();

            rbGear.constraints = RigidbodyConstraints.None;
        }

        // 하차 방향 오버라이드
        protected override Vector3 OffDirection()
        {
            float playerPos = isRight ? 1f : -1f;

            float dir = (boardDrive.MoveInput == Vector2.zero) ? playerPos : boardDrive.MoveInput.x;

            Vector3 direction = (Vector3.up + new Vector3(dir, 0, 0).normalized).normalized;
            return direction;
        }
        #endregion

        // 이벤트 메서드
        #region Event Methods
        //Collision 시리즈
        protected virtual void OnCollisionChange(Collision collision, bool groundedState)
        {
            if (collision.gameObject.CompareTag("Ground"))
                isGrounded = groundedState; // 지면에 닿았는지 여부를 설정
        }

        private void OnCollisionEnter(Collision collision) => OnCollisionChange(collision, true);

        private void OnCollisionStay(Collision collision) => OnCollisionChange(collision, true);

        private void OnCollisionExit(Collision collision) => OnCollisionChange(collision, false);
        #endregion
    }
}