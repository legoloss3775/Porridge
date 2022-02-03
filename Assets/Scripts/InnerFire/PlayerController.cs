using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnerFire {
    public enum PlayerState {
        InputMovement,
    }
    public class PlayerController : MonoBehaviour {
        public float walkSpeed;
        public PlayerState state;
        private Rigidbody2D playerRb;
        private BoxCollider2D playerCol;

        public void Awake() {
            playerRb = GetComponent<Rigidbody2D>();
            playerCol = GetComponent<BoxCollider2D>();
        }
        private void Update() {
            Movement();

            UpdatePlayerState();

            UpdatePlayerGravity();
        }
        private void UpdatePlayerState() {
            if (GetMovementDirection() != Vector2.zero) {
                state = PlayerState.InputMovement;
            }
        }
        private void UpdatePlayerGravity() {
            switch (state) {
                case PlayerState.InputMovement:
                    playerRb.gravityScale = 0;
                    playerRb.freezeRotation = true;
                    break;
            }
        }
        private void Movement() => playerRb.velocity = GetMovementDirection() * walkSpeed;
        private Vector2 GetMovementDirection() {
            Vector2 direction = Vector2.zero;

            direction.x += Input.GetAxis("Horizontal");
            direction.y += Input.GetAxis("Vertical");
            return direction;
        }
    }
}
