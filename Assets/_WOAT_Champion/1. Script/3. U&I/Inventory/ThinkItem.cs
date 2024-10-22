using UnityEngine;

namespace WOAT
{
    public class ThinkItem : MonoBehaviour
    {
        public Player player;
        private PlayerHead playerHead;

        private void Awake()
        {
            playerHead = player.transform.GetComponentInChildren<PlayerHead>();
        }

        private void OnEnable()
        {
            AttentionPlayer();
        }

        private void AttentionPlayer()
        {
            Vector3 direction = (this.transform.position - playerHead.transform.position).normalized;

            this.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}