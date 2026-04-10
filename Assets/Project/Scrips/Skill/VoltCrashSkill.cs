using UnityEngine;

namespace Survivor
{
    public class VoltCrashSkill : MonoBehaviour
    {
        public SkillData skillData;
        public GameObject thunderPrefab;
        public float shootInterval = 2.0f;
        public float damage = 30f;

        private float timer;
        private PlayerStats stats;

        void Start() { stats = GetComponentInParent<PlayerStats>(); }

        void Update()
        {
            // 😤 [마법시계 적용]
            float currentCooldown = (stats != null) ? shootInterval * stats.cooldownMultiplier : shootInterval;

            timer += Time.deltaTime;
            if (timer >= currentCooldown) { Shoot(); timer = 0f; }
        }

        void Shoot()
        {
            GameObject targetObj = FindNearestEnemy();
            if (targetObj == null) return;

            GameObject go = Instantiate(thunderPrefab, targetObj.transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Explosion;

                // 😤 [힘의 원석 & 거인의 장갑 적용]
                float finalDmg = (stats != null) ? damage * stats.damageMultiplier : damage;
                float finalSize = Mathf.Pow(1.5f, skillData.level);
                if (stats != null) finalSize *= stats.attackRangeArea;

                proj.Setup(Vector3.zero, finalDmg, 0f, finalSize);
                proj.ThunderHit();
            }
        }

        GameObject FindNearestEnemy() { /* 기존 동일 */ return null; }
    }
}