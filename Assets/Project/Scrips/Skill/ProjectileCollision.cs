using UnityEngine;

namespace Survivor
{
    public class ProjectileCollision : MonoBehaviour
    {
        private Projectile parentProjectile;

        void Start()
        {
            parentProjectile = GetComponentInParent<Projectile>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (parentProjectile != null)
                {
                    parentProjectile.OnTargetHit(other); // 부모에게 충돌 알림
                }
            }
        }
    }
}