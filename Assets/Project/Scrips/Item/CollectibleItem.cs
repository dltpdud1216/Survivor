using UnityEngine;

namespace Survivor
{
    public class CollectibleItem : MonoBehaviour
    {
        public float expAmount = 5f;
        public float flySpeed = 10f; // 속도를 살짝 올리는 게 타격감이 좋습니다 😤

        private bool isFlying = false;
        private Transform playerTransform;
        private PlayerStats playerStats; // 캐싱해서 사용 😤

        void Update()
        {
            // 1. 플레이어 참조가 없으면 찾기
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    playerStats = player.GetComponent<PlayerStats>(); // 여기서 한 번만 가져오기
                }
                return;
            }

            // 2. 거리 계산
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 3. 자석 범위 체크 (캐싱된 stats 사용)
            if (!isFlying && playerStats != null)
            {
                if (distance <= playerStats.magnetRange)
                {
                    isFlying = true;
                }
            }

            // 4. 비행 및 획득 로직
            if (isFlying)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    playerTransform.position, flySpeed * Time.deltaTime);

                // 닿으면 획득 (Trigger 대신 거리로 판정할 때 0.1f면 적당합니다)
                if (distance < 0.2f)
                {
                    if (playerStats != null)
                    {
                        playerStats.GetExp(expAmount);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}