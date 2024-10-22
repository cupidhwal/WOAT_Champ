namespace WOAT
{
    public class GravityStrider : RidingBoots
    {
        // 라이프 사이클
        #region Life Cycle
        protected override void Awake()
        {
            base.Awake();

            // 실제 변수값 할당
            dashSpeed = 50;
            jumpForce = 1600;

            // 기능 클래스를 생성자로 인스턴스화
            bootsCombine = new (this);
            bootsMove = new (this, dashSpeed, jumpForce);
            bootsGS = new (this);
        }
        #endregion
    }
}