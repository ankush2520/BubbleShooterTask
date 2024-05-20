using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    [System.Serializable]
    public class LevelItem
    {
        public bool canSpawn;
        public BubbleColorType colorType;
    }

    [System.Serializable]
    public class LevelRow {
        public List<LevelItem> levelRow = new List<LevelItem>(10);
    }

    [System.Serializable]
    public class LevelData {
       
        public List<LevelRow> levelColumn = new List<LevelRow>(10);
    }
    public class LevelManager : MonoBehaviour
    {
        public List<LevelData> levels = new List<LevelData>();
        private int levelCount = 0;

        private void OnEnable()
        {
            MainEvents.OnGridEmpty.AddListener(OnGridEmpty);
        }

        private void OnDisable()
        {
            MainEvents.OnGridEmpty.RemoveListener(OnGridEmpty);
        }

        private void OnGridEmpty()
        {
            Invoke(nameof(UpdateLevel), 1f);
        }

        private void UpdateLevel()
        {
            levelCount++;
            if (levelCount>= levels.Count)
            {
                levelCount = 0;
            }
            MainEvents.OnLevelUpdated.Dispatch(levelCount);
        }

        public bool CanSpawnBubble(GridItem gridItem)
        { 
            return levels[levelCount].levelColumn[gridItem.Y].levelRow[gridItem.X].canSpawn;         
        }

        public BubbleColorType GetBubbleColorType(GridItem gridItem) {
            return levels[levelCount].levelColumn[gridItem.Y].levelRow[gridItem.X].colorType;
        }
    }
}
