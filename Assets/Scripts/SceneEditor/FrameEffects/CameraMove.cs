using System.Collections;
using UnityEngine;

namespace FrameCore.FrameEffects {
    public class CameraMove : EffectPrefab {
        public Vector3 moveToPosition { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().cameraTurnAnimationData.moveTo; else return Vector3.zero; } }

        private void OnEnable() {

        }
        public override void OnFrameKeyChanged() {

            var cameraMove = GetComponent<FrameEffects.CameraMove>();

            //cameraMove.moveToPosition = Camera.main.transform.position;
            FrameController.AddAnimationToQueue(cameraMove.gameObject.name, true);
            cameraMove.StartCoroutine(cameraMove.MoveCamera(cameraMove.moveToPosition, cameraMove.speed));
        }
        public IEnumerator MoveCamera(Vector3 moveToPositon, float speed) {

            yield return new WaitForSeconds(animationDelay);

            var t = 0f;
            var start = Camera.main.transform.position;

            while (t < 1) {
                t += Time.deltaTime * speed;

                if (t > 1) t = 1;
                Camera.main.transform.position = Vector3.Lerp(start, moveToPositon, t);

                yield return null;
            }

            FrameController.RemoveAnimationFromQueue(gameObject.name);
        }
    }
}
