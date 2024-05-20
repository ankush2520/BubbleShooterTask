using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task {
    public class Shooter : MonoBehaviour
    {
        [SerializeField] BubbleHandler bubblePrefab;

        private BubbleHandler shootingBubble;
        private BubbleHandler nextShootingBubble;

        private bool canShootBubble = true;
        private bool gamePause = false;

        [SerializeField] Transform shootPosition;
        [SerializeField] Transform nextShootPosition;

        [SerializeField] Rotator rotator;

        private void OnEnable()
        {
            MainEvents.OnBackgroundButtonClicked.AddListener(OnBackgroundButtonClicked);
            MainEvents.OnBubbleCollision.AddListener(OnBubbleCollision);
            MainEvents.OnGenerateGridBubbles.AddListener(OnGenerateGridBubbles);
            MainEvents.OnGridEmpty.AddListener(OnGridEmpty);
            MainEvents.OnLevelUpdated.AddListener(OnLevelUpdated);
        }

        private void OnDisable()
        {
            MainEvents.OnBackgroundButtonClicked.RemoveListener(OnBackgroundButtonClicked);
            MainEvents.OnBubbleCollision.RemoveListener(OnBubbleCollision);
            MainEvents.OnGenerateGridBubbles.RemoveListener(OnGenerateGridBubbles);
            MainEvents.OnGridEmpty.RemoveListener(OnGridEmpty);

            MainEvents.OnLevelUpdated.RemoveListener(OnLevelUpdated);

        }

        private void OnLevelUpdated(int obj)
        {
            gamePause = false;
            canShootBubble = true;
        }

        private void OnGridEmpty()
        {
            Destroy(nextShootingBubble.gameObject);
            Destroy(shootingBubble.gameObject);
            gamePause = true;
        }

        private void OnGenerateGridBubbles()
        {
            Invoke(nameof(GenerateShootingBubble), 0.01f);
        }

        private void OnBubbleCollision(BubbleHandler bubble)
        {
            if (!gamePause)
            {
                GenerateShootingBubble();
                Invoke(nameof(EnableBubbleShooting), 0.2f);
            }
        }

        private void OnBackgroundButtonClicked()
        {
            if (canShootBubble)
            {
                shootingBubble.Shoot();
                canShootBubble = false;
            }
        }

        private void EnableBubbleShooting()
        {
            if (!gamePause)
                canShootBubble = true;
        }

        private void GenerateShootingBubble()
        {
            if (nextShootingBubble)
            {
                shootingBubble = nextShootingBubble;
            }
            else
            {
                BubbleColorType colorType = GetColorType();
                if (colorType == BubbleColorType.None)
                {
                    return;
                }
                shootingBubble = Instantiate(bubblePrefab);
                shootingBubble.SetColor(colorType);
            }

            shootingBubble.transform.position = shootPosition.position;
            shootingBubble.transform.rotation = rotator.transform.rotation;
            shootingBubble.canCollide = true;
            shootingBubble.transform.SetParent(shootPosition);

            GenerateNextShootingBubble();

        }

        private void GenerateNextShootingBubble() {
            BubbleColorType colorType = GetColorType();
            if (colorType==BubbleColorType.None)
            {
                return;
            }
            nextShootingBubble = Instantiate(bubblePrefab);
            nextShootingBubble.SetColor(colorType);
            nextShootingBubble.transform.position = nextShootPosition.position;
            nextShootingBubble.transform.rotation = rotator.transform.rotation;
            nextShootingBubble.canCollide = true;
            nextShootingBubble.transform.SetParent(nextShootPosition);
        }

        private BubbleColorType GetColorType()
        {
            BubbleColorType colorType = BubbleColorType.None;

            List<BubbleColorType> availableColorsInGrid = new List<BubbleColorType>();

            for (int i = 0; i < Singleton.Instance.Grid_Generator.MainGrid.GetLength(0); i++)
            {
                for (int j = 0; j < Singleton.Instance.Grid_Generator.MainGrid.GetLength(1); j++)
                {
                    if (Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached)
                    {
                        if (!availableColorsInGrid.Contains(Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached.ColorType))
                        {
                            availableColorsInGrid.Add(Singleton.Instance.Grid_Generator.MainGrid[i, j].bubbleAttached.ColorType);
                        }
                    }
                }
            }

            int randomIndex = UnityEngine.Random.Range(0, availableColorsInGrid.Count);

            if (availableColorsInGrid.Count > 0)
                colorType = availableColorsInGrid[randomIndex];
            
            return colorType;
        }

        private void OnGameOver() {

            gamePause = true;
        }
    }
}
