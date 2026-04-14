using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class ProjectileCollision : MonoBehaviour
    {
        private Projectile parent;
        private HashSet<int> hitEnemyIDs = new HashSet<int>();
        private int currentPierce = 0;

        void Start()
        {
            parent = GetComponentInParent<Projectile>();
            if (parent == null) parent = GetComponent<Projectile>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;

            if (parent == null) parent = GetComponentInParent<Projectile>();
            if (parent == null) return;

            int id = other.gameObject.GetInstanceID();
            if (hitEnemyIDs.Contains(id)) return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) enemy = other.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                // 1. 공통 데미지 처리
                enemy.TakeDamage(parent.damage);
                hitEnemyIDs.Add(id);

                // 2. [파이어볼 핵심] 폭발 타입(Explosion)일 경우 처리
                if (parent.type == Projectile.ProjectileType.Explosion)
                {
                    // 😤 여기서 Explode를 호출해야 데미지가 주변까지 쫙 박힙니다!
                    parent.Explode(false);
                    Destroy(parent.gameObject); // 파이어볼은 터지면 삭제
                    return; // 파이어볼은 여기서 로직 종료
                }

                // 3. [얼음화살 핵심] 빙결 처리
                bool isEvolved = parent.isEvolvedProjectile || parent.name.Contains("2_0") || gameObject.name.Contains("2_0");
                if (isEvolved)
                {
                    enemy.ApplyFreeze(1.5f);
                }

                // 4. 관통 처리 (Straight 타입 등)
                if (currentPierce < parent.pierceCount)
                {
                    currentPierce++;
                }
                else
                {
                    Destroy(parent.gameObject);
                }
            }
        }
    }
}