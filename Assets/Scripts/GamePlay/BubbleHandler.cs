using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task {
    public class BubbleHandler : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public BubbleColorType ColorType { get; private set; }

        public Rigidbody2D rigidBody;
        // [SerializeField] CircleCollider2D collider;

        private bool canDrop = false;
        public bool canCollide = false;

        [SerializeField] SpriteRenderer spriteRenderer;
        float timer = 0;

        private void FixedUpdate()
        {
            MaintainRotation();

            if (canDrop)
            {
                timer += Time.deltaTime;
                if (timer > 2)
                {
                    Destroy(gameObject);
                }
                transform.Translate(Vector3.down * Time.deltaTime * 10);
            }

        }

        public void SetCoordinates(int x,int y) {
            X = x;
            Y = y;
        }

        public void SetColor(BubbleColorType colorType) {
            ColorType = colorType;
            spriteRenderer.sprite = Singleton.Instance.UI_Elements.GetColoredSprite(colorType);
        }

        public SpriteRenderer GetSpriteRenderer() {
            return spriteRenderer;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canCollide)
            {
                
                if (collision.gameObject.CompareTag("bubble") || collision.gameObject.CompareTag("topCollider"))
                {
                    canCollide = false;
                    rigidBody.bodyType = RigidbodyType2D.Kinematic;
                    rigidBody.velocity = Vector3.zero;
                    MainEvents.OnBubbleCollision.Dispatch(this);
                }
            }
        }

        public void Shoot()
        {
            if (rigidBody)
            {
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                rigidBody.velocity = transform.up * 25;
            }
           
            MainEvents.OnBubbleShoot.Dispatch();
        }

        protected void MaintainRotation()
        {
            Vector3 worldDirectionToPointForward = rigidBody.velocity.normalized;
            Vector3 localDirectionToPointForward = Vector3.up;

            Vector3 currentWorldForwardDirection = transform.TransformDirection(
                    localDirectionToPointForward);
            float angleDiff = Vector3.SignedAngle(currentWorldForwardDirection,
                    worldDirectionToPointForward, Vector3.forward);

            transform.Rotate(Vector3.forward, angleDiff, Space.World);
        }

        public void Drop()
        {
            this.gameObject.layer = 0;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            canDrop = true;
        }
    }
}
