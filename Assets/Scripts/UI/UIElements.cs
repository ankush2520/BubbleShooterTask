using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task {
    public class UIElements : MonoBehaviour
    {

        [SerializeField] Sprite[] coloredSprites;

        public Sprite GetColoredSprite(BubbleColorType colorType) {
            int index = (int)colorType - 1;
            //if (index < 0) index = 0;
            return coloredSprites[index];
        }
    }
}
