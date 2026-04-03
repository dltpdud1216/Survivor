using UnityEngine;
using Survivor; // 네임스페이스 필수

public class PlayerStats : MonoBehaviour
{
    [Header("Level System")]
    public int level = 1;
    public float currentExp = 0; // 아이템이 float(5f)이므로 float 추천
    public float nextLevelExp = 10;

    [Header("Passive Stats")]
    public float magnetRange = 1.5f;
    public float attackRangeArea = 1f;

    [Header("UI Reference")]
    // 인스펙터에서 SkillSelectBG를 드래그해서 넣으세요!
    [SerializeField] private SkillSelectionUIManager uiManager;

    // 아이템(CollectibleItem)이 이 함수를 호출합니다!
    public void GetExp(float amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득: {currentExp} / {nextLevelExp}");

        if (currentExp >= nextLevelExp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= nextLevelExp; // 남은 경험치 이월
        nextLevelExp += 10; // 레벨업 할수록 필요 경험치 증가

        Debug.Log("레벨업! 스킬 선택창을 띄웁니다.");

        if (uiManager != null)
        {
            uiManager.ShowRandomSkillSelection();
        }
        else
        {
            Debug.LogError("PlayerStats에 UI Manager가 연결되지 않았습니다!");
        }
    }
}