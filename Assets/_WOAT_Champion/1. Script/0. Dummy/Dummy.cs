using UnityEngine;

namespace WOAT
{
    public class Dummy : MonoBehaviour
    {
        #region 종합 회전
        /*
        public void Rotation()
        {
            //각 축의 Delta 값
            headXRotation -= lookInput.y * mouseSensitivity;
            //Y축 회전은 조건에 따라 세분화해야 하므로 if-else문 내에서 연산

            //각 축의 한계 회전각
            headXRotation = Mathf.Clamp(headXRotation, -50f, 50f);
            headYRotation = Mathf.Clamp(headYRotation, -80f, 80f);

            if (isBoard != true)
            {
                //KeepGoing == true, head만 회전 가능
                if (isKeepGoing == true)
                {
                    headYRotation += lookInput.x * mouseSensitivity;
                    headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
                }

                //KeepGoing == false, head 상하 회전, body 좌우 회전
                else
                {
                    //head를 body의 방향으로 동기화, head는 사실상 상하 회전만 가능
                    headYRotation = Mathf.Lerp(headYRotation, 0, syncSensitivity * Time.deltaTime);
                    headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);

                    //body 회전 시작
                    if (lookInput != Vector2.zero)
                        bodyYRotation += lookInput.x * mouseSensitivity * Time.deltaTime;
                    else
                        bodyYRotation = 0;

                    rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, bodyYRotation, 0f));
                }
            }

            //플레이어가 보드에 탑승 중일 경우
            else
            {
                if (isKeepGoing == true)
                {
                    headYRotation += lookInput.x * mouseSensitivity;
                    headTransform.localRotation = Quaternion.Euler(headXRotation, headYRotation, 0f);
                }

                else
                {
                    InitializeRotation();
                    headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, Quaternion.identity, 0.1f);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, gearTransform.localRotation, 0.2f));
                }
            }
        }
        */
        #endregion

        #region async/await
        /*
        private async void Dash(float coolTime)
        {
            2. 카트리지 소비량이 점프보다 적을 것
            카트리지 미구현

            // 대시는 플레이어가 체공 중이면 쓸 수 없는 기능이다
            if (player.playerStates.isGrounded == false && player.playerStates.isClimbing == false) return;

            // 먼저 PhysicMaterial을 참조하고
            PhysicMaterial material = boots.transform.GetComponent<CapsuleCollider>().material;

            // 일시적으로 지면과 라이딩기어 중 더 낮은 쪽의 마찰력 계수를 적용 (라이딩기어 = 0)
            material.frictionCombine = PhysicMaterialCombine.Minimum;

            // 대시 실행
            rbPlayer.AddForce(player.transform.forward* dashSpeed, ForceMode.VelocityChange);

            await Task.Delay((int)(coolTime* 1000));

            // 지면과 라이딩기어 중 더 높은 쪽의 마찰력 계수를 적용하여 정지
            material.frictionCombine = PhysicMaterialCombine.Maximum;
        }
        */
        #endregion 
    }
}