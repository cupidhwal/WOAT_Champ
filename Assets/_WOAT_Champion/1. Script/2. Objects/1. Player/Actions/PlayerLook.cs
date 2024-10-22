using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 플레이어의 시야 제어를 관리하는 클래스
    public class PlayerLook
    {
        // 필드
        #region Variables
        // 조정값
        private readonly float mouseSensitivity;     // 마우스 감도
        private readonly float syncSensitivity;      // Lerp, Slerp 보간 감도

        // 단순 변수
        private float headXRotation;        // head X축 회전값
        private float headYRotation;        // head Y축 회전값
        private float bodyYRotation;        // body Y축 회전값

        // 주목 기능 전용 변수
        private bool isAttention = false;

        // 복합 변수
        private Vector2 lookInput;          // 마우스 입력

        // 컴포넌트
        private Transform gearTransform;    // 탑승한 라이딩기어의 Transform
        private Transform headTransform;    // 플레이어의 머리 부분 Transform
        private readonly Rigidbody rb;      // 플레이어 Rigidbody

        private readonly Player player;     // 플레이어 클래스 참조
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnLookPerformed(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
            Rotation();     // 마우스 움직임에 따른 시야 회전 처리
        }

        public void OnLookCanceled(InputAction.CallbackContext _) => lookInput = Vector2.zero;
        #endregion

        // 생성자
        #region Constructor
        public PlayerLook(Player player, float mouseSensitivity, float syncSensitivity)
        {
            this.player = player;
            this.rb = player.transform.GetComponent<Rigidbody>();
            this.headTransform = player.transform.Find("Neck");     // 목 부분을 찾아 머리 회전용으로 사용

            this.mouseSensitivity = mouseSensitivity;
            this.syncSensitivity = syncSensitivity;

            player.playerStates.isKeepGoing = false;                // 초기화 시 KeepGoing 상태 false로 설정
        }
        #endregion

        // 메서드
        #region Methods
        // 시야 회전을 초기화하는 메서드
        private void InitializeRotation(float headX, float headY, float bodyY)
        {
            headXRotation = headX;
            headYRotation = headY;
            bodyYRotation = bodyY;
        }

        // 탑승한 라이딩기어를 설정하는 메서드
        public void SetThisGear(Transform gearTransform)
        {
            this.gearTransform = gearTransform;
        }

        // Player의 기본 시야 제어, Update 사용 안 함
        // KeepGoing 중의 헤드 단독 제어 및 기본 제어
        private void Rotation()
        {
            // 각 축의 Delta 값
            headXRotation -= lookInput.y * mouseSensitivity;

            // 각 축의 한계 회전각
            headXRotation = Mathf.Clamp(headXRotation, -50f, 50f);
            headYRotation = Mathf.Clamp(headYRotation, -80f, 80f);

            // 플레이어가 부츠에 탑승 중이거나 기본 상태일 경우
            if (player.playerStates.isBoard != true)
            {
                // KeepGoing == true일 때, head만 회전 가능
                if (player.playerStates.isKeepGoing == true)
                {
                    headYRotation += lookInput.x * mouseSensitivity;
                    headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
                }
                // KeepGoing == false일 때, head 상하 회전, body 좌우 회전
                else
                {
                    // body 회전 시작
                    if (lookInput != Vector2.zero)
                        bodyYRotation = lookInput.x * mouseSensitivity;
                    else
                        bodyYRotation = 0;

                    rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, bodyYRotation, 0f));
                }
            }

            // 플레이어가 보드에 탑승 중일 경우
            else if (player.playerStates.isKeepGoing == true)
            {
                headYRotation += lookInput.x * mouseSensitivity;
                headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
            }
        }

        // KeepGoing 해제 시 시야 방향을 초기화하는 메서드
        public void SyncRotationHead()
        {
            if (player.playerStates.isKeepGoing == true) return;

            // 플레이어가 기본 상태일 경우
            if (player.playerStates.isBoard == null)
            {
                // 주목 기능
                #region Attention
                if (player.playerRide.ridableGears.Count > 0)
                {
                    Vector3 targetDirection = (player.ridingGear.transform.position - headTransform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                    if (isAttention == false)
                    {
                        // 주목 기능 시작
                        OnAttentionEnter(targetDirection, targetRotation);
                        return;
                    }

                    else
                    {
                        // 주목 기능 진행
                        OnAttentionStay(targetRotation);
                        return;
                    }
                }

                if (player.playerRide.ridableGears.Count == 0 && isAttention == true)
                {
                    // 주목 기능 종료
                    OnAttentionExit(headTransform.rotation, rb.transform.rotation);
                    return;
                }
                #endregion

                // head를 body의 방향으로 동기화
                headYRotation = Mathf.Lerp(headYRotation, 0, syncSensitivity * Time.deltaTime);

                // head는 사실상 상하 회전만 가능
                headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
            }

            // 플레이어가 부츠에 탑승 중일 경우
            else if (player.playerStates.isBoard == false)
            {
                // head를 body의 방향으로 동기화
                headYRotation = Mathf.Lerp(headYRotation, 0, syncSensitivity * Time.deltaTime);

                // head는 사실상 상하 회전만 가능
                headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
            }

            // 플레이어가 보드에 탑승 중일 경우
            else
            {
                // 방향값 초기화
                InitializeRotation(0f, -DefineSync(), 0f);

                // body가 보드의 정면 방향으로부터 Y축 방향으로 -80/80도 회전한 상태이므로
                Quaternion targetRotation = Quaternion.Euler(0f, -DefineSync(), 0f);

                // head를 보드의 정면 방향으로 동기화
                headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, targetRotation, 0.1f);
            }
        }

        public void SyncRotationBody()
        {
            if (player.playerStates.isKeepGoing == true) return;

            // 플레이어가 기본 상태일 경우
            if (player.playerStates.isBoard == null)
                // body를 지면의 수직 방향으로 동기화
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0), 0.2f));

            // 플레이어가 부츠에 탑승 중일 경우
            else if (player.playerStates.isBoard == false && player.playerStates.isGrounded == true)
                // body를 지면의 수직 방향으로 동기화
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0), 0.2f));

            // 플레이어가 보드에 탑승 중일 경우
            else if(player.playerStates.isBoard == true)
            {
                // 동기화 회전 방향을 보드의 정면으로부터 Y축으로 -80/80도 회전한 방향으로 잡고
                Quaternion targetRotation = gearTransform.localRotation * Quaternion.Euler(0f, DefineSync(), 0f);

                // body를 해당 방향으로 동기화
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.2f));
            }
        }

        // 주목 기능 - 특정 오브젝트를 바라보는 메서드
        #region Attention Methods
        private void OnAttentionEnter(Vector3 gearDir, Quaternion gearRot)
        {
            if (Vector3.Angle(headTransform.forward, gearDir) > 10)
                headTransform.rotation = Quaternion.Slerp(headTransform.rotation, gearRot, 0.1f);
            else
                isAttention = true;
        }

        private void OnAttentionStay(Quaternion gear)
        {
            headTransform.rotation = gear;
        }

        private void OnAttentionExit(Quaternion head, Quaternion body)
        {
            // 주목 기능이 해제될 경우를 대비하여 각 축의 회전값 초기화
            InitializeRotation(0f, 0f, 0f);

            if (Vector3.Angle(headTransform.forward, rb.transform.forward) > 2.5)
                headTransform.rotation = Quaternion.Slerp(head, body, 0.05f);
            else
                isAttention = false;
        }
        #endregion

        // 보드 탑승 직전 플레이어의 위치에 따라 동기화 방향을 결정하는 메서드
        private float DefineSync()
        {
            RidingBoard ridingBoard = (RidingBoard)player.ridingGear;
            return ridingBoard.IsRight ? -80f : 80f;
        }
        #endregion
    }
}