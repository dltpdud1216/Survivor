using UnityEngine;

namespace Survivor
{
    public class FireballSkill : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject fireballPrefab;
        public float shootInterval = 2f;
        public float damage = 10f;
        public float projectileSpeed = 10f;

        private float timer;

        private void OnEnable()
        {
            Debug.Log("화염구 스킬 활성화!");
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

            // 😤 타겟이 없으면 플레이어 정면, 있으면 적 방향
            Vector3 shootDir = targetObj != null ?
                (targetObj.transform.position - transform.position).normalized :
                transform.right; // 2D면 보통 right가 정면

            // 생성 시 회전값 계산
            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject go = Instantiate(fireballPrefab, transform.position, spawnRotation);
            go.transform.SetParent(null);

            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null)
            {
                // 타겟 정보를 넘겨줘야 Projectile이 날아갑니다 😤
                proj.Setup(shootDir, damage, projectileSpeed, 0f, targetObj != null ? targetObj.transform : null);
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