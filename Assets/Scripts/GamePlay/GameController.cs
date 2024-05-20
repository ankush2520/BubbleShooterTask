using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

namespace MegaCat_Task
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] GameObject topCollider;
        [SerializeField] TextMeshProUGUI scoreText, levelText;
        [SerializeField] GameObject gameOverPanel;
        
        private int score;

        private Vector3 topColliderInitialPos;
        private int bubbleShootCount;
        private int multiplier = 1;

        private void Start()
        {
            levelText.text = "Level : " + (1);
            topColliderInitialPos = topCollider.transform.position;
        }

        private void OnEnable()
        {
            MainEvents.OnScoreUpdate.AddListener(OnScoreUpdate);
            MainEvents.OnBubbleShoot.AddListener(OnBubbleShoot);
            MainEvents.OnGameOver.AddListener(OnGameOver);
            MainEvents.OnLevelUpdated.AddListener(OnLevelUpdated);
            MainEvents.OnGridEmpty.AddListener(OnGridEmpty);
        }

        private void OnDisable()
        {
            MainEvents.OnScoreUpdate.RemoveListener(OnScoreUpdate);
            MainEvents.OnBubbleShoot.RemoveListener(OnBubbleShoot);
            MainEvents.OnGameOver.RemoveListener(OnGameOver);
            MainEvents.OnLevelUpdated.RemoveListener(OnLevelUpdated);
            MainEvents.OnGridEmpty.RemoveListener(OnGridEmpty);
        }

        private void OnGridEmpty()
        {
            levelText.text = "Level Done";
        }

        private void OnLevelUpdated(int levelCount)
        {
            //  UiPanel.SetActive(true);
            levelText.text = "Level : " + (levelCount + 1);
            bubbleShootCount = 0;
            topCollider.transform.position = topColliderInitialPos;
        }

        private void OnGameOver()
        {
            topCollider.transform.position = topColliderInitialPos;
            gameOverPanel.SetActive(true);
            scoreText.text = "Game Over";
        }

        public void Reload() {
            SceneManager.LoadScene(0);
        }

        private void OnBubbleShoot()
        {
            bubbleShootCount++;
            if (bubbleShootCount == 12)
            {
                bubbleShootCount = 0;

                topCollider.transform.position -= new Vector3(0, 0.75f, 0);
                MainEvents.OnMoveGridDown.Dispatch();
            }
        }
    
        private void OnScoreUpdate(int _score)
        {
            score += (_score * multiplier);
            scoreText.text = "Score : " + score;
        }
    }
}
