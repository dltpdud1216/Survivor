using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class VoltCrashSkill : MonoBehaviour
    {
        public GameObject lightningPrefab;
        public float shootInterval = 1.2f;
        public float damage = 30f;
        public float scanRadius = 8f;

        private float timer;

        void Update()
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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, scanRadius);
            List<Transform> targets = new List<Transform>();

            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy")) targets.Add(col.transform);
            }

            if (targets.Count == 0) return;

            Transform randomTarget = targets[Random.Range(0, targets.Count)];
            Vector3 spawnPos = randomTarget.position;
            spawnPos.y += 0.5f;

            GameObject go = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.type = Projectile.ProjectileType.Explosion;
                proj.Setup(Vector3.zero, damage, 0f, 0f);
            }
        }
    }
}