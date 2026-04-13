using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class CheatKeyManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SkillData> allSkillDatabase = new List<SkillData>();
        [SerializeField] private SkillSelectionUIManager uiManager;

        void Update()
        {
            // 1:독안개(0), 2:화염구(3), 3:얼음화살(2), 4:벼락(4), 5:칼날(1)
            if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyCheat(0, 107);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyCheat(3, 104);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyCheat(2, 106);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyCheat(4, 105);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ApplyCheat(1, 108);
        }

        private void ApplyCheat(int activeId, int passiveId)
        {
            if (uiManager == null) uiManager = FindObjectOfType<SkillSelectionUIManager>();

            var activeData = allSkillDatabase.Find(s => s.id == activeId);
            var passiveData = allSkillDatabase.Find(s => s.id == passiveId);

            if (activeData != null) { activeData.level = 4; uiManager.OnSkillSelected(activeData); }
            if (passiveData != null) { passiveData.level = 1; uiManager.OnSkillSelected(passiveData); }

            var evoData = allSkillDatabase.Find(s => s.type == SkillType.Evolution && s.requiredActiveId == activeId);
            if (evoData != null) uiManager.OnSkillSelected(evoData);
        }
    }
}