using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 플레이어의 중심 클래스
    // 플레이어의 각종 기능 클래스를 취합하고 총괄한다.
    public class Player : MonoBehaviour
    {
        // 필드
        #region Variables
        public List<RidingGear> ridableGears;

        // 단순 변수
        private readonly float moveSpeed = 7.5f;
        private readonly float JumpForce = 500f;
        private readonly float mouseSensitivity = 0.1f;
        private readonly float syncSensitivity = 10f;

        // 컨트롤러 획득
        public Control control;

        // 플레이어 기능 클래스
        private PlayerInteraction playerInteraction;
        private PlayerUse playerUse;
        public PlayerStates playerStates;
        public PlayerMove playerMove;
        public PlayerLook playerLook;
        public PlayerRide playerRide;
        public RidingGear ridingGear;           // 실제로 탑승한 라이딩기어

        // 유틸리티
        public CursorUtility cursorUtility;
        #endregion

        // 속성
        #region Properties
        public PlayerInteraction PlayerInteraction => playerInteraction;
        public PlayerUse PlayerUse => playerUse;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            // 참고용
            ridableGears = playerRide.ridableGears;

            // 마우스 커서 잠금
            Cursor.lockState = CursorLockMode.Locked;

            // 기본적으로 비활성화하는 컨트롤
            control.Player.RideOn.Disable();
            control.Player.CursorClick.Disable();
            control.Player.CursorPosition.Disable();

            // 상호작용 키 입력은 일단 꺼둔다
            //control.UI.Disable();
        }

        private void FixedUpdate()
        {
            // 이동 제어
            playerMove.Move();

            // 시야 제어 body 보간
            playerLook.SyncRotationBody();
        }

        private void LateUpdate()
        {
            // 시야 제어 head 보간
            playerLook.SyncRotationHead();
        }

        private void Awake()
        {
            control = new();

            // MonoBehaviour 기능 클래스 가져오기
            playerInteraction = this.transform.GetComponent<PlayerInteraction>();
            playerUse = this.transform.GetComponent<PlayerUse>();

            // 기능 클래스 인스턴스화
            playerStates = new(this);
            playerMove = new(this, moveSpeed, JumpForce);
            playerLook = new(this, mouseSensitivity, syncSensitivity);
            playerRide = new(this);

            // 유틸리티 인스턴스화
            cursorUtility = new(this);
        }

        private void OnEnable()
        {
            // 이동 제어 이벤트 핸들러 구독
            control.Player.Move.performed += playerMove.OnMovePerformed;
            control.Player.Move.canceled += playerMove.OnMoveCanceled;
            control.Player.Jump.started += playerMove.OnJumpStarted;

            // 시야 제어 이벤트 핸들러 구독
            control.Player.Look.performed += playerLook.OnLookPerformed;
            control.Player.Look.canceled += playerLook.OnLookCanceled;

            // 탑승 제어 이벤트 핸들러 구독
            control.Player.RideOn.started += playerRide.OnRideOnStarted;

            // 상호작용 이벤트 핸들러 구독
            control.Player.Interaction.started += playerInteraction.OnInteractionStarted;

            // 퀵슬롯 이벤트 구독
            control.Player.Inventory.started += playerUse.OnInventoryStarted;
            control.Player.QuickSlot1.started += playerUse.OnQuickSlot1DownStarted;
            control.Player.QuickSlot2.started += playerUse.OnQuickSlot2DownStarted;
            control.Player.QuickSlot3.started += playerUse.OnQuickSlot3DownStarted;
            control.Player.CursorClick.started += PlayerUse.inventoryManager.OnCursorClickStarted;
            control.Player.CursorClick.canceled += PlayerUse.inventoryManager.OnCursorClickCanceled;

            // 마우스 커서 제어 이벤트 핸들러 구독
            control.Player.CursorSwitch.started += cursorUtility.OnCursorSwitchStarted;
            control.Player.CursorSwitch.canceled += cursorUtility.OnCursorSwitchCanceled;
            control.Player.CursorPosition.performed += cursorUtility.OnCursorPositionPerformed;

            // Restart 이벤트 핸들러 구독
            control.Player.Restart.started += playerStates.OnRestartStarted;

            // KeepGoing 이벤트 핸들러 구독
            control.Player.KeepGoing.performed += OnKeepGoingPerformed;
            control.Player.KeepGoing.canceled += OnKeepGoingCanceled;

            // 실행취소 이벤트 핸들러 구독
            control.UI.Cancel.started += playerUse.OnCancelStarted;

            // 대리자 이벤트에 메서드 구독
            OnKeepGoing += FlagCheck;

            // 컨트롤 활성화
            control.Player.Enable();
            control.UI.Enable();
        }

        private void OnDisable()
        {
            // 컨트롤 비활성화
            control.UI.Disable();
            control.Player.Disable();

            // 대리자 이벤트에 메서드 구독 해제
            OnKeepGoing -= FlagCheck;

            // 실행취소 이벤트 핸들러 구독 해제
            control.UI.Cancel.started -= playerUse.OnCancelStarted;

            // KeepGoing 이벤트 핸들러 구독 해제
            control.Player.KeepGoing.performed -= OnKeepGoingPerformed;
            control.Player.KeepGoing.canceled -= OnKeepGoingCanceled;

            // Restart 이벤트 핸들러 구독 해제
            control.Player.Restart.started -= playerStates.OnRestartStarted;

            // 마우스 커서 제어 이벤트 핸들러 구독 해제
            control.Player.CursorSwitch.started -= cursorUtility.OnCursorSwitchStarted;
            control.Player.CursorSwitch.canceled -= cursorUtility.OnCursorSwitchCanceled;
            control.Player.CursorPosition.performed -= cursorUtility.OnCursorPositionPerformed;

            // 퀵슬롯 이벤트 구독 해제
            control.Player.Inventory.started -= playerUse.OnInventoryStarted;
            control.Player.QuickSlot1.started -= playerUse.OnQuickSlot1DownStarted;
            control.Player.QuickSlot2.started -= playerUse.OnQuickSlot2DownStarted;
            control.Player.QuickSlot3.started -= playerUse.OnQuickSlot3DownStarted;
            control.Player.CursorClick.started -= PlayerUse.inventoryManager.OnCursorClickStarted;
            control.Player.CursorClick.canceled -= PlayerUse.inventoryManager.OnCursorClickCanceled;

            // 상호작용 이벤트 핸들러 구독 해제
            control.Player.Interaction.started -= playerInteraction.OnInteractionStarted;

            // 탑승 제어 이벤트 핸들러 구독 해제
            control.Player.RideOn.started -= playerRide.OnRideOnStarted;

            // 시야 제어 이벤트 핸들러 구독 해제
            control.Player.Look.performed -= playerLook.OnLookPerformed;
            control.Player.Look.canceled -= playerLook.OnLookCanceled;

            // 이동 제어 이벤트 핸들러 구독 해제
            control.Player.Move.performed -= playerMove.OnMovePerformed;
            control.Player.Move.canceled -= playerMove.OnMoveCanceled;
            control.Player.Jump.started -= playerMove.OnJumpStarted;
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        private void OnKeepGoingPerformed(InputAction.CallbackContext context)
        {
            playerStates.isKeepGoing = true;
            OnKeepGoing?.Invoke();      // 이벤트 호출
        }

        private void OnKeepGoingCanceled(InputAction.CallbackContext context)
        {
            playerStates.isKeepGoing = false;
            OnKeepGoing?.Invoke();      // 이벤트 호출
        }
        #endregion

        // 대리자
        #region Delegate
        // KeepGoing 기능 대리자
        public delegate void KeepGoing();

        public event KeepGoing OnKeepGoing;

        // KeepGoing 기능이란?
        /*
        플레이어가 필요할 때마다 발동하여 head만으로 시야를 제어하는 기능.
        초기 기획의도는, 보드에 탑승할 경우 방향 동기화 때문에 플레이어는 회전을 할 수 없는데
        이때 KeepGoing 기능을 발동하여 키 입력이 유지되는 동안 주변을 확인할 수 있도록 하는 것이었다.
        이를테면 보드 타입 전용 비공식전인 [레이스 온 로드]를 플레이 할 경우,
        상위를 달리는 플레이어는 사고의 위험을 감수하고서라도 라이벌의 추격을 확인하고 싶은 기분을 강하게 느낄 때가 있는데
        KeepGoing 기능은 바로 이 경험을 위한 기능이다.

        그런데 개발 도중, 이 기능을 잘만 활용하면 유저 상호작용의 주요 수단이 될 수 있음을 깨달았다.
        이를테면 기본 상태일 때 라이딩기어가 주변에 있으면 라이딩센스에 의해 주목 기능이 실행되는데,
        KeepGoing 기능을 활성화시키면 라이딩센스 트리거를 일시적으로 비활성화시킬 수 있어 주목 기능도 무시할 수 있게 된다.
        그래서 이 기능의 이벤트/대리자를 발전시켜 몇 가지 기능을 추가적으로 수행하도록 한다.
        */
        public void FlagCheck()
        {
            // 보드에 탑승한 경우
            if (playerStates.isBoard == true)
            {
                if (playerStates.isKeepGoing == true)
                    control.Player.Look.Enable();

                else
                    control.Player.Look.Disable();
            }

            // 부츠에 탑승한 경우, 구현 사항 없음

            // 기본 상태
            else if (playerStates.isBoard == null)
                // isKeepGoing == true => 트리거 Off
                // isKeepGoing == false => 트리거 On
                playerStates.SwitchTrigger(!playerStates.isKeepGoing);
        }

        // 라이딩기어의 OnStanceChanged 이벤트에 구독
        // 플레이어 컨트롤을 중앙 관리하는 메서드
        public void ControlCheck()
        {
            if (playerStates.isBoard != null)
            {
                // 보드에 탑승한 경우
                if (playerStates.isBoard == true)
                {
                    control.Player.Move.Disable();
                    control.Player.Jump.Disable();
                    control.Player.Look.Disable();
                }

                // 부츠에 탑승한 경우
                else if (playerStates.isBoard == false)
                    control.Player.Jump.Disable();

                // 트리거 스위치
                playerStates.SwitchTrigger(false);

                // 탑승하면 탑승 기능 비활성화
                control.Player.RideOn.Disable();
            }

            // 기본 상태
            else
            {
                // 플레이어 컨트롤 활성화
                control.Player.Enable();

                // 탑승 기능은 기본적으로 비활성화
                control.Player.RideOn.Disable();

                // 트리거 스위치
                playerStates.SwitchTrigger(true);
            }
        }
        #endregion

        // 메서드
        #region Methods
        // 라이딩기어에 탑승 시 필요한 참조 변수를 각 기능 클래스에 뿌리는 메서드
        public void SetThisGear(RidingGear ridingGear, Transform gearTransform, bool? isBoard)
        {
            // 해당 라이딩기어의 정보를 각 기능 클래스에 뿌리기
            playerStates.SetThisGear(isBoard);
            playerLook.SetThisGear(gearTransform);

            if (ridingGear != null)
                // 탑승한 라이딩기어를 기억
                this.ridingGear = ridingGear;
        }
        #endregion

        // 이벤트 메서드
        #region Event Methods
        // Joint가 깨질 때 호출되는 메서드
        private void OnJointBreak(float breakForce)
        {
            Debug.Log("앗!!");

            ridingGear.CrashAccident();
        }

        // Trigger 시리즈
        #region OnTrigger
        protected void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("RidingGear"))
                playerRide.RideInteraction(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("RidingGear"))
                playerRide.NullInteraction(other);
        }
        #endregion

        // Collision 시리즈
        #region OnCollision
        private void OnCollisionChange(Collision collision, bool groundedState)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                playerMove.LastMoveDirection = playerMove.MoveInput;
                playerStates.isGrounded = groundedState;
            }
        }

        private void OnCollisionEnter(Collision collision) => OnCollisionChange(collision, true);

        private void OnCollisionStay(Collision collision) => OnCollisionChange(collision, true);

        private void OnCollisionExit(Collision collision) => OnCollisionChange(collision, false);
        #endregion
        #endregion
    }
}