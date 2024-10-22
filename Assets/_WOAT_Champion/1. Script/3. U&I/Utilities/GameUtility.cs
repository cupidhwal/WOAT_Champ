using UnityEngine;

namespace WOAT
{
    // 에디터에서 직접적으로 사용할 유틸리티
    public static class GameUtility
    {
        public static void Gravity(Transform target, float gravityStrength = 9.81f)
        {
            Vector3 currentVelocity = Vector3.zero;
            Vector3 gravity = new(0, -gravityStrength, 0);
            currentVelocity += gravity * Time.deltaTime;
            target.position += currentVelocity * Time.deltaTime;
        }
    }
}