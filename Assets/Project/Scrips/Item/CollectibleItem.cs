using UnityEngine;

namespace Survivor
{
    public class CollectibleItem : MonoBehaviour
    {
        public float expAmount = 5f;
        public float flySpeed = 10f;

        private bool isFlying = false;
        private Transform playerTransform;
        private PlayerStats playerStats;

        void Update()
        {
            // 1. 플레이어 참조가 없으면 찾기
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    playerStats = player.GetComponent<PlayerStats>();
                }
                return;
            }

            // 2. 거리 계산
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 3. 자석 범위 체크 (PlayerStats의 magnetRange 수치를 실시간으로 참조 😤)
            if (!isFlying && playerStats != null)
            {
                // 레벨업 시 magnetRange가 커지면, 이 조건문이 더 멀리서도 발동됩니다!
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

                // 닿으면 획득
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