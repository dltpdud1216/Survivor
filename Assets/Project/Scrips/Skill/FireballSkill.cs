using UnityEngine;

namespace Survivor
{
    public class FireballSkill : MonoBehaviour
    {
        public static FireballSkill Instance;
        public SkillData skillData;

        public GameObject fireballPrefab;
        public GameObject evolvedPrefab;

        public float shootInterval = 2f;
        public float damage = 15f;
        public float projectileSpeed = 10f;

        private float timer;
        private PlayerStats stats;
        public bool isEvolved = false;

        private void Awake() { Instance = this; }
        void Start() { stats = GetComponentInParent<PlayerStats>(); }

        private void Update()
        {
            if (Time.timeScale == 0) return;
            if (!isEvolved && skillData != null && skillData.level >= 5) isEvolved = true;

            float cooldown = (stats != null) ? shootInterval * stats.cooldownMultiplier : shootInterval;
            timer += Time.deltaTime;
            if (timer >= cooldown) { Shoot(); timer = 0f; }
        }

        public void ForceEvolveFireball() { isEvolved = true; }

        void Shoot()
        {
            GameObject prefab = (isEvolved && evolvedPrefab != null) ? evolvedPrefab : fireballPrefab;
            GameObject target = FindNearestEnemy();
            Vector3 dir = target != null ? (target.transform.position - transform.position).normalized : transform.right;

            GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Explosion;
                float finalDmg = (stats != null) ? damage * stats.damageMultiplier : damage;

                // 😤 인자 3개: (방향, 데미지, 속도) - 사이즈 배율은 아예 안 보냄
                proj.Setup(dir, finalDmg, projectileSpeed);
            }
        }

        GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDist = Mathf.Infinity;
            foreach (var e in enemies)
            {
                float dist = Vector3.Distance(transform.position, e.transform.position);
                if (dist < minDist) { minDist = dist; nearest = e; }
            }
            return nearest;
        }
    }
}