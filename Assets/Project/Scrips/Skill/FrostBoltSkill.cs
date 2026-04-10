using UnityEngine;

namespace Survivor
{
    public class FrostBoltSkill : MonoBehaviour
    {
        public SkillData skillData;
        public GameObject frostBoltPrefab;
        public float shootInterval = 1.5f;
        public float damage = 15f;
        public float projectileSpeed = 12f;
        public int basePierceCount = 2;

        private float timer;
        private PlayerStats stats;

        void Start() { stats = GetComponentInParent<PlayerStats>(); }

        void Update()
        {
            float currentCooldown = (stats != null) ? shootInterval * stats.cooldownMultiplier : shootInterval;

            timer += Time.deltaTime;
            if (timer >= currentCooldown) { Shoot(); timer = 0f; }
        }

        void Shoot()
        {
            // 😤 1. 가장 가까운 적을 찾습니다.
            GameObject targetObj = FindNearestEnemy();
            Vector3 shootDir;

            if (targetObj != null)
            {
                // 😤 2. 적이 있으면 적 방향으로 계산
                shootDir = (targetObj.transform.position - transform.position).normalized;
            }
            else
            {
                // 😤 3. 적이 없으면 플레이어가 바라보는 방향(오른쪽)으로 쏨
                shootDir = transform.right;
            }

            GameObject go = Instantiate(frostBoltPrefab, transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Straight;
                proj.pierceCount = basePierceCount;

                float finalDmg = (stats != null) ? damage * stats.damageMultiplier : damage;
                float finalSize = Mathf.Pow(1.5f, skillData.level);
                if (stats != null) finalSize *= stats.attackRangeArea;

                // 😤 4. 계산된 방향으로 투사체 설정
                proj.Setup(shootDir, finalDmg, projectileSpeed, finalSize);
            }
        }

        // 😤 [핵심 복구] 적을 실제로 찾는 로직입니다.
        GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDistance = Mathf.Infinity;
            Vector3 currentPos = transform.position;

            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(currentPos, enemy.transform.position);
                if (dist < minDistance)
                {
                    nearest = enemy;
                    minDistance = dist;
                }
            }
            return nearest;
        }
    }
}