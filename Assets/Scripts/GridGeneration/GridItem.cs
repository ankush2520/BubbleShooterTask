using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    public class GridItem : MonoBehaviour
    {
        public int X, Y;
        public Vector3 gridPosition;
        public GridItemState state;
        public BubbleHandler bubbleAttached;

        private void OnEnable()
        {
            MainEvents.OnMoveGridDown.AddListener(OnMoveGridDown);
        }

        private void OnDisable()
        {
            MainEvents.OnMoveGridDown.RemoveListener(OnMoveGridDown);
        }

        private void OnMoveGridDown()
        {
            gridPosition -= new Vector3(0, 0.75f, 0);
            transform.position -= new Vector3(0, 0.75f, 0);
            if (bubbleAttached)
                bubbleAttached.transform.position -= new Vector3(0, 0.75f, 0);
        }

        public void SetValues (int x,int y,Vector3 pos, GridItemState _state) {
            transform.position = pos;
            X = x;
            Y = y;
            state = _state;
            gridPosition = pos;
        }

        public void SetAttachedBubble(BubbleHandler bubble)
        {
            bubbleAttached = bubble;
            
        }

        // public
    }
}