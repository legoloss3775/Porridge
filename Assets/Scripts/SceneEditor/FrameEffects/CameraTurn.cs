using System.Collections;
using UnityEngine;

namespace FrameCore.FrameEffects {
    public class CameraTurn :EffectPrefab {
        public float degreesX { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().cameraTurnAnimationData.degreesX; else return 90f; } }
        public float degreesY { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().cameraTurnAnimationData.degreesY; else return 0f; } }
        // Vector3 finalRot { get { return new Vector3(degreesY, degreesX, Camera.main.transform.rotation.eulerAngles.z); } }
        public Vector3 rotation;

        void Start() {
            /**FrameController.AddAnimationToQueue(gameObject.name, true);
            StartCoroutine(TurnCamera(degreesX, degreesY, speed));**/
        }
        private void Update() {
            /**if(degreesX >= 0) {
                if(rotation.y >= degreesX) {
                    FrameController.RemoveAnimationFromQueue(gameObject.name);
                }
            }
            else {
                if (rotation.y <= degreesX) {
                    FrameController.RemoveAnimationFromQueue(gameObject.name);
                }
            }
            if(degreesY >= 0) {
                if (rotation.x >= degreesY) {
                    FrameController.RemoveAnimationFromQueue(gameObject.name);
                }
            }
            else {
                if (rotation.x <= degreesY) {
                    FrameController.RemoveAnimationFromQueue(gameObject.name);
                }
            }**/
            // Debug.Log(Camera.main.transform.rotation.eulerAngles.y);
        }
        public override void OnFrameKeyChanged() {

            var cameraTurn = GetComponent<FrameEffects.CameraTurn>();

            cameraTurn.rotation = Camera.main.transform.rotation.eulerAngles;
            FrameController.AddAnimationToQueue(cameraTurn.gameObject.name, true);
            cameraTurn.StartCoroutine(cameraTurn.TurnCamera(cameraTurn.degreesX, cameraTurn.degreesY, cameraTurn.speed));
        }
        public IEnumerator TurnCamera(float degreesX, float degreesY, float speed) {

            //this.rotation = Quaternion.Euler(rot);
            yield return new WaitForSeconds(animationDelay);
            if (degreesX > 0) {
                while (rotation.y < degreesX) {
                    rotation = Camera.main.transform.rotation.eulerAngles;
                    rotation += new Vector3(0, Time.deltaTime, 0) * speed;
                    Camera.main.transform.rotation = Quaternion.Euler(rotation);
                    yield return null;
                }
            }
            else {
                while (rotation.y > degreesX) {
                    //rotation = Camera.main.transform.rotation.eulerAngles;
                    rotation -= new Vector3(0, Time.deltaTime, 0) * speed;
                    Camera.main.transform.rotation = Quaternion.Euler(rotation);
                    yield return null;
                }
            }
            if (degreesY > 0) {
                while (rotation.x < degreesY) {
                    rotation = Camera.main.transform.rotation.eulerAngles;
                    rotation += new Vector3(Time.deltaTime, 0, 0) * speed;
                    Camera.main.transform.rotation = Quaternion.Euler(rotation);
                    yield return null;
                }
            }
            else {
                while (rotation.x > degreesY) {
                    rotation = Camera.main.transform.rotation.eulerAngles;
                    rotation -= new Vector3(Time.deltaTime, 0, 0) * speed;
                    Camera.main.transform.rotation = Quaternion.Euler(rotation);
                    yield return null;
                }
            }
            FrameController.RemoveAnimationFromQueue(gameObject.name);
        }
    }
}
