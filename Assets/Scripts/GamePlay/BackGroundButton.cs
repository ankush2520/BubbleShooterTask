using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MegaCat_Task
{
    public class BackGroundButton : MonoBehaviour
    {
        public void OnClickButton() {
            MainEvents.OnBackgroundButtonClicked.Dispatch();
        }
    }
}