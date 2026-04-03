using UnityEngine;

namespace Survivor
{
    public class FireballSkill : MonoBehaviour
    {
        public GameObject fireballPrefab;
        public float damage = 15f;
        public float fireRate = 1.2f;
        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                Shoot();
                timer = 0;
            }
        }

        void Shoot()
        {
            GameObject target = FindClosestEnemy();
            if (target == null) return;

            // 투사체 생성 및 방향 설정
            GameObject go = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Vector2 dir = (target.transform.position - transform.position).normalized;

            // 공용 Projectile 스크립트 사용
            go.GetComponent<Projectile>().Setup(dir, damage, 8f);
        }

        GameObject FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float minDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDistance) { closest = enemy; minDistance = dist; }
            }
            return closest;
        }
    }
}