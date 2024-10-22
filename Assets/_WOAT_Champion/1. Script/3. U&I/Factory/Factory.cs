using UnityEngine;

namespace WOAT
{
    // 아이템의 생성과 파괴를 전담하는 클래스
    public class Factory : Singleton<Factory>
    {
        // 필드
        #region Variables
        public ItemDatabase itemDatabase;
        #endregion

        // 메서드
        #region Methods
        #region Core Methods
        // 아이템 생성 메서드
        public GameObject Instantiate(GameObject prefab)
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        public GameObject Instantiate(GameObject prefab, Transform parent)
        {
            return UnityEngine.Object.Instantiate(prefab, parent);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return UnityEngine.Object.Instantiate(prefab, position, rotation);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
        }

        // 아이템 파괴 메서드
        public void Destroy(GameObject obj, float delay = 0f)
        {
            UnityEngine.Object.Destroy(obj, delay);
        }
        #endregion

        // 아이템 사용 가능 여부를 판별하는 중앙 메서드
        private bool CanUse(ItemKey itemKey, IUsable usableItem)
        {
            int itemID = itemKey.itemID;
            bool canUse = false;

            switch (itemID)
            {
                case 0:
                    break;

                default: break;
            }

            if (canUse) return true;
            else return false;
        }
        #endregion
    }
}