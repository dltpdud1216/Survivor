using UnityEngine;

namespace Survivor
{
    public class CollectibleItem : MonoBehaviour
    {
        public float expAmount = 5f;
        private bool isFlying = false;
        private Transform playerTransform;
        private float flySpeed = 8f;

        void Update()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 매 프레임 GetComponent 하는 대신 참조를 가져오거나, 
            // PlayerStats의 magnetRange를 체크합니다.
            PlayerStats stats = playerTransform.GetComponent<PlayerStats>();
            float range = stats.magnetRange;

            if (distance <= range) isFlying = true;

            if (isFlying)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    playerTransform.position, flySpeed * Time.deltaTime);

                if (distance < 0.1f)
                {
                    // 위에서 수정한 PlayerStats의 GetExp를 호출!
                    stats.GetExp(expAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}