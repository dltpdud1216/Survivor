using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    // [Enum 선언] 클래스 바깥에 두어 외부 참조 에러 방지 😤
    public enum ProjectileType { Explosion, Penetration }

    public class Projectile : MonoBehaviour
    {
        [Header("Settings")]
        public ProjectileType type;
        public int pierceCount = 0;
        public float explosionRadius = 2.0f;
        public Vector3 forcedScale = new Vector3(0.5f, 0.5f, 0.5f);

        [Header("Direction Fix")]
        public float rotationOffset = 90f;

        private float speed;
        private float damage;
        private Vector3 direction;
        private Transform target;
        private int currentPierce = 0;
        private bool isDead = false;
        private bool isInitialized = false;
        private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        // 렌더러와 콜라이더 참조
        private SpriteRenderer spriteRenderer;
        private Collider2D bodyCollider;

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bodyCollider = GetComponent<Collider2D>();

            // 😤 [중요] 생성되자마자 일단 숨기고 충돌도 막습니다. (잔상 방지)
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (bodyCollider != null) bodyCollider.enabled = false;
        }

        public void Setup(Vector3 dir, float dmg, float projSpeed, float extra, Transform targetEnemy = null)
        {
            // 데이터 할당
            this.direction = dir.normalized;
            this.damage = dmg;
            this.speed = projSpeed;
            this.target = targetEnemy;

            // 물리/위치 초기화
            transform.SetParent(null);
            transform.localScale = forcedScale;

            // 😤 [핵심] Setup이 호출된 이 순간에만 모습과 충돌을 켭니다.
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (bodyCollider != null) bodyCollider.enabled = true;

            RotateTowardsDirection();
            isInitialized = true;
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            // 셋업 전이거나 죽었다면 이동 금지 😤
            if (!isInitialized || isDead) return;

            if (target != null && target.gameObject.activeInHierarchy)
            {
                direction = (target.position - transform.position).normalized;
            }

            RotateTowardsDirection();
            transform.position += direction * speed * Time.deltaTime;
        }

        private void RotateTowardsDirection()
        {
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }

        public void OnTargetHit(Collider2D enemyCollider)
        {
            if (!isInitialized || isDead || enemyCollider == null) return;

            if (type == ProjectileType.Explosion)
            {
                isDead = true;
                Explode();
                Destroy(gameObject);
            }
            else
            {
                if (hitEnemies.Contains(enemyCollider.gameObject)) return;
                hitEnemies.Add(enemyCollider.gameObject);
                ApplyDamage(enemyCollider);

                if (currentPierce >= pierceCount)
                {
                    isDead = true;
                    Destroy(gameObject);
                }
                else currentPierce++;
            }
        }

        private void ApplyDamage(Collider2D enemy)
        {
            var enemyHealth = enemy.GetComponent<Enemy>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage);
        }

        private void Explode()
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var res in results)
            {
                if (res.CompareTag("Enemy"))
                {
                    var enemyHealth = res.GetComponent<Enemy>();
                    if (enemyHealth != null) enemyHealth.TakeDamage(damage);
                }
            }
        }
    }
}