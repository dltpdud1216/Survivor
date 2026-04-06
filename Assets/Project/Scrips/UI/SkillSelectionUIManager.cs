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
        [SerializeField] private GameObject playerObject;

        private List<MonoBehaviour> playerActiveSkills = new List<MonoBehaviour>();
        private List<SkillData> currentOptions = new List<SkillData>();
        private bool isFirstSelect = true;

        private Dictionary<string, string> skillNameMap = new Dictionary<string, string>() {
            { "독 안개", "mlasma" }, { "화염구", "fireball" }, { "얼음 화살", "frostbolt" },
            { "벼락", "voltcrash" }, { "회전하는 칼날", "spinningblade" },
            { "신성한 심판", "divinejudgement" }, { "영원한 지옥불", "eternalinferno" },
            { "빙하의 별", "glacialstar" }, { "궤도 톱날", "orbitingsaw" }, { "죽음의 발걸음", "toxictrail" },
            { "거인의 장갑", "giantgauntlets" }, { "헤르메스의 신발", "hermesshoes" },
            { "행운의 부적", "luckycharm" }, { "마법 시계", "magichourglass" },
            { "자석", "magnet" }, { "힘의 원석", "powercrystal" }
        };

        void Start()
        {
            RefreshPlayerReference();
            SetupButtons();
            ShowRandomSkillSelection();
        }

        private void SetupButtons()
        {
            for (int i = 0; i < skillChoiceButtons.Count; i++)
            {
                int index = i;
                if (skillChoiceButtons[i] == null) continue;

                skillChoiceButtons[i].onClick.RemoveAllListeners();
                skillChoiceButtons[i].onClick.AddListener(() => {
                    OnSkillSelected(index);
                });
            }
        }

        public void RefreshPlayerReference()
        {
            if (playerObject == null) playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                playerActiveSkills = playerObject.GetComponentsInChildren<MonoBehaviour>(true).ToList();
            }
        }

        public void ShowRandomSkillSelection()
        {
            if (skillPanel != null) skillPanel.SetActive(true);
            Time.timeScale = 0f;

            // 1. 기본 필터링 (널 체크 및 만렙 제외)
            var filteredPool = allSkillDatabase.Where(s => s != null && s.level < 8).ToList();

            // 2. 첫 선택 시 액티브 스킬 강제 필터링 (얼음 화살 타입 확인 필수! 😤)
            if (isFirstSelect)
            {
                var activeOnly = filteredPool.Where(s => s.type == SkillType.Active).ToList();

                // 만약 액티브 필터링 후 개수가 너무 적으면 전체 풀 사용 (안전장치)
                if (activeOnly.Count >= skillChoiceButtons.Count)
                {
                    filteredPool = activeOnly;
                }
            }

            // 3. 랜덤 셔플 및 추출
            currentOptions = filteredPool.OrderBy(x => Random.value).Take(skillChoiceButtons.Count).ToList();

            // 4. 로그로 결과 확인 (얼음 화살이 포함되었는지 콘솔에서 바로 확인 가능)
            foreach (var opt in currentOptions) Debug.Log($"🎲 선택지에 등장: {opt.skillName}");

            UpdateUI();
        }

        public void OnSkillSelected(int index)
        {
            try
            {
                if (index >= 0 && index < currentOptions.Count)
                {
                    SkillData selected = currentOptions[index];

                    selected.level++;
                    ActivateSkillScript(selected.skillName);
                }
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                if (isFirstSelect) isFirstSelect = false;
                ResumeGame();
            }
        }

        private void ActivateSkillScript(string displayName)
        {
            string searchKey = displayName.Trim();
            string englishKeyword = skillNameMap.ContainsKey(searchKey) ? skillNameMap[searchKey] : searchKey.Replace(" ", "").ToLower();

            foreach (var script in playerActiveSkills)
            {
                if (script == null) continue;
                string className = script.GetType().Name.ToLower();

                if (className.Contains(englishKeyword.ToLower()))
                {
                    script.enabled = true;
                    return;
                }
            }
        }

        private void ResumeGame()
        {
            if (skillPanel != null) skillPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        private void UpdateUI()
        {
            // 모든 버튼 일단 비활성화
            foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false);

            // 옵션 개수만큼 버튼 활성화 및 데이터 매핑
            for (int i = 0; i < currentOptions.Count; i++)
            {
                if (i >= skillChoiceButtons.Count) break;

                skillChoiceButtons[i].gameObject.SetActive(true);

                var txt = skillChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = currentOptions[i].skillName;

                var icon = skillChoiceButtons[i].GetComponentsInChildren<Image>(true).FirstOrDefault(x => x.gameObject.name == "Icon");
                if (icon != null) icon.sprite = currentOptions[i].skillIcon;
            }
        }
    }
}