using UnityEngine;

namespace Survivor
{
    public class MagnetSkill : MonoBehaviour
    {
        private PlayerStats stats;
        public float baseIncrease = 1.5f;

        void Awake() { stats = GetComponentInParent<PlayerStats>(); }

        public void ApplyMagnetEffect()
        {
            if (stats == null) stats = GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                // 😤 현재 범위를 기준으로 1.5배 곱해버립니다 (복리 적용)
                stats.magnetRange *= 1.5f;
                Debug.Log($"🧲 자석 범위 1.5배 강화! 현재 범위: {stats.magnetRange}");
            }
        }
    }
}