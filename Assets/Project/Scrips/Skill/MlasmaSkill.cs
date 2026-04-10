using UnityEngine;

namespace Survivor
{
    public class MlasmaSkill : MonoBehaviour
    {
        // 😤 강화 로직을 위해 SkillData 추가
        public SkillData skillData;

        [SerializeField] private GameObject mlasmaPrefab;
        [SerializeField] private float baseDamage = 5f; // 기본 데미지
        private GameObject spawnedMlasma;
        private int lastProcessedLevel = -1;

        void Update()
        {
            // 😤 실시간 레벨업 감지: SkillData 레벨이 바뀌면 안개 크기와 데미지 갱신
            if (skillData != null && lastProcessedLevel != skillData.level)
            {
                lastProcessedLevel = skillData.level;
                UpdateMlasmaEffect();
            }
        }

        void OnEnable()
        {
            if (mlasmaPrefab != null && spawnedMlasma == null)
            {
                // 플레이어의 자식으로 소환
                spawnedMlasma = Instantiate(mlasmaPrefab, transform.position, Quaternion.identity, transform);
                UpdateMlasmaEffect(); // 소환 즉시 수치 적용
            }
        }

        private void UpdateMlasmaEffect()
        {
            if (spawnedMlasma == null) return;

            Projectile proj = spawnedMlasma.GetComponent<Projectile>();
            if (proj != null)
            {
                // 😤 [강화 로직] 1.5배 복리 배율 계산 (0강=1배, 1강=1.5배, 2강=2.25배, 3강=3.375배)
                float multiplier = Mathf.Pow(1.5f, skillData.level);

                // 😤 [폭발형 지정] 및 수치 갱신
                proj.type = Projectile.ProjectileType.Explosion;

                // 독 안개는 날아가지 않으므로 speed는 0, multiplier로 크기와 데미지 조절
                proj.Setup(Vector3.zero, baseDamage, 0f, multiplier);

                Debug.Log($"🟣 독 안개 강화 완료! 레벨: {skillData.level}, 배율: {multiplier}");
            }
        }
    }
}