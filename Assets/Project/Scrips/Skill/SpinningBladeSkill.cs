using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class SpinningBladeSkill : MonoBehaviour
    {
        public SkillData skillData;

        [Header("Prefab")]
        [SerializeField] private GameObject bladePrefab;

        [Header("Damage Settings")]
        [SerializeField] private float baseDamage = 15f;

        [Header("Movement & Size Control")]
        [SerializeField] private float rotationSpeed = 200f;
        [SerializeField] private float baseRadius = 2f;

        // 😤 이 값이 0.2라면, 첫 획득 시 정확히 0.2 크기로 나옵니다.
        [SerializeField] private float baseScaleValue = 0.2f;

        private List<GameObject> activeBlades = new List<GameObject>();
        private float currentAngle = 0f;
        private int lastProcessedLevel = -1;

        void Update()
        {
            if (skillData != null && lastProcessedLevel != skillData.level)
            {
                lastProcessedLevel = skillData.level;
                UpdateBladeStatus();
            }
        }

        void LateUpdate()
        {
            if (activeBlades.Count == 0) return;

            currentAngle += rotationSpeed * Time.deltaTime;

            // 😤 레벨 1(첫 획득)일 때 multiplier는 1이 되어야 함
            // 만약 UI 매니저에서 선택 시 레벨이 1이 된다면 (level - 1)로 계산
            float multiplier = Mathf.Pow(1.5f, skillData.level - 1);
            if (multiplier < 1f) multiplier = 1f;

            float currentRadius = baseRadius * multiplier;

            // 😤 [크기 고정] 인스펙터의 baseScaleValue에 배율을 곱함
            float currentScale = baseScaleValue * multiplier;

            float angleStep = 360f / activeBlades.Count;

            for (int i = 0; i < activeBlades.Count; i++)
            {
                if (activeBlades[i] == null) continue;
                float finalAngle = (currentAngle + (i * angleStep)) * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), 0) * currentRadius;
                activeBlades[i].transform.position = transform.position + offset;

                // 😤 매 프레임 여기서 크기를 강제로 박아버립니다.
                activeBlades[i].transform.localScale = new Vector3(currentScale, currentScale, 1f);

                activeBlades[i].transform.right = (activeBlades[i].transform.position - transform.position).normalized;
            }
        }

        private void UpdateBladeStatus()
        {
            foreach (var b in activeBlades) if (b != null) Destroy(b);
            activeBlades.Clear();

            // 첫 획득(레벨 1) 시 2개
            int count = skillData.level + 1;
            if (count < 2) count = 2;

            // 첫 획득(레벨 1) 시 기본 데미지
            float finalDamage = baseDamage * Mathf.Pow(1.5f, skillData.level - 1);

            for (int i = 0; i < count; i++)
            {
                GameObject newBlade = Instantiate(bladePrefab);
                // 😤 칼날은 Projectile 스크립트의 크기 제어를 무시해야 하므로 
                // 만약 Blade 스크립트만 있다면 아래처럼 데미지만 넣어줍니다.
                if (newBlade.TryGetComponent<Blade>(out Blade b))
                {
                    b.currentDamage = finalDamage;
                }
                activeBlades.Add(newBlade);
            }
        }
    }
}