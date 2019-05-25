using UnityEngine;


namespace AIBehavior
{
    public class AISkillState : MonoBehaviour
    {
        public new string name = "Untitled";
        public long Id = 0;
        public string Icon = "";
        public string Desc = "";
        public bool foldoutOpen = false;
        public float currentCoolDown = 0;

        private void Update()
        {
            if (currentCoolDown > 0)
            {
                currentCoolDown -= Time.deltaTime;
            }
        }
    }
}