using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 플레이어의 이동 제어를 관리하는 클래스
    public class PlayerMove
    {
        // 필드
        #region Variables
        // 단순 변수
        private readonly float moveSpeed;
        private readonly float jumpForce;

        // 복합 변수
        private Vector2 moveInput;          // 이동 입력
        private Vector2 lastMoveDirection;  // 마지막 이동 방향

        // 컴포넌트
        private readonly Rigidbody rb;

        // 클래스 컴포넌트
        private readonly Player player;
        #endregion

        // 속성
        #region Properties
        public Vector2 MoveInput { get { return moveInput; } }
        public Vector2 LastMoveDirection { get { return lastMoveDirection; } set { lastMoveDirection = value; } }
        #endregion

        // 생성자
        #region Constructor
        public PlayerMove(Player player, float moveSpeed, float jumpForce)
        {
            this.player = player;
            this.rb = player.transform.GetComponent<Rigidbody>();
            this.moveSpeed = moveSpeed;
            this.jumpForce = jumpForce;
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnMovePerformed(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>().normalized;

        public void OnMoveCanceled(InputAction.CallbackContext _) => moveInput = Vector2.zero;
        public void OnJumpStarted(InputAction.CallbackContext _)
        {
            if (player.playerStates.isGrounded)
                Jump();
        }
        #endregion

        // 메서드
        #region Methods
        // 플레이어의 기본 이동 메서드
        public void Move()
        {
            Vector3 moveDirection = new Vector3(lastMoveDirection.x, 0, lastMoveDirection.y);

            Vector3 forward = player.transform.forward * moveDirection.z;
            Vector3 right = player.transform.right * moveDirection.x;

            Vector3 move = moveSpeed * Time.fixedDeltaTime * (forward + right).normalized;

            rb.MovePosition(rb.position + move);
        }

        // 플레이어의 기본 점프 메서드
        public void Jump()
        {
            rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
        }
        #endregion
    }
}