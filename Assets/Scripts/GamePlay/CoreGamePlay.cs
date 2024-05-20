using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MegaCat_Task
{
    public enum BubbleColorType
    {
        None, Red, Blue, Green, Yellow, White, Brown
    }
    public class CoreGamePlay : MonoBehaviour
    {
        [SerializeField] BubbleHandler bubblePrefab;
       
        [SerializeField] Transform bubbleParent;

        public List<BubbleHandler> similarBubbles = new List<BubbleHandler>();

        private void OnEnable()
        {
            MainEvents.OnGenerateGridBubbles.AddListener(OnGenerateGridBubbles);
            MainEvents.OnBubbleCollision.AddListener(OnBubbleCollision);
        }

        private void OnDisable()
        {
            MainEvents.OnGenerateGridBubbles.RemoveListener(OnGenerateGridBubbles);
            MainEvents.OnBubbleCollision.RemoveListener(OnBubbleCollision);
        }

        private void OnBubbleCollision(BubbleHandler bubbleCollided)
        {
            AttachCollidedBubbleToGrid(bubbleCollided);           
        }

        private GridItem GetNearestGridItem(Vector3 pos) {
            GridItem item = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < Singleton.Instance.Grid_Generator.MainGrid.GetLength(0); i++) {
                for (int j = 0; j < Singleton.Instance.Grid_Generator.MainGrid.GetLength(1); j++)
                {
                    float distance = Vector3.Distance(Singleton.Instance.Grid_Generator.MainGrid[i, j].gridPosition, pos);
                    if (distance < minDistance && Singleton.Instance.Grid_Generator.MainGrid[i, j].state == GridItemState.Free
                        && distance < 0.75f)
                    {
                        minDistance = distance;

                        item = Singleton.Instance.Grid_Generator.MainGrid[i, j];
                    }
                }
            }
            return item;
        }
        private void AttachCollidedBubbleToGrid(BubbleHandler bubbleCollided)
        {
            GridItem item = GetNearestGridItem(bubbleCollided.transform.position);
            if (item)
            {
                SetGridBubbleProperties(bubbleCollided, item);
                CheckForCombination(item);
            }

            if (bubbleCollided.transform.position.y < Singleton.Instance.Grid_Generator.BottomGridYPos)
            {
              //  bubbleCollided.GetSpriteRenderer().color = Color.black;
                MainEvents.OnGameOver.Dispatch();
            }
        }

        private void CheckForCombination(GridItem item)
        {
            bool[,] visited = new bool[10, 10];
            BubbleColorType colorType = item.bubbleAttached.ColorType;
            int x = item.X;
            int y = item.Y;
            DepthFirstSearchToFindSimilarColors(x, y, Singleton.Instance.Grid_Generator.MainGrid, visited, colorType);
            Invoke(nameof(RemoveBubbles), 0.1f);
        }

        public void RemoveBubbles() {
            if (similarBubbles.Count > 2)
            {
                for (int i = 0; i < similarBubbles.Count; i++)
                {
                    Singleton.Instance.Grid_Generator.MainGrid[similarBubbles[i].X, similarBubbles[i].Y].state = GridItemState.Free;
                    Singleton.Instance.Grid_Generator.MainGrid[similarBubbles[i].X, similarBubbles[i].Y].bubbleAttached = null;
                    Destroy(similarBubbles[i].gameObject);
                }

                MainEvents.OnScoreUpdate.Dispatch(similarBubbles.Count);
            }
            similarBubbles.Clear();

            Invoke(nameof(DetachBubblesNotConnectedToTop), 0.1f);
        }

        private void DetachBubblesNotConnectedToTop()
        {
            bool[,] visited = new bool[10, 10];
            bool[,] connected = new bool[10, 10];

            for (int i = 0; i < 10; i++)
            {
                DepthFirstSearchForDetachedBubbles(i, 0, Singleton.Instance.Grid_Generator.MainGrid, visited, connected);
            }

            int fallenbubbleCount = 0;
            for (int i = 0; i < connected.GetLength(0); i++)
            {
                for (int j = 0; j < connected.GetLength(1); j++)
                {
                    if (!connected[i, j] && Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached)
                    {
                        Singleton.Instance.Grid_Generator.MainGrid[i, j].state = GridItemState.Free;
                        Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached.Drop();
                        Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached = null;
                        fallenbubbleCount++;
                    }
                    
                }
            }

            MainEvents.OnScoreUpdate.Dispatch(fallenbubbleCount);
            CheckForBubbleCountInGrid();

        }

        private void CheckForBubbleCountInGrid()
        {
            int bubbleCount = 0;
            for (int i = 0; i < Singleton.Instance.Grid_Generator.MainGrid.GetLength(0); i++)
            {
                for (int j = 0; j < Singleton.Instance.Grid_Generator.MainGrid.GetLength(1); j++) {
                    if (Singleton.Instance.Grid_Generator.MainGrid[i,j].bubbleAttached)
                    {
                        bubbleCount++;
                    }
                }
            }
            if (bubbleCount == 0) {
                MainEvents.OnGridEmpty.Dispatch();
            }           
        }

        private void DepthFirstSearchForDetachedBubbles(int x, int y, GridItem[,] gridItem, bool[,] visited, bool[,] connected)
        {
            if (x >= 0 && x < 10 && y >= 0 && y < 10 && !visited[x, y])
            {
                if ((!connected[x, y]) && gridItem[x, y].bubbleAttached)
                {
                    connected[x, y] = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            bool rootNode = j == 0 && i == 0;
                            bool evenRow = y % 2 == 0 && ((i == 1 & j == 1) || (i == 1 && j == -1));
                            bool oddRow = y % 2 != 0 && ((i == -1 & j == -1) || (i == -1 && j == 1));

                            if (rootNode || evenRow || oddRow)
                                continue;

                            DepthFirstSearchForDetachedBubbles(x + i, y + j, gridItem, visited, connected);
                        }
                    }
                }
               
                visited[x, y] = true;
            }
        }

        private void DepthFirstSearchToFindSimilarColors(int x,int y,GridItem[,] gridItem, bool[,] visited, BubbleColorType colorType) {
            
            if (x >= 0 && x < 10 && y >= 0 && y < 10 && !visited[x, y] && gridItem[x, y].bubbleAttached)
            {
                BubbleColorType color = gridItem[x, y].bubbleAttached.ColorType;

                if (color == colorType)
                {
                    if (!similarBubbles.Contains(gridItem[x, y].bubbleAttached))
                    {
                        similarBubbles.Add(gridItem[x, y].bubbleAttached);

                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                bool rootNode = j == 0 && i == 0;
                                bool evenRow = y % 2 == 0 && ((i == 1 & j == 1) || (i == 1 && j == -1));
                                bool oddRow = y % 2 != 0 && ((i == -1 & j == -1) || (i == -1 && j == 1));

                                if (rootNode || evenRow || oddRow)
                                    continue;

                                DepthFirstSearchToFindSimilarColors(x + i, y + j, gridItem, visited, colorType);
                            }
                        }
                       
                    }
                }

                visited[x, y] = true;
            }
           
        }

        private void OnGenerateGridBubbles()
        {
            for (int i = 0; i < Singleton.Instance.Grid_Generator.MainGrid.GetLength(0); i++)
            {
                for (int j = 0; j < Singleton.Instance.Grid_Generator.MainGrid.GetLength(1); j++)
                {
                    if (Singleton.Instance.Grid_Generator.MainGrid[i, j].state == GridItemState.Free
                        && Singleton.Instance.Level_Manager.CanSpawnBubble(Singleton.Instance.Grid_Generator.MainGrid[i, j]))
                    {
                        BubbleHandler bubble = Instantiate(bubblePrefab);
                        BubbleColorType colorType = Singleton.Instance.Level_Manager.GetBubbleColorType(Singleton.Instance.Grid_Generator.MainGrid[i, j]);
                        bubble.SetColor(colorType);
                        SetGridBubbleProperties(bubble, Singleton.Instance.Grid_Generator.MainGrid[i, j]);
                    }
                }
            }
        }

        private void SetGridBubbleProperties(BubbleHandler bubble, GridItem item)
        {
            bubble.transform.position = item.gridPosition;      
            bubble.SetCoordinates(item.X, item.Y);
            bubble.rigidBody.bodyType = RigidbodyType2D.Kinematic;
            bubble.canCollide = false;
            bubble.transform.SetParent(bubbleParent);

            item.SetAttachedBubble(bubble);
            item.state = GridItemState.Occupied;


        }
    }
}