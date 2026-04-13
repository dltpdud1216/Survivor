using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class SpinningBladeSkill : MonoBehaviour
    {
        public SkillData skillData;

        [Header("Prefabs")]
        [SerializeField] private GameObject bladePrefab;
        [SerializeField] private GameObject evolvedBladePrefab;

        [Header("Base Settings (Level 1)")]
        [SerializeField] private float baseDamage = 15f;
        [SerializeField] private float baseRotationSpeed = 200f; // 기본 회전 속도
        [SerializeField] private float baseRadius = 2f;
        [SerializeField] private float baseScaleValue = 0.2f;

        private List<GameObject> activeBlades = new List<GameObject>();
        private float currentAngle = 0f;
        private int lastProcessedLevel = -1;

        void Update()
        {
            if (skillData == null) return;
            if (lastProcessedLevel != skillData.level)
            {
                lastProcessedLevel = skillData.level;
                UpdateBladeStatus();
            }
        }

        void LateUpdate()
        {
            if (activeBlades.Count == 0) return;

            // 😤 진화 시 회전 속도 1.5배 보너스
            float currentRotationSpeed = baseRotationSpeed;
            if (skillData.type == SkillType.Evolution) currentRotationSpeed *= 1.5f;

            currentAngle += currentRotationSpeed * Time.deltaTime;

            float multiplier = Mathf.Pow(1.5f, Mathf.Max(0, skillData.level - 1));

            // 진화 시 크기 보너스
            if (skillData.type == SkillType.Evolution) multiplier *= 1.5f;

            float currentRadius = baseRadius * multiplier;
            float currentScale = baseScaleValue * multiplier;
            float angleStep = 360f / activeBlades.Count;

            for (int i = 0; i < activeBlades.Count; i++)
            {
                if (activeBlades[i] == null) continue;
                float finalAngle = (currentAngle + (i * angleStep)) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), 0) * currentRadius;
                activeBlades[i].transform.position = transform.position + offset;
                activeBlades[i].transform.localScale = new Vector3(currentScale, currentScale, 1f);
                activeBlades[i].transform.right = (activeBlades[i].transform.position - transform.position).normalized;
            }
        }

        public void ForceEvolveBlade() { UpdateBladeStatus(); }

        private void UpdateBladeStatus()
        {
            foreach (var b in activeBlades) if (b != null) Destroy(b);
            activeBlades.Clear();

            if (skillData == null) return;

            // 1. 프리팹 결정
            GameObject targetPrefab = (skillData.type == SkillType.Evolution && evolvedBladePrefab != null) ? evolvedBladePrefab : bladePrefab;

            // 2. 😤 칼날 개수 결정
            // 일반: 1렙(2개), 2렙(3개), 3렙(4개), 4렙(5개)
            // 진화: 무조건 4개부터 시작 (레벨업 마다 추가 가능)
            int count = (skillData.type == SkillType.Evolution)
                        ? 4 + (skillData.level - 1)
                        : Mathf.Max(2, skillData.level + 1);

            // 3. 😤 데미지 결정 (진화 시 기본 2.5배 뻥튀기)
            float evolutionDamageMult = (skillData.type == SkillType.Evolution) ? 2.5f : 1.0f;
            float finalDamage = baseDamage * Mathf.Pow(1.5f, Mathf.Max(0, skillData.level - 1)) * evolutionDamageMult;

            // 4. 생성 및 데미지 전달
            for (int i = 0; i < count; i++)
            {
                GameObject newBlade = Instantiate(targetPrefab);
                if (newBlade.TryGetComponent<Blade>(out Blade b))
                {
                    b.currentDamage = finalDamage;
                }
                activeBlades.Add(newBlade);
            }

            Debug.Log($"⚔️ {targetPrefab.name} 생성! 개수: {count}, 데미지: {finalDamage}, 속도 보너스 적용됨");
        }
    }
}