using System.Collections;
using UnityEngine;

namespace WOAT
{
    // 중력 기능을 유사하게 구현하기 위한 모든 아이템의 부모 클래스
    public class Item : MonoBehaviour
    {
        // 필드
        #region Variables
        private bool isGrounded = false;
        private Coroutine gravity = null;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        protected virtual void Start()
        {
            // 처음 생성 시, 중력을 작동시키기 위해 코루틴 시작
            gravity = StartCoroutine(Gravity());
        }
        #endregion

        // 이벤트 메서드
        #region Event Methods
        private void OnCollisionEnter(Collision collision)
        {
            // 지면과 충돌하면 중력 작용을 멈춤
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                if (gravity != null) StopCoroutine(Gravity());
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // 지면에서 떠나면 중력 작용을 시작
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
                gravity = StartCoroutine(Gravity());
            }
        }
        #endregion

        // 유틸리티
        #region Utilities
        IEnumerator Gravity()
        {
            Vector3 velocity = Vector3.zero;
            while (!isGrounded)
            {
                velocity += Physics.gravity * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;

                // Raycast로 지면과의 충돌 검사
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.0f))
                    if (hit.collider.CompareTag("Ground"))
                    {
                        isGrounded = true;
                        transform.position = hit.point; // 지면에 정확히 붙도록 위치 보정
                        velocity = Vector3.zero; // 속도 초기화
                    }

                yield return null;
            }
            yield break;
        }
        #endregion
    }
}