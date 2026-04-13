using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class VoltCrashSkill : MonoBehaviour
    {
        public SkillData skillData;
        public GameObject thunderPrefab;
        public float shootInterval = 2.0f;
        public float damage = 30f;
        public float scanRange = 10f; // 😤 적을 찾는 탐색 범위

        private float timer;
        private PlayerStats stats;

        void Start()
        {
            stats = GetComponentInParent<PlayerStats>();
        }

        void Update()
        {
            // 😤 [마법시계 적용]
            float currentCooldown = (stats != null) ? shootInterval * stats.cooldownMultiplier : shootInterval;

            timer += Time.deltaTime;
            if (timer >= currentCooldown)
            {
                Shoot();
                timer = 0f;
            }
        }

        void Shoot()
        {
            GameObject targetObj = FindNearestEnemy();

            // 😤 적이 없으면 벼락을 안 쏩니다. (가장 큰 원인)
            if (targetObj == null) return;

            // 벼락 생성
            GameObject go = Instantiate(thunderPrefab, targetObj.transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Explosion;

                // 😤 [힘의 원석 & 거인의 장갑 적용]
                float finalDmg = (stats != null) ? damage * stats.damageMultiplier : damage;

                // 스킬 데이터가 있을 때만 레벨 비례 크기 계산 (없으면 기본값 1.5)
                float finalSize = (skillData != null) ? Mathf.Pow(1.5f, skillData.level) : 1.5f;
                if (stats != null) finalSize *= stats.attackRangeArea;

                // 셋업 및 히트 실행
                proj.Setup(Vector3.zero, finalDmg, 0f, finalSize);
                proj.ThunderHit();
            }
        }

        // 😤 적을 찾는 로직 완결본 (이게 비어있으면 벼락 안 나감)
        GameObject FindNearestEnemy()
        {
            // "Enemy" 태그를 가진 모든 오브젝트를 찾음
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDistance = scanRange;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
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