using System;
using System.Collections.Generic;
using Enums;
using Player;
using UnityEngine;

namespace FinishLine
{
    public class SlotsArea : MonoBehaviour
    {
        [SerializeField] private List<FinishSlot> slots;

        /*private void OnEnable()
        {
            PlayerController.TeamGetPoint += FillNextSlot;
        }*/

        public void FillNextSlot(TeamType teamType)
        {
            Debug.Log(TryFillNextAvailableSlot(teamType) ? "Slot filled" : "Slot not filled");
        }

        /*private void OnDisable()
        {
            PlayerController.TeamGetPoint += FillNextSlot;
        }*/
        
        

        public bool TryFillNextAvailableSlot(TeamType teamType)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsFilled)
                {
                    slot.Fill(teamType);
                    return true;
                }
            }

            return false; // All slots filled
        }
    }
}