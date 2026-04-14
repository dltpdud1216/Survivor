using UnityEngine;

namespace Survivor
{
    public class FrostBoltSkill : MonoBehaviour
    {
        public SkillData skillData;
        public GameObject frostBoltPrefab;
        public GameObject evolvedPrefab;

        public float shootInterval = 1.5f;
        public float damage = 15f;
        public float projectileSpeed = 12f;
        public int basePierceCount = 2;

        private float timer;
        private PlayerStats stats;
        public bool isEvolved = false;

        void Start() { stats = GetComponentInParent<PlayerStats>(); }

        void Update()
        {
            if (!isEvolved && skillData != null && skillData.level >= 5) isEvolved = true;

            float currentCooldown = (stats != null) ? shootInterval * stats.cooldownMultiplier : shootInterval;
            timer += Time.deltaTime;
            if (timer >= currentCooldown) { Shoot(); timer = 0f; }
        }

        void Shoot()
        {
            GameObject targetObj = FindNearestEnemy();
            Vector3 shootDir = (targetObj != null) ? (targetObj.transform.position - transform.position).normalized : transform.right;

            GameObject prefabToSpawn = (isEvolved && evolvedPrefab != null) ? evolvedPrefab : frostBoltPrefab;
            GameObject go = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Straight;
                proj.pierceCount = basePierceCount;

                // 😤 데미지 배율 적용
                float finalDmg = (stats != null) ? damage * stats.damageMultiplier : damage;

                // 😤 사이즈 1.0 (원본) 고정
                proj.Setup(shootDir, finalDmg, projectileSpeed, 1.0f, isEvolved);
            }
        }

        GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < minDistance) { nearest = enemy; minDistance = dist; }
            }
            return nearest;
        }
    }
}