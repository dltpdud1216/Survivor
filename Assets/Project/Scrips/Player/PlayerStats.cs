using UnityEngine;
using Survivor;

public class PlayerStats : MonoBehaviour
{
    [Header("Health System")]
    public float maxHP = 100f;
    public float currentHP;
    [SerializeField] private PlayerHPBar hpBar;

    [Header("Original Stats")]
    // 😤 [에러 해결] 사라졌던 자석 범위를 다시 살려냈습니다!
    public float magnetRange = 1.5f;

    [Header("Passive Stats (ID 104-108)")]
    public float attackRangeArea = 1f;    // 104: 거인의 장갑
    public float damageMultiplier = 1f;  // 105: 힘의 원석
    public float moveSpeedMultiplier = 1f; // 106: 헤르메스의 신발
    public float cooldownMultiplier = 1f;  // 107: 마법시계
    public float critChance = 0.05f;      // 108: 행운의 부적

    [Header("Level System")]
    public int level = 1;
    public float currentExp = 0;
    public float nextLevelExp = 10;

    [Header("UI Reference")]
    [SerializeField] private SkillSelectionUIManager uiManager;

    void Awake() { currentHP = maxHP; }
    void Start() { if (hpBar != null) hpBar.UpdateHPBar(currentHP, maxHP); }

    // 😤 아이템/패시브 업그레이드 함수
    public void UpgradeStat(int itemID)
    {
        switch (itemID)
        {
            case 104: attackRangeArea += 0.2f; break;
            case 105: damageMultiplier += 0.2f; break;
            case 106: moveSpeedMultiplier += 0.15f; break;
            case 107: cooldownMultiplier -= 0.1f; break;
            case 108: critChance += 0.05f; break;

                // 😤 나중에 자석 범위 업그레이드 아이템이 생긴다면 여기서 magnetRange를 늘리면 됩니다.
        }
        cooldownMultiplier = Mathf.Max(cooldownMultiplier, 0.3f);
    }


    public void TakeDamage(float damage)
    {
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