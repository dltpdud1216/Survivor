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
        public float explosionRadius = 3.0f; // 판정 범위를 조금 더 넓혔습니다 😤
        public float slowAmount = 0.5f;

        private Vector3 direction;
        private float speed;
        private bool isInitialized = false;
        private HashSet<int> hitEnemyIDs = new HashSet<int>();

        public void Setup(Vector3 dir, float dmg, float projSpeed, float extra, Transform target = null)
        {
            this.direction = dir.normalized;
            this.damage = dmg;
            this.speed = projSpeed;
            isInitialized = true;

            // 벼락(Explosion)은 제자리 유지, 투사체는 발사
            if (type == ProjectileType.Explosion)
                Destroy(gameObject, 1.5f);
            else
                Destroy(gameObject, 5f);
        }

        private void Update()
        {
            if (!isInitialized || type == ProjectileType.Explosion) return;
            transform.position += direction * speed * Time.deltaTime;
        }

        // 😤 화염구 등 투사체 충돌 시 호출 (ProjectileCollision.cs에서 부름)
        public void OnTargetHit(Collider2D enemyCollider)
        {
            if (!isInitialized || enemyCollider == null) return;
            if (!enemyCollider.CompareTag("Enemy")) return;

            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"🔥 투사체 적중! 데미지: {damage}");

                // 관통 기능이 없다면 여기서 파괴
                Destroy(gameObject);
            }
        }

        // ⚡ [애니메이션 이벤트: 20프레임] 벼락 타격
        public void ThunderHit()
        {
            hitEnemyIDs.Clear();
            Explode();
        }

        public void ThunderSlow()
        {
            Explode(); // 😤 데미지가 확실히 들어가게 슬로우 시점에도 체크
        }

        private void Explode()
        {
            // 😤 'Enemy' 태그를 가진 모든 콜라이더를 긁어옵니다.
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (var res in results)
            {
                if (res.CompareTag("Enemy"))
                {
                    Enemy enemy = res.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        Debug.Log($"⚡ 벼락 폭발 적중: {enemy.name}, 데미지: {damage}");
                    }
                }
            }
        }

        // 😤 에디터 Scene 뷰에서 폭발 범위를 노란 원으로 보여줍니다.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}