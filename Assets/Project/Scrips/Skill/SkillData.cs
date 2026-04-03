using UnityEngine;

namespace Survivor
{
    public enum SkillType { Active, Passive, Evolution }

    [CreateAssetMenu(fileName = "NewSkill", menuName = "Survivor/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Main Settings")]
        public SkillType type;
        public string skillName;
        public int id;
        public int level = 0; // ★ 추가: 현재 스킬 레벨 (0은 미보유, 1부터 시작)
        public Sprite skillIcon;

        [Header("Description")]
        [TextArea(3, 5)]
        public string description;

        [Header("Evolution Formula (진화용일 때만 사용)")]
        public int requiredActiveId;
        public int requiredPassiveId;

        [Header("Stats")]
        public float damage;
        public float range;
        public float speed;
    }
}