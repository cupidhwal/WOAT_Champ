using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WOAT
{
    // 플레이어의 상호작용을 담당하는 클래스
    // 기능 클래스지만 상호작용 전용 트리거를 전담하여 처리하기 위해 MonoBehaviour를 쓴다
    public class PlayerInteraction : MonoBehaviour
    {
        // 필드
        #region Variables
        // Dictionary를 사용해 상호작용 오브젝트와 UI를 관리
        private readonly Dictionary<GameObject, GameObject> interactables = new();

        // 상호작용 UI 프리팹
        public GameObject pointerPrefab;

        // 클래스 컴포넌트
        private Player player;
        #endregion

        // 라이프 사이클
        #region Life Cycle
        private void Start()
        {
            player = GetComponent<Player>();
        }
        #endregion

        // 이벤트 핸들러
        #region Event Handlers
        public void OnInteractionStarted(InputAction.CallbackContext _) => ThisIsMine();
        #endregion

        // 메서드
        #region Methods
        // 상호작용이 가능한 오브젝트 줍기, UI 제거 및 리스트 최신화
        public void ThisIsMine()
        {
            GameObject closestObject = MathUtility.MinDistance(this.transform.gameObject, interactables);

            // 상호작용 대상이 실제로 존재하면
            if (closestObject != null)
            {
                // 인터페이스로부터 itemData 가져오기
                if (closestObject.TryGetComponent<IInteractable>(out var interactable))
                {
                    ItemKey itemData = interactable.GetItemData();

                    // 인벤토리에 더하는 메서드 호출
                    player.PlayerUse.inventoryManager.AddItem(itemData);
                }

                // 해당 오브젝트 줍기
                Destroy(closestObject);

                // UI 오브젝트 제거
                Destroy(interactables[closestObject]);
                
                // Dictionary에서 해당 오브젝트와 UI 제거
                interactables.Remove(closestObject);
            }
        }

        // "저게 뭐지?" 상호작용 화살표 생성 메서드
        private void WhatIsThat(Collider other)
        {
            // 인터페이스로 상호작용 가능한 오브젝트만 구별
            if (other.gameObject.GetComponent<IInteractable>() == null) return;

            // 해당 오브젝트가 이미 리스트에 있는지 확인
            if (!interactables.ContainsKey(other.gameObject))
            {
                // 상호작용 가능한 오브젝트와 대응하는 UI 오브젝트 생성 및 추가
                GameObject pointerInstance = Instantiate(pointerPrefab, other.transform.position + Vector3.up, Quaternion.identity);
                interactables.Add(other.gameObject, pointerInstance);
            }
        }

        // 상호작용 화살표 유지 메서드
        private void TakeALook()
        {
            foreach (KeyValuePair<GameObject, GameObject> pair in interactables)
            {
                GameObject interactableObject = pair.Key;
                GameObject pointerObject = pair.Value;

                // 리스트의 모든 오브젝트와 그에 대응하는 UI 위치 업데이트
                if (interactableObject != null && pointerObject != null)
                    pointerObject.transform.position = interactableObject.transform.position + Vector3.up;
            }
        }


        // "쓰레기네." 상호작용 화살표 제거 메서드
        private void Sucks(Collider other)
        {
            // 인터페이스로 상호작용 가능한 오브젝트만 구별
            if (other.gameObject.GetComponent<IInteractable>() == null) return;

            // 해당 오브젝트가 리스트에 있는지 확인
            if (interactables.ContainsKey(other.gameObject))
            {
                // UI 오브젝트 제거
                Destroy(interactables[other.gameObject]);

                // Dictionary에서 해당 오브젝트와 UI 제거
                interactables.Remove(other.gameObject);
            }
        }
        #endregion

        // 이벤트 메서드
        #region Event Methods
        private void OnTriggerEnter(Collider other)
        {
            WhatIsThat(other);
        }

        private void OnTriggerStay(Collider _)
        {
            TakeALook();
        }

        private void OnTriggerExit(Collider other)
        {
            Sucks(other);
        }
        #endregion
    }
}

/*
        나중에 라이딩기어에 상호작용 UI를 넣을 경우를 대비한 메서드
        public void ThisIsMine()
        {
            int index = interactableObjects.IndexOf(player.ridingGear.gameObject);

            // 상호작용 대상이 라이딩기어인 경우
            if (player.playerStates.isBoard != null && index >= 0)
            {
                // UI 오브젝트 제거
                Destroy(pointerObjects[index]);

                // 리스트에서 해당 오브젝트와 UI 제거
                interactableObjects.RemoveAt(index);
                pointerObjects.RemoveAt(index);
                index--;
            }
        }














// 상호작용이 가능한 오브젝트 줍기, UI 제거 및 리스트 최신화
        public void ThisIsMine()
        {
            // 플레이어와 가장 가까운 오브젝트 찾기
            float distance = float.MaxValue;
            GameObject closestObject = null;

            foreach (GameObject obj in interactableObjects)
            {
                float dis = Vector3.Distance(this.transform.position, obj.transform.position);
                if (dis < distance)
                {
                    distance = dis;
                    closestObject = obj;
                }
            }

            // 플레이어와 가장 가까운 오브젝트의 인덱스
            int index = interactableObjects.IndexOf(closestObject);

            // 상호작용 대상이 실제로 존재하면
            if (index >= 0)
            {
                // UI 오브젝트 제거
                Destroy(pointerObjects[index]);

                // 리스트에서 해당 오브젝트와 UI 제거
                interactableObjects.RemoveAt(index);
                pointerObjects.RemoveAt(index);
            }
        }

        // "저게 뭐지?" 상호작용 화살표 생성 메서드
        private void WhatIsThat(Collider other)
        {
            // 인터페이스로 상호작용 가능한 오브젝트만 구별
            if (other.gameObject.GetComponent<IInteractable>() == null) return;

            // 해당 오브젝트가 이미 리스트에 있는지 확인
            if (!interactableObjects.Contains(other.gameObject))
            {
                // 상호작용 가능한 오브젝트와 대응하는 UI 오브젝트 생성 및 추가
                interactableObjects.Add(other.gameObject);
                GameObject pointerInstance = Instantiate(pointerPrefab, other.transform.position + Vector3.up, Quaternion.identity);
                pointerObjects.Add(pointerInstance);
            }
        }

        // 상호작용 화살표 유지 메서드
        private void TakeALook()
        {
            for (int i = 0; i < interactableObjects.Count; i++)
            {
                // 리스트의 모든 오브젝트와 그에 대응하는 UI 위치 업데이트
                if (interactableObjects[i] != null && pointerObjects[i] != null)
                    pointerObjects[i].transform.position = interactableObjects[i].transform.position + Vector3.up;
            }
        }

        // "쓰레기네." 상호작용 화살표 제거 메서드
        private void Sucks(Collider other)
        {
            // 인터페이스로 상호작용 가능한 오브젝트만 구별
            if (other.gameObject.GetComponent<IInteractable>() == null) return;

            // 해당 오브젝트가 리스트에 있는지 확인
            int index = interactableObjects.IndexOf(other.gameObject);
            if (index != -1)
            {
                // UI 오브젝트 제거
                Destroy(pointerObjects[index]);

                // 리스트에서 해당 오브젝트와 UI 제거
                interactableObjects.RemoveAt(index);
                pointerObjects.RemoveAt(index);
            }
        }
*/