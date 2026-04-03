using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class SpinningBladeSkill : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject bladePrefab;

        [Header("Level & Damage")]
        [Range(1, 3)] public int bladeLevel = 1;
        private float[] damages = { 10f, 20f, 40f };

        [Header("Movement & Size Control")]
        [SerializeField] private float rotationSpeed = 200f;
        [SerializeField] private float baseRadius = 2f;

        // ★중요: 인스펙터에서 0.1 ~ 0.5 사이로 아주 작게 시작해보세요.
        [SerializeField] private float baseScaleValue = 0.2f;
        // ★중요: 레벨업 시 크기가 확 커지지 않게 배율을 조정합니다.
        [SerializeField] private float scaleMultiplierPerLevel = 1.1f;

        private List<GameObject> activeBlades = new List<GameObject>();
        private float currentAngle = 0f;

        void Start()
        {
            UpdateBladeStatus();
        }

        void LateUpdate()
        {
            if (activeBlades.Count == 0) return;

            currentAngle += rotationSpeed * Time.deltaTime;

            // 레벨에 따른 반지름과 크기 계산
            float currentRadius = baseRadius + (bladeLevel - 1) * 0.5f;
            // 1강(Level 2)이면 baseScaleValue * 1.1, 2강(Level 3)이면 * 1.2 이런 식입니다.
            float currentScale = baseScaleValue * (1f + (bladeLevel - 1) * (scaleMultiplierPerLevel - 1f));

            float angleStep = 360f / activeBlades.Count;

            for (int i = 0; i < activeBlades.Count; i++)
            {
                if (activeBlades[i] == null) continue;

                float finalAngle = (currentAngle + (i * angleStep)) * Mathf.Deg2Rad;

                // 위치 고정
                Vector3 offset = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), 0) * currentRadius;
                activeBlades[i].transform.position = transform.position + offset;

                // [해결사] localScale을 우리가 정한 아주 작은 값으로 강제 고정합니다.
                activeBlades[i].transform.localScale = new Vector3(currentScale, currentScale, 1f);

                // 방향 고정
                activeBlades[i].transform.right = (activeBlades[i].transform.position - transform.position).normalized;
            }
        }

        public void LevelUp()
        {
            if (bladeLevel < 3)
            {
                bladeLevel++;
                UpdateBladeStatus();
            }
        }

        private void UpdateBladeStatus()
        {
            foreach (var b in activeBlades) if (b != null) Destroy(b);
            activeBlades.Clear();

            int count = bladeLevel + 1;
            float dmg = damages[Mathf.Clamp(bladeLevel - 1, 0, damages.Length - 1)];

            for (int i = 0; i < count; i++)
            {
                GameObject newBlade = Instantiate(bladePrefab);
                if (newBlade.TryGetComponent<Blade>(out Blade b))
                {
                    b.currentDamage = dmg;
                }
                activeBlades.Add(newBlade);
            }
        }
    }
}