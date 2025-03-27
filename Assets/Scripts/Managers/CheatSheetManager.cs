using UnityEngine;

namespace Managers
{
    public class CheatSheetManager : MonoSingleton<CheatSheetManager>
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameManager.Instance.ResetPlayerPosition();
                print("Reset Player Position");

            }
           
          
        }
    }
}
