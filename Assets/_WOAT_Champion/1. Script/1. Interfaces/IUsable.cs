namespace WOAT
{
    // 실제로 사용할 수 있는 아이템의 기능을 보장하는 인터페이스
    public interface IUsable
    {
        public void UseItem(Player player);
    }
}