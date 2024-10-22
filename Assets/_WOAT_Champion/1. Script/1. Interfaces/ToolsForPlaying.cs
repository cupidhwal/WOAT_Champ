namespace WOAT
{
    public class ToolsForPlaying
    {
        /*
        //이벤트 대리자
        using System;

        public class Program
        {
            // 대리자 선언
            public delegate void SimpleDelegate();

            // 이벤트 선언: SimpleDelegate 형식의 메서드를 가리킬 수 있는 이벤트
            public static event SimpleDelegate OnGreet;

            // 메서드: 이벤트가 발생할 때 실행될 동작
            public static void SayHello()
            {
                Console.WriteLine("Hello, world!");
            }

            public static void Main(string[] args)
            {
                // 이벤트에 메서드를 구독 (연결)
                OnGreet += SayHello;

                // 조건이 만족되면 이벤트 호출 (상태 변화에 따라 자동 호출)
                if (OnGreet != null)  // 구독된 메서드가 있는지 확인
                {
                    OnGreet();  // 이벤트 발생 (SayHello가 자동으로 호출됨)
                }
            }
        }

        // 이벤트 리스너
        // 라이딩기어 내부
        public event Action<bool> OnRidingStatusChanged;

        protected virtual void ToggleRidingStatus()
        {
            isRiding = !isRiding;
            OnRidingStatusChanged?.Invoke(isRiding); // 상태 변화를 플레이어에게 전달
        }

        // Player 클래스 내부
        public void RegisterRidingGear(RidingGear ridingGear)
        {
            ridingGear.OnRidingStatusChanged += (status) => { isRiding = status; };
        }

        // 인터페이스
        IMovable
            무브, 런
        IJumpable
            점프,
        IDrivable
            턴, 틸트

        // 싱글톤 컨트롤러

        플레이어의 기능
        Move
            수평 이동
            WASD 등
        Run
            직선 이동
            WS키만 사용 가능, 경우에 따라 modifier 필요
        Jump
            수직 이동
            Space
        Eyes
            회전
            마우스 제어
        상호작용
            기타 키 입력
        기타 참고
            라이딩기어와 상호작용

        라이딩기어의 기능
        RideOn
        RideOff
        방향 동기화

        보드 타입
        Move
        Turn
        Tilt
        Enhance Mode AA
            장애물 탐지
            장애물 위치 판정
            회피 경로 계산
            회피 기동
            트릭 점프

        부츠 타입
        무브
        점프
        런
        아이즈
        상호작용
        인핸스 모드
            벽 탐지
            바로 서기
            중력 컨트롤
        */
    }
}