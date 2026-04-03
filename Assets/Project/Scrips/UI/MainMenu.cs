using UnityEngine;

namespace Survivor
{

    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject Option;
        [SerializeField]
        private GameObject Inventory;
        [SerializeField]
        private GameObject Market;
        [SerializeField]
        private GameObject Dungen;


        public void OnOption()
        {
            Option.SetActive(true);
            Time.timeScale= 0f;
        }
        public void OffOption()
        {
            Option.SetActive(false);
            Time.timeScale = 1f;

        }
        public void OnInventory()
        {
           Inventory.SetActive(true);
        }
        public void OffInventory()
        {
            Inventory.SetActive(false);
        }
        public void OnMarket()
        {
            Market.SetActive(true);
        }
        public void OffMarket()
        {
            Market.SetActive(false);
        }
        public void OnDungen()
        {
            Dungen.SetActive(true);
        }
         public void OffDungen()
        {
            Dungen.SetActive(false);

        }
        public void BackMainMenu()
        {
            SceneFader.Instance.FadeTo("MainMenu");
        }
        public void StartMenu()
        {
            SceneFader.Instance.FadeTo("Start");
        }
        public void Nomal()
        {
            SceneFader.Instance.FadeTo("Normal");
            if (Option != null)
            {
                Option.SetActive(false);
            }
        }
        public void Hard()
        {
            SceneFader.Instance.FadeTo("Hard");
            if (Option != null)
            {
                Option.SetActive(false);
            }
        }
    }
}