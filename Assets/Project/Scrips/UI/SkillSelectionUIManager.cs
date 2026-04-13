using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Survivor
{
    public class SkillSelectionUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private List<Button> skillChoiceButtons = new List<Button>();
        [SerializeField] private GameObject skillPanel;

        [Header("Skill Pool")]
        [SerializeField] private List<SkillData> allSkillDatabase = new List<SkillData>();

        [Header("Player Reference")]
        public GameObject playerObject;

        private List<SkillData> currentOptions = new List<SkillData>();
        private static bool isFirstSelectionDone = false;
        private PlayerStats playerStats;

        private Dictionary<string, string> skillNameMap = new Dictionary<string, string>() {
            { "독 안개", "MlasmaSkill" }, { "화염구", "FireballSkill" }, { "얼음 화살", "FrostBoltSkill" },
            { "벼락", "VoltCrashSkill" }, { "회전하는 칼날", "SpinningBladeSkill" },
            { "자석", "MagnetSkill" }, { "헤르메스의 신발", "HermesShoesSkill" }
        };

        private void Awake()
        {
            isFirstSelectionDone = false;
            foreach (var skill in allSkillDatabase) { if (skill != null) skill.level = 0; }
            RefreshPlayerReference();
        }

        public void ShowRandomSkillSelection()
        {
            if (skillPanel != null) skillPanel.SetActive(true);
            Time.timeScale = 0f;
            RefreshPlayerReference();

            List<SkillData> evolvableSkills = allSkillDatabase
                .Where(s => s != null && s.type == SkillType.Evolution && s.level == 0 && IsEvolutionReady(s))
                .ToList();

            var normalPool = allSkillDatabase.Where(s => s != null && s.type != SkillType.Evolution && s.level < 4).ToList();
            List<SkillData> filteredPool = !isFirstSelectionDone ? normalPool.Where(s => s.type == SkillType.Active).ToList() : normalPool.Concat(evolvableSkills).ToList();

            int count = Mathf.Min(skillChoiceButtons.Count, filteredPool.Count);
            currentOptions = filteredPool.OrderBy(x => Random.value).Take(count).ToList();
            UpdateUIAndListeners();
        }

        private bool IsEvolutionReady(SkillData evo)
        {
            var active = allSkillDatabase.Find(s => s.id == evo.requiredActiveId);
            var passive = allSkillDatabase.Find(s => s.id == evo.requiredPassiveId);
            return (active != null && active.level >= 4) && (passive != null && passive.level > 0);
        }

        private void UpdateUIAndListeners()
        {
            foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false);
            for (int i = 0; i < currentOptions.Count; i++)
            {
                SkillData data = currentOptions[i];
                skillChoiceButtons[i].gameObject.SetActive(true);
                var iconImage = skillChoiceButtons[i].GetComponentsInChildren<Image>(true).FirstOrDefault(x => x.gameObject.name == "Icon");
                if (iconImage != null) iconImage.sprite = data.skillIcon;
                skillChoiceButtons[i].onClick.RemoveAllListeners();
                skillChoiceButtons[i].onClick.AddListener(() => OnSkillSelected(data));
            }
        }

        public void OnSkillSelected(SkillData selected)
        {
            if (playerObject == null) RefreshPlayerReference();

            if (selected.type == SkillType.Evolution)
            {
                var materialActive = allSkillDatabase.Find(s => s.id == selected.requiredActiveId);
                if (materialActive != null)
                {
                    string className = GetTargetClassName(materialActive.skillName);
                    var script = playerObject.GetComponent(className);

                    // 😤 여기서 직접 데이터를 주입하고 교체 함수를 때려버립니다.
                    if (script is MlasmaSkill mlasma) { mlasma.skillData = selected; mlasma.ForceEvolveMlasma(); }
                    else if (script is SpinningBladeSkill blade) { blade.skillData = selected; blade.ForceEvolveBlade(); }
                }
                selected.level = 1;
            }
            else
            {
                selected.level++;
                if (selected.type == SkillType.Active) ActivateSkillScript(selected.skillName);
                else if (selected.type == SkillType.Passive && playerStats != null) playerStats.UpgradeStat(selected.id);
            }

            if (SkillInventoryUI.Instance != null) SkillInventoryUI.Instance.RefreshUI(selected);
            isFirstSelectionDone = true;
            ResumeGame();
        }

        private string GetTargetClassName(string displayName)
        {
            if (!skillNameMap.TryGetValue(displayName.Trim(), out string targetClassName))
                targetClassName = displayName.Trim().Replace(" ", "");
            return targetClassName;
        }

        private void ActivateSkillScript(string displayName)
        {
            string targetClassName = GetTargetClassName(displayName);
            RefreshPlayerReference();
            var scripts = playerObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var script in scripts)
            {
                if (script != null && script.GetType().Name == targetClassName) { script.enabled = true; break; }
            }
        }

        public void RefreshPlayerReference()
        {
            if (playerObject == null) playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) playerStats = playerObject.GetComponent<PlayerStats>();
        }

        private void SetupButtons() { foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false); }
        private void ResumeGame() { if (skillPanel != null) skillPanel.SetActive(false); Time.timeScale = 1f; }
    }
}