using UnityEngine;

namespace Survivor
{
    public class MagnetSkill : MonoBehaviour
    {
        private PlayerStats stats;
        private CircleCollider2D magnetCollider;

        void Awake()
        {
            stats = GetComponentInParent<PlayerStats>();
            magnetCollider = GetComponent<CircleCollider2D>();

            if (magnetCollider != null)
            {
                magnetCollider.isTrigger = true;
                if (stats != null) magnetCollider.radius = stats.magnetRange;
            }
        }

        // 😤 [수정] PlayerStats에서 호출 시 실제 유니티 콜라이더 반지름을 확장
        public void ApplyMagnetEffect()
        {
            if (stats != null && magnetCollider != null)
            {
                // 수치 동기화
                magnetCollider.radius = stats.magnetRange;
                Debug.Log($"[자석 범위 확장] 물리 Radius가 {magnetCollider.radius}로 업데이트되었습니다.");
            }
        }
    }
}