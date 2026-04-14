using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class CheatKeyManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SkillData> allSkillDatabase = new List<SkillData>();
        [SerializeField] private SkillSelectionUIManager uiManager;

        [Header("Mobile UI")]
        [SerializeField] private GameObject cheatPanel; // 치트 버튼들을 담은 부모 패널

        void Update()
        {
            // PC용 키보드 치트 (테스트용으로 유지)
            if (Input.GetKeyDown(KeyCode.Alpha1)) Cheat_Mlasma();    // 독 안개
            if (Input.GetKeyDown(KeyCode.Alpha2)) Cheat_Fireball();  // 화염구
            if (Input.GetKeyDown(KeyCode.Alpha3)) Cheat_FrostBolt(); // 얼음 화살
            if (Input.GetKeyDown(KeyCode.Alpha4)) Cheat_VoltCrash(); // 벼락
            if (Input.GetKeyDown(KeyCode.Alpha5)) Cheat_Blade();     // 회전 칼날
        }

        // 😤 [모바일용 버튼 연결 함수들]
        public void Cheat_Mlasma() => ApplyCheat(0, 107);
        public void Cheat_Fireball() => ApplyCheat(3, 104);
        public void Cheat_FrostBolt() => ApplyCheat(2, 106);
        public void Cheat_VoltCrash() => ApplyCheat(4, 105);
        public void Cheat_Blade() => ApplyCheat(1, 108);

        // 치트 패널 토글 (화면 가릴 때 껐다 켜는 용도)
        public void ToggleCheatPanel()
        {
            if (cheatPanel != null)
                cheatPanel.SetActive(!cheatPanel.activeSelf);
        }

        private void ApplyCheat(int activeId, int passiveId)
        {
            if (uiManager == null) uiManager = FindObjectOfType<SkillSelectionUIManager>();
            if (uiManager == null) return;

            var activeData = allSkillDatabase.Find(s => s.id == activeId);
            var passiveData = allSkillDatabase.Find(s => s.id == passiveId);

            // 1. 재료 스킬 주입
            if (activeData != null) { activeData.level = 4; uiManager.OnSkillSelected(activeData); }
            if (passiveData != null) { passiveData.level = 1; uiManager.OnSkillSelected(passiveData); }

            // 2. 진화 데이터 주입 (여기서 자동으로 프리팹 교체 함수들이 실행됨)
            var evoData = allSkillDatabase.Find(s => s.type == SkillType.Evolution && s.requiredActiveId == activeId);
            if (evoData != null)
            {
                uiManager.OnSkillSelected(evoData);
                Debug.Log($"<color=yellow>📱 [모바일 치트] {evoData.skillName} 진화 완료!</color>");
            }
        }
    }
}