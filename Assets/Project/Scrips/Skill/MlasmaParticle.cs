using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class MlasmaParticle : MonoBehaviour
    {
        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles; // 알갱이 배열

        [SerializeField] private float damage = 10f;
        [SerializeField] private float damageRadius = 1.0f; // ★ 안개 알갱이 하나당 데미지 범위

        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            // 최대 파티클 개수만큼 배열 미리 생성 (최적화)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        void Update()
        {
            // 1. 현재 살아있는 모든 알갱이 정보를 가져옵니다.
            int numParticlesAlive = ps.GetParticles(particles);

            // 2. 알갱이 하나하나를 훑으면서 주변에 적이 있는지 봅니다.
            for (int i = 0; i < numParticlesAlive; i++)
            {
                // 알갱이의 현재 위치 (World 좌표)
                Vector3 particleWorldPos = particles[i].position;

                // 3. 알갱이 주변 damageRadius 안에 "Enemy" 레이어가 있는지 확인
                // 1 << LayerMask.NameToLayer("Enemy")는 Enemy 레이어만 보겠다는 뜻입니다.
                Collider2D hit = Physics2D.OverlapCircle(particleWorldPos, damageRadius, 1 << LayerMask.NameToLayer("Enemy"));

                if (hit != null)
                {
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        // 초당 데미지 (중복 데미지 방지를 위해 수치를 낮게 조절하거나 Time.deltaTime 활용)
                        enemy.TakeDamage(damage * Time.deltaTime);
                    }
                }
            }
        }
    }
}