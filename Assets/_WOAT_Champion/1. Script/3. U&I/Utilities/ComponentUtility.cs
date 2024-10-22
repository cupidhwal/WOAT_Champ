using UnityEngine;

namespace WOAT
{
    // MonoBehaviour에 없는 편의용 메서드 모음
    public static class ComponentUtility
    {
        // 부모 체인에서 T 타입의 컴포넌트를 안전하게 가져오는 메서드 (스태틱 메서드)
        public static bool TryGetComponentInParent<T>(Transform transform, out T component) where T : Component
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }
    }
}