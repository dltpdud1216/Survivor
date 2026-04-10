using UnityEngine;

namespace Survivor
{
    public class FireballSkill : MonoBehaviour
    {
        // 😤 강화 로직을 위해 SkillData 추가
        public SkillData skillData;

        [Header("Settings")]
        public GameObject fireballPrefab;
        public float shootInterval = 2f;
        public float damage = 10f;
        public float projectileSpeed = 10f;

        private float timer;

        private void OnEnable()
        {
            Debug.Log("화염구 스킬 활성화!");
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= shootInterval)
            {
                Shoot();
                timer = 0f;
            }
        }

        void Shoot()
        {
            GameObject targetObj = FindNearestEnemy();

            Vector3 shootDir = targetObj != null ?
                (targetObj.transform.position - transform.position).normalized :
                transform.right;

            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject go = Instantiate(fireballPrefab, transform.position, spawnRotation);
            go.transform.SetParent(null);

            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null)
            {
                // 😤 [폭발형 지정]
                proj.type = Projectile.ProjectileType.Explosion;

                // 😤 [강화 로직] 1.5배 복리 배율 계산
                float multiplier = Mathf.Pow(1.5f, skillData.level);

                // Setup의 4번째 인자로 multiplier 전달
                proj.Setup(shootDir, damage, projectileSpeed, multiplier, targetObj != null ? targetObj.transform : null);
            }
        }

        GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
            return nearest;
        }
    }
}