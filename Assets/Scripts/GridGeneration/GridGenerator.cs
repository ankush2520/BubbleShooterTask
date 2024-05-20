using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    public enum GridItemState
    {
        None, Free, Occupied, Prohibited
    }
    public class GridGenerator : MonoBehaviour
    {
       
        [SerializeField] GridItem itemPrefab;
        [SerializeField] Transform initialPoint;
        [SerializeField] Transform parent;
        private float gap = 0.75f;

        private int XCount = 10;
        private int YCount = 10;

        public float BottomGridYPos;

        public GridItem[,] MainGrid = new GridItem[10, 10];
        private void Start()
        {
            GenerateGrid();
        }

        private void OnEnable()
        {
            MainEvents.OnLevelUpdated.AddListener(OnLevelUpdated);
        }

        private void OnDisable()
        {
            MainEvents.OnLevelUpdated.RemoveListener(OnLevelUpdated);
        }

        private void OnLevelUpdated(int levelCount)
        {
            ClearGridData();
            GenerateGrid();
        }

        private void ClearGridData() {
            for (int i = 0; i < MainGrid.GetLength(0); i++)
            {
                for (int j = 0; j < MainGrid.GetLength(1); j++)
                {
                    if (MainGrid[i, j].bubbleAttached)
                    {
                        Destroy(MainGrid[i, j].bubbleAttached.gameObject);
                    }
                    Destroy(MainGrid[i, j].gameObject);
                }
            }
            MainGrid = new GridItem[10, 10];
        }

        private void GenerateGrid() {

            float YPos = initialPoint.position.y;
            for (int i = 0; i < YCount; i++)
            {
                int rowSize = XCount;
                float XPos = initialPoint.position.x;

                if (i % 2 != 0)
                {
                    XPos += gap / 2;
                }

                for (int j = 0; j < rowSize; j++)
                {
                    GridItem item = Instantiate(itemPrefab);
                    item.transform.SetParent(parent.transform);
                    Vector3 pos = new Vector3(XPos + (j * gap), YPos, 0);

                    GridItemState state = (i % 2 != 0 && j == rowSize - 1) ? GridItemState.Prohibited : GridItemState.Free;

                    item.SetValues(j, i, pos, state);

                    MainGrid[j, i] = item;
                }
                YPos -= gap;
            }
            BottomGridYPos = YPos + gap;
            MainEvents.OnGenerateGridBubbles.Dispatch();

        }
    }
}
