using UnityEngine;
using Survivor;

namespace Survivor
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health System")]
        public float maxHP = 100f;
        public float currentHP;
        [SerializeField] private PlayerHPBar hpBar;

        [Header("Original Stats")]
        public float magnetRange = 1.5f;

        [Header("Passive Stats")]
        public float attackRangeArea = 1f;
        public float damageMultiplier = 1f;
        public float moveSpeedMultiplier = 1f;
        public float cooldownMultiplier = 1f;
        public float critChance = 0.05f;

        [Header("Level System")]
        public int level = 1;
        public float currentExp = 0;
        public float nextLevelExp = 10;

        [Header("UI Reference")]
        [SerializeField] private SkillSelectionUIManager uiManager;

        void Awake() { currentHP = maxHP; }
        void Start() { if (hpBar != null) hpBar.UpdateHPBar(currentHP, maxHP); }

        public void UpgradeStat(int itemID)
        {
            switch (itemID)
            {
                case 103: // 😤 자석 범위 업그레이드 (ID 103 확인 완료!)
                    magnetRange += 2.0f;
                    Debug.Log($"자석 수치 증가: {magnetRange}");
                    break;
                case 104: attackRangeArea += 0.2f; break;
                case 105: damageMultiplier += 0.2f; break;
                case 106: moveSpeedMultiplier += 0.15f; break;
                case 107: cooldownMultiplier -= 0.1f; break;
                case 108: critChance += 0.05f; break;
            }
            cooldownMultiplier = Mathf.Max(cooldownMultiplier, 0.3f);
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0) return;
            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            if (hpBar != null) hpBar.UpdateHPBar(currentHP, maxHP);
            if (currentHP <= 0) Die();
        }

        public void GetExp(float amount)
        {
            currentExp += amount;
            if (currentExp >= nextLevelExp) LevelUp();
        }

        void LevelUp()
        {
            level++;
            currentExp -= nextLevelExp;
            nextLevelExp += 10;
            currentHP = maxHP;
            if (hpBar != null) hpBar.UpdateHPBar(currentHP, maxHP);
            if (uiManager != null) uiManager.ShowRandomSkillSelection();
        }

        void Die() { Time.timeScale = 0f; Debug.Log("사망"); }
    }
}