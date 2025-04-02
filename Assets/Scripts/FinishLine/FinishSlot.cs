using Enums;
using UnityEngine;

namespace FinishLine
{
    public class FinishSlot : MonoBehaviour
    {
        public bool IsFilled { get; private set; } = false;

        private Renderer _renderer;

        [SerializeField] private Color sunTeamColor = Color.yellow;
        [SerializeField] private Color moonTeamColor = Color.blue;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void Fill(TeamType teamType)
        {
            if (IsFilled) return;

            Color chosenColor = teamType == TeamType.Sun ? sunTeamColor : moonTeamColor;
            _renderer.material.color = chosenColor;
            IsFilled = true;
        }
    }
}