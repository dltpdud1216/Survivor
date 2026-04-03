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

        [Header("Player Skills Reference")]
        [SerializeField] private List<MonoBehaviour> playerActiveSkills = new List<MonoBehaviour>();

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

        void Awake()
        {
            // 스킬 데이터 레벨 초기화
            foreach (var skill in allSkillDatabase) skill.level = 0;

            // 버튼 리스너 등록 (람다식 클로저 방지를 위해 index 변수 따로 선언)
            for (int i = 0; i < skillChoiceButtons.Count; i++)
            {
                int index = i;
                skillChoiceButtons[i].onClick.RemoveAllListeners();
                skillChoiceButtons[i].onClick.AddListener(() => OnSkillSelected(index));
            }
        }

        void Start()
        {
            // 씬 시작하자마자 첫 스킬 선택창 띄우기
            ShowRandomSkillSelection();
        }

        public void ShowRandomSkillSelection()
        {
            if (skillPanel != null) skillPanel.SetActive(true);

            Time.timeScale = 0f; // 게임 일시정지
            currentOptions.Clear();

            List<SkillData> filteredPool = new List<SkillData>();
            if (isFirstSelect)
            {
                filteredPool = allSkillDatabase.Where(s => s.type == SkillType.Active).ToList();
            }
            else
            {
                filteredPool = allSkillDatabase.Where(s => s.type == SkillType.Active || s.type == SkillType.Passive).ToList();
                var evoPool = allSkillDatabase.Where(s => s.type == SkillType.Evolution).ToList();
                foreach (var evo in evoPool) if (CanEvolve(evo)) filteredPool.Add(evo);
            }

            filteredPool = filteredPool.Where(s => s.level < 8).ToList();

            if (filteredPool.Count == 0)
            {
                ResumeGame();
                return;
            }

            currentOptions = filteredPool.OrderBy(x => Random.value).Take(skillChoiceButtons.Count).ToList();
            UpdateUI();
        }

        private bool CanEvolve(SkillData evo)
        {
            bool activeReady = allSkillDatabase.Any(s => s.id == evo.requiredActiveId && s.level >= 8);
            bool passiveReady = allSkillDatabase.Any(s => s.id == evo.requiredPassiveId && s.level >= 1);
            return activeReady && passiveReady;
        }

        private void UpdateUI()
        {
            foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false);

            for (int i = 0; i < currentOptions.Count; i++)
            {
                skillChoiceButtons[i].gameObject.SetActive(true);

                // 아이콘 설정
                var iconTransform = skillChoiceButtons[i].transform.Find("Icon");
                if (iconTransform != null) iconTransform.GetComponent<Image>().sprite = currentOptions[i].skillIcon;

                // 텍스트 설정 (Lv 표시)
                var textTransform = skillChoiceButtons[i].transform.Find("Text (TMP)");
                if (textTransform != null)
                {
                    string levelText = currentOptions[i].level > 0 ? $" Lv.{currentOptions[i].level + 1}" : " New!";
                    textTransform.GetComponent<TextMeshProUGUI>().text = currentOptions[i].skillName + levelText;
                }
            }
        }

        public void OnSkillSelected(int index)
        {
            Debug.Log($"버튼 클릭됨! 선택한 인덱스: {index}");

            if (index < 0 || index >= currentOptions.Count) return;

            // 1. 선택한 스킬 데이터 업데이트
            SkillData selected = currentOptions[index];
            selected.level++;

            // 2. 실제 스킬 스크립트 활성화 (에러 방지를 위해 try-catch)
            try
            {
                ActivateSkillScript(selected.skillName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"스킬 활성화 중 에러 발생: {e.Message}");
            }

            if (isFirstSelect) isFirstSelect = false;

            // 3. 무조건 게임 재개 😤
            ResumeGame();
        }

        private void ActivateSkillScript(string displayName)
        {
            string searchKey = displayName.Trim();
            string englishName = skillNameMap.ContainsKey(searchKey) ? skillNameMap[searchKey] : searchKey.Replace(" ", "").ToLower();

            bool found = false;
            foreach (var skillScript in playerActiveSkills)
            {
                if (skillScript != null && skillScript.GetType().Name.ToLower().Contains(englishName))
                {
                    skillScript.enabled = true;
                    found = true;
                    break;
                }
            }

            if (!found) Debug.LogWarning($"{displayName}에 해당하는 스크립트를 playerActiveSkills에서 찾지 못했습니다.");
        }

        private void ResumeGame()
        {
            Debug.Log("ResumeGame 호출됨: 패널을 끄고 시간을 다시 흐르게 합니다.");

            if (skillPanel != null) skillPanel.SetActive(false);

            // 일시정지 해제 😤
            Time.timeScale = 1f;
        }
    }
}