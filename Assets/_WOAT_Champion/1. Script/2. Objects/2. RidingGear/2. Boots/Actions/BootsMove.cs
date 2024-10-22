using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    public class BootsMove
    {
        // 필드
        #region Variables
        // 단순 변수
        private readonly int dashSpeed;
        private readonly int jumpForce;

        // 점프 기능 전용 필드
        private float jumpCool = 0.5f;
        private float proForJump = 40f;
        private bool isJumped = false;

        // 대시 기능 전용 필드
        private float dashCool = 1f;
        private float proForDash = 25f;
        private bool isDashed = false;

        // 컴포넌트
        private Rigidbody rbPlayer;

        // 클래스 컴포넌트
        private Player player;
        private RidingBoots boots;
        #endregion

        // 생성자
        #region Constructor
        public BootsMove(RidingBoots ridingBoots, int dashSpeed, int jumpForce)
        {
            this.boots = ridingBoots;
            this.dashSpeed = dashSpeed;
            this.jumpForce = jumpForce;
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnJumpStarted(InputAction.CallbackContext _)
        {
            if (!isJumped && (boots.bootsCombine.CheckCartridge(proForJump)))
            {
                Jump(player.playerStates.isGrounded, jumpCool);
                boots.bootsCombine.Cartridge.UseCartridge(proForJump);

                if (boots.bootsGS.IsDetecting == false)
                    boots.bootsGS.DriveGS();
            }
        }

        public void OnDashStarted(InputAction.CallbackContext _)
        {
            if (!isDashed && boots.bootsCombine.CheckCartridge(proForDash))
            {
                Dash(dashCool);
                boots.bootsCombine.Cartridge.UseCartridge(proForDash);
            }
        }
        #endregion

        // 메서드
        #region Methods
        // 중심 클래스 RidingBoots로부터 각 기능 클래스에 뿌려진 Player 클래스 참조 메서드
        public void SetPlayer(Player player)
        {
            this.player = player;

            if (player != null)
                rbPlayer = player.GetComponent<Rigidbody>();
        }

        // 부츠 타입 라이딩기어의 점프 기능 메서드
        public async void Jump(bool isGrounded, float coolTime)
        {
            // 점프 시작
            isJumped = true;

            Vector2 moveInput = player.playerMove.MoveInput;
            Vector2 lastMoveDirection = player.playerMove.LastMoveDirection;

            /*
            현실적인 움직임에서, 초능력을 쓰지 않는 이상 기본적으로 공중에서의 이동 방향 전환은 불가능하다
            하지만 부츠 타입 라이딩기어는 연속 점프 기능을 허용하고, 세계관 설정 상 카트리지 추진제의 폭발력으로 점프를 행하므로
            체공 중 다음 점프는 원하는 방향으로 힘을 가하는 방식을 채택할 수 있다
            */
            // 이동 방향 연산
            Vector3 currentDirection = (isGrounded) ? new Vector3(lastMoveDirection.x, 0, lastMoveDirection.y) : new Vector3(moveInput.x, 0, moveInput.y);

            Vector3 forward = player.transform.forward;
            Vector3 right = player.transform.right;

            forward.y = 0;
            right.y = 0;

            Vector3 moveDirection = (right * currentDirection.x + forward * currentDirection.z).normalized;

            // 점프 방향
            Vector3 jumpDirection = (moveDirection + player.transform.up).normalized;

            rbPlayer.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

            // 점프 쿨다운
            await Task.Delay((int)(coolTime * 1000));

            // 점프 종료
            isJumped = false;
        }

        // 부츠 타입 라이딩기어의 대시 기능 메서드
        private async void Dash(float coolTime)
        {
            // 대시는 플레이어가 체공 중이면 쓸 수 없는 기능이다
            if (player.playerStates.isGrounded == false && player.playerStates.isClimbing == false) return;
            if (isDashed == true) return;
            isDashed = true;

            // 먼저 PhysicMaterial을 참조하고
            PhysicsMaterial material = boots.transform.GetComponent<CapsuleCollider>().material;

            // 일시적으로 지면과 라이딩기어 중 더 낮은 쪽의 마찰력 계수를 적용 (라이딩기어 = 0)
            material.frictionCombine = PhysicsMaterialCombine.Minimum;

            // 대시 실행
            rbPlayer.AddForce(player.transform.forward * dashSpeed, ForceMode.VelocityChange);

            // 지면과 라이딩기어 중 더 높은 쪽의 마찰력 계수를 적용하여 정지
            await Task.Delay((int)(0.3 * 1000));
            material.frictionCombine = PhysicsMaterialCombine.Maximum;

            if (player.playerStates.isGrounded == true || player.playerStates.isClimbing == true)
                rbPlayer.AddForce(2
                    * Physics.gravity.magnitude
                    * rbPlayer.mass
                    * -player.transform.up, ForceMode.Impulse);

            // 쿨다운
            await Task.Delay((int)((coolTime - 0.3) * 1000));
            isDashed = false;
        }
        #endregion
    }
}

#region Dummy
/*
public class RidingBootsMove : MonoBehaviour
{
    // 기능 목록
    #region 기능 구현 계획 및 메모
    /*
    1. 점프 기능 (Jump)
    - 플레이어가 지면에 닿아 있는 상태(isGrounded)일 때만 사용 가능
    - 점프할 때 전방 이동 방향도 함께 고려하여 점프 방향 계산
    - 이중 점프 기능은 고려하지 않음 (추가 가능성 있음)

    2. 카트리지 시스템 (Cartridge System)
    - 부츠의 점프 기능을 제한하기 위해 카트리지 시스템 도입
    - 카트리지를 소모해 점프를 하며, 카트리지가 부족할 경우 점프 불가
    - 카트리지는 일정 시간 충전하거나 아이템을 통해 보급 가능하도록 설계
    - 카트리지 UI 구현 필요 (화면에 남은 카트리지 수량 표시)

    3. 인핸스 모드 (Enhance Mode)
    - 벽 등반(Climb) 기능: 지정된 벽면(Wall 레이어)에서만 등반 가능
    - 직립 유지(StandStraight) 기능: 벽면 등반 시에도 플레이어가 직립 유지
    - 중력 조정(Gravity Control): 벽면 등반 시 중력을 조정하여 플레이어가 붙어 있도록 함
    - 벽 타기 중엔 다른 점프 불가, 중력 조정이 완료되면 등반 가능 상태 전환

    4. 트리거 콜라이더 (Trigger Collider)
    - 트리거 콜라이더 추가: 플레이어 주변에 트리거 콜라이더를 배치하여 라이딩기어와 상호작용할 수 있도록 설계
    - 라이딩기어와의 상호작용은 오직 트리거 내에서만 가능
    - 트리거 반경은 플레이어 주변 약 5m, 해당 반경 내에 진입한 라이딩기어를 감지하여 플레이어가 탑승 가능 여부를 판단

    5. 향후 개선 사항
    - 점프 시의 반응성을 개선하기 위해 애니메이션 추가 검토
    - 카트리지 시스템과 UI를 더 직관적으로 구성 (예: 잔여량 표시 애니메이션)
    - 중력 조정 기능 테스트 중: 다른 오브젝트와의 충돌 문제 해결 필요
}
*/
#endregion