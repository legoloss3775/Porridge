using System.Collections;
using UnityEngine;

namespace FrameCore.FrameEffects {
    public class CameraMove : MonoBehaviour {
        public float speed { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().animationSpeed; else return 1f; } }
        public float animationDelay { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().animationDelay; else return 0f; } }
        public Vector3 moveToPosition { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().cameraTurnAnimationData.moveTo; else return Vector3.zero; } }

        private void OnEnable() {

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
