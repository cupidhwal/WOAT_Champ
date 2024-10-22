using UnityEngine;

namespace WOAT
{
    public class InteractablePoint : MonoBehaviour
    {
        private Player player;
        private PlayerHead playerHead;

        private void Start()
        {
            player = FindFirstObjectByType<Player>();
            playerHead = player.transform.GetComponentInChildren<PlayerHead>();
        }

        private void LateUpdate()
        {
            AttentionPlayer();
        }

        private void AttentionPlayer()
        {
            Vector3 direction = (playerHead.transform.position - this.transform.position).normalized;

            this.transform.localRotation = Quaternion.LookRotation(direction);
        }
    }
}