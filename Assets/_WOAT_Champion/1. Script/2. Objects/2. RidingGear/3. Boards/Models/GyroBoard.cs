namespace WOAT
{
    // 보드 타입 라이딩기어의 GyroBoard 모델 클래스
    // 상속받은 라이프 사이클 Start 메서드에 기능 클래스의 생성자로 초기화를 진행하여 해당 모델의 성능을 결정한다
    public class GyroBoard : RidingBoard
    {
        // 라이프 사이클
        #region Life Cycle
        protected override void Awake()
        {
            // 실제 변수값 할당
            maxSpeed = 50;
            reverseSpeed = 20;
            turnSpeed = 4;
            tiltSpeed = 5;
            initialForce = 500;
            moveForce = 5000;
            downForce = 2500;
            brakeCoefficient = 0.5f;

            base.Awake();

            // 기능 클래스를 생성자로 인스턴스화
            boardDrive = new BoardDrive(this, maxSpeed, reverseSpeed, turnSpeed, tiltSpeed, initialForce, moveForce, downForce, brakeCoefficient);
        }
        #endregion
    }
}