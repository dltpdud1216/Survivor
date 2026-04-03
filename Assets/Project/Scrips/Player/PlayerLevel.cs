using UnityEngine;
using Survivor; // 네임스페이스 잊지 마세요!

namespace Survivor
{
    

public class PlayerLevel : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToLevelUp = 100;

    // 아까 만든 UI 매니저를 연결해줍니다.
    [SerializeField] private SkillSelectionUIManager uiManager;

    public void GetExp(int amount)
    {
        currentExp += amount;

        if (currentExp >= expToLevelUp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp = 0; // 경험치 초기화
        expToLevelUp += 50; // 다음 레벨은 더 힘들게!

        // ★ 여기서 UI 매니저의 함수를 호출해서 창을 띄웁니다!
        uiManager.ShowRandomSkillSelection();
    }
}
}