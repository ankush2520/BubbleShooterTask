using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    public static class MainEvents
    {
        public static Event OnGenerateGridBubbles = new Event();
        public static Event OnBackgroundButtonClicked = new Event();
        public static Event<BubbleHandler> OnBubbleCollision = new Event<BubbleHandler>();
        public static Event OnGameOver = new Event();
        public static Event<int> OnScoreUpdate = new Event<int>();
        public static Event OnBubbleShoot = new Event();

        public static Event OnMoveGridDown = new Event();

        public static Event OnGridEmpty = new Event();
        public static Event<int> OnLevelUpdated = new Event<int>();
    }
}
