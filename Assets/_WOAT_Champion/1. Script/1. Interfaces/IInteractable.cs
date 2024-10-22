namespace WOAT
{
    // 상호작용 가능한 오브젝트, 즉 주워서 인벤토리에 담을 수 있는 아이템의 인터페이스
    public interface IInteractable
    {
        // 아이템을 주울 때 반드시 해당 아이템의 정보를 읽어야 하므로 이를 보장
        public ItemKey GetItemData();
    }
}