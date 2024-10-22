using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 보드 타입 라이딩기어의 이동 제어를 담당하는 클래스
    public class BoardDrive
    {
        // 필드
        #region Variables
        // 단순 변수
        private readonly int maxSpeed;
        private readonly int reverseSpeed;
        private readonly int turnSpeed;
        private readonly int tiltSpeed;
        private readonly int initialForce;
        private readonly int moveForce;
        private readonly int downForce;
        private readonly float brakeCoefficient;

        // 복합 변수
        private Vector2 moveInput;              // 이동 입력
        private Vector3 moveDirection;          // 이동 방향
        private Vector3 currentVelocity;        // 현재 속도

        // 컴포넌트
        private readonly Rigidbody rbGear;

        // 클래스 컴포넌트
        public RidingBoard board;
        #endregion

        // 속성
        #region Properties
        public Vector2 MoveInput => moveInput;
        #endregion

        // 생성자
        #region Constructor
        public BoardDrive(RidingBoard board, int maxSpeed, int reverseSpeed, int turnSpeed, int tiltSpeed, int initialForce, int moveForce, int downForce, float brakeCoefficient)
        {
            // 컴포넌트 초기화
            this.board = board;
            this.rbGear = board.transform.GetComponent<Rigidbody>();

            // 성능 변수 초기화
            this.maxSpeed = maxSpeed;
            this.turnSpeed = turnSpeed;
            this.tiltSpeed = tiltSpeed;
            this.reverseSpeed = reverseSpeed;
            this.initialForce = initialForce;
            this.moveForce = moveForce;
            this.downForce = downForce;
            this.brakeCoefficient = brakeCoefficient;
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnDrivePerformed(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

        public void OnDriveCanceled(InputAction.CallbackContext _) => moveInput = Vector2.zero;
        #endregion

        // 메서드
        #region Methods
        public void Drive()
        {
            currentVelocity = rbGear.linearVelocity;

            if (moveInput.y >= 0.1)
            {
                if (currentVelocity.magnitude <= 0.1)
                    rbGear.AddForce((board.transform.forward * moveInput.y).normalized * initialForce, ForceMode.Impulse);

                if (currentVelocity.magnitude > 0.1)
                    rbGear.AddForce((board.transform.forward * moveInput.y).normalized * moveForce, ForceMode.Force);

                if (currentVelocity.magnitude >= maxSpeed)
                    rbGear.linearVelocity = rbGear.linearVelocity.normalized * maxSpeed;

                if (moveDirection == Vector3.zero)
                    rbGear.linearVelocity *= brakeCoefficient;
            }
            else
            {
                if (currentVelocity.magnitude <= 0.1)
                    rbGear.AddForce((board.transform.forward * moveInput.y).normalized * initialForce, ForceMode.Impulse);

                if (currentVelocity.magnitude > 0.1)
                    rbGear.AddForce((board.transform.forward * moveInput.y).normalized * moveForce, ForceMode.Force);

                if (currentVelocity.magnitude >= reverseSpeed)
                    rbGear.linearVelocity = rbGear.linearVelocity.normalized * reverseSpeed;

                if (moveDirection == Vector3.zero)
                    rbGear.linearVelocity *= brakeCoefficient;
            }
        }

        public void Turn()
        {
            if (MoveDirection() != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MoveDirection());
                Quaternion smoothBoardRotation = Quaternion.Slerp(rbGear.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
                rbGear.MoveRotation(smoothBoardRotation);

                // 턴의 미끄러짐을 방지하기 위해 다운포스를 작성
                rbGear.AddForce(Vector3.down * downForce, ForceMode.Force);
            }
        }

        public void Tilt()
        {
            float tiltAngle = 0f;

            if (moveInput.x != 0)
                tiltAngle = Mathf.Lerp(0, 30f, Mathf.Abs(moveInput.x)) * -Mathf.Sign(moveInput.x);

            Quaternion currentRotation = rbGear.rotation;
            Quaternion targetTiltRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, tiltAngle);
            Quaternion smoothTiltRotation = Quaternion.Slerp(currentRotation, targetTiltRotation, tiltSpeed * Time.fixedDeltaTime);
            rbGear.MoveRotation(smoothTiltRotation);
        }

        public Vector3 MoveDirection()
        {
            Vector3 forward = board.transform.forward;
            Vector3 right = board.transform.right;

            forward.y = 0;
            right.y = 0;

            return moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        }
        #endregion
    }
}