using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class Projectile : MonoBehaviour
    {
        public enum ProjectileType { Guided, Straight, Explosion }
        public ProjectileType type;

        [Header("Settings")]
        public float damage = 20f;
        public float speed = 7f;
        public float explosionRadius = 3.0f;
        public float slowAmount = 0.5f; // 😤 슬로우 위력
        public int pierceCount = 0;

        [Header("Rotation Settings")]
        public float rotationOffset = 0f;

        private Vector3 direction;
        private bool isInitialized = false;
        private int currentPierce = 0;
        private HashSet<int> hitEnemyIDs = new HashSet<int>();

        private Vector3 initialScale;
        private float initialExplosionRadius;

        private void Awake()
        {
            initialScale = transform.localScale;
            initialExplosionRadius = explosionRadius;
        }

        public void Setup(Vector3 dir, float dmg, float projSpeed, float multiplier, Transform target = null)
        {
            this.direction = dir.normalized;
            float finalMultiplier = (multiplier < 1f) ? 1f : multiplier;

            this.damage = dmg * finalMultiplier;
            this.explosionRadius = initialExplosionRadius * finalMultiplier;
            this.speed = projSpeed;
            transform.localScale = initialScale * finalMultiplier;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

            isInitialized = true;

            if (speed <= 0) Destroy(gameObject, 1.2f); // 😤 연출을 위해 조금 더 길게 유지
            else Destroy(gameObject, 5f);
        }

        private void Update()
        {
            if (!isInitialized || (type == ProjectileType.Explosion && speed <= 0)) return;
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInitialized || !collision.CompareTag("Enemy")) return;
            OnTargetHit(collision);
        }

        public void OnTargetHit(Collider2D enemyCollider)
        {
            int id = enemyCollider.gameObject.GetInstanceID();
            if (hitEnemyIDs.Contains(id)) return;

            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy == null) return;

            enemy.TakeDamage(damage);
            hitEnemyIDs.Add(id);

            if (type == ProjectileType.Explosion)
            {
                Explode(false);
                Destroy(gameObject);
            }
            else
            {
                if (currentPierce < pierceCount) currentPierce++;
                else Destroy(gameObject);
            }
        }

        // 😤 [복구] 애니메이션 이벤트가 찾는 'ThunderHit'
        public void ThunderHit()
        {
            if (!isInitialized) return;
            hitEnemyIDs.Clear();
            Explode(false);
        }

        // 😤 [복구/에러 해결] 애니메이션 이벤트가 찾는 'ThunderSlow'
        public void ThunderSlow()
        {
            if (!isInitialized) return;
            Explode(true); // 슬로우 효과를 켜서 폭발 실행
        }

        private void Explode(bool applySlow)
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var res in results)
            {
                if (res.CompareTag("Enemy"))
                {
                    Enemy enemy = res.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        // 😤 슬로우 적용
                        if (applySlow) enemy.ApplySlow(slowAmount, 1.5f);
                    }
                }
            }
        }
    }
}