using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace WOAT
{
    // 부츠 타입 라이딩기어의 인핸스 모드 : Gravity Stride
    // 상시 발동, 벽을 타는 동안 카트리지의 추진제 지속 소모
    public class BootsGS
    {
        // 필드
        #region Variables
        // 단순 변수
        private readonly float detectRadius = 10.0f;
        private float factorGS;

        private bool isDetecting;               // 부츠 타입의 인핸스 모드: GS 전용 변수

        // 복합 변수
        private Vector3 standDirection;         // 직립 방향
        private Vector3 wallDirection;          // 초기 법선 벡터
        private Vector3 closestPoint;           // 벽 등반이 가능한 오브젝트의 표면에서 플레이어와 가장 가까운 지점 변수

        // 인핸스 모드: GS의 실행이 가능한 레이어를 저장하는 리스트
        private readonly List<LayerMask> layerMasks;

        // 컴포넌트
        private Rigidbody rbPlayer;

        // 클래스 컴포넌트
        private RidingBoots boots;
        private Player player;
        #endregion

        // 속성
        #region Properties
        public bool IsDetecting => isDetecting;
        #endregion

        // 생성자
        #region Constructor
        public BootsGS(RidingBoots ridingBoots)
        {
            // 컴포넌트 초기화
            this.boots = ridingBoots;

            // 리스트 초기화
            layerMasks = new List<LayerMask>
            {
                LayerMask.GetMask("Wall"),
                LayerMask.GetMask("Earth")
            };
        }
        #endregion

        // 메서드
        #region Methods
        public void SetPlayer(Player player)
        {
            this.player = player;

            if (player == null) return;
            this.rbPlayer = player.transform.GetComponent<Rigidbody>();
        }

        // 인핸스 모드: Gravity Stride
        public void EnhanceMode()
        {
            // 벽면에 수직하게 서기
            StandStraight();

            // 벽면에 수직하게 가중력 생성
            GravityControl();

            // 인핸스 모드를 실행 중일 땐 초당 ((가중력+수직항력)/중력) * 0.3f 만큼 카트리지를 소모한다
            boots.bootsCombine.Cartridge.UseCartridge(factorGS * 0.3f * Time.fixedDeltaTime);
        }

        public async void DriveGS()
        {
            if (player == null) return;

            if (player.playerStates.isBoard == false)
            {
                // 탐지 루틴 시작까지 약간의 시간 여유
                await Task.Delay(100);

                while (player.playerStates.isGrounded == false)
                {
                    // 비동기적 벽 탐지 기능 활성화
                    isDetecting = true;

                    // 벽 탐지 기능 실행
                    DetectDistance();

                    // 0.5초(500밀리초)에 한 번만 실행
                    await Task.Delay(500);
                }

                // 지면에 착지하면 탐지 끝
                isDetecting = false;
            }
        }

        private void DetectDistance()
        {
            Collider closestCollider = null;        // 가장 가까운 콜라이더를 저장할 변수
            float closestDistance = Mathf.Infinity; // 가장 가까운 거리 초기값을 무한대로 설정

            // 현재 프레임에서 실제로 가장 가까운 객체를 찾기 위해, 매번 초기화
            foreach (LayerMask mask in layerMasks)
            {
                Collider[] hitColliders = Physics.OverlapSphere(boots.transform.position,
                                                                detectRadius,
                                                                mask);

                foreach (var hitCollider in hitColliders)
                {
                    // 오브젝트 표면에서의 가장 가까운 점을 찾음
                    closestPoint = hitCollider.ClosestPoint(boots.transform.position);

                    // 현재 프레임에서의 정확한 거리 계산
                    float distance = (closestPoint - boots.transform.position).sqrMagnitude;

                    // 가장 가까운 객체와의 거리 비교
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = hitCollider;
                    }
                }
            }

            // 가장 가까운 객체가 감지된 경우 법선 벡터를 계산
            if (closestCollider != null && closestCollider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                if (Physics.Raycast(boots.transform.position, (closestPoint - boots.transform.position).normalized, out RaycastHit hit, detectRadius))
                    wallDirection = hit.normal;
        }

        // 플레이어를 벽면에 수직하게 세우는 메서드
        private void StandStraight()
        {
            if (player.playerStates.isClimbing == false) return;

            // wallDirection을 사용해 목표 회전을 계산
            Quaternion targetRotation = Quaternion.FromToRotation(player.transform.up, wallDirection) * player.transform.rotation;

            // 플레이어가 벽면에 서도록 회전
            rbPlayer.MoveRotation(targetRotation);
        }

        // 플레이어가 벽면에 계속 붙어 있을 수 있도록 가중력을 생성하는 메서드
        private void GravityControl()
        {
            if (player.playerStates.isClimbing == false) return;

            // 2. 벽면 법선 벡터의 반대 방향으로 다운포스(가중력) 계산
            Vector3 resultantForce = -wallDirection * Physics.gravity.magnitude; // 프로젝트에 설정된 중력 가속도 값 사용

            factorGS = (resultantForce + Physics.gravity).magnitude / Physics.gravity.magnitude;

            // 3. 계산된 합력 벡터를 플레이어에 적용 (중력 + 벽면 방향 힘)
            rbPlayer.AddForce(resultantForce, ForceMode.Acceleration);
        }
        #endregion
    }
}