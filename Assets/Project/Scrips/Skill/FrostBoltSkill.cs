using UnityEngine;

namespace Survivor
{
    public class FrostBoltSkill : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject frostBoltPrefab;
        public float shootInterval = 1.5f;
        public float damage = 8f;
        public float projectileSpeed = 12f;
        public float freezeDuration = 2f;

        private float timer;

        private void OnEnable()
        {
            Debug.Log("얼음 화살 스킬 활성화!");
            // 😤 [수정] 시작하자마자 쏘지 않도록 0으로 초기화
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

            // 😤 타겟 없으면 정면, 있으면 적 방향
            Vector3 shootDir = targetObj != null ?
                (targetObj.transform.position - transform.position).normalized :
                transform.right;

            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject go = Instantiate(frostBoltPrefab, transform.position, spawnRotation);
            go.transform.SetParent(null);

            Projectile projectile = go.GetComponent<Projectile>();
            if (projectile != null)
            {
                // 😤 [수정] 인자 5개 정확히 전달 (타겟 데이터 필수!)
                projectile.Setup(shootDir, damage, projectileSpeed, freezeDuration, targetObj != null ? targetObj.transform : null);
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