using System.Collections;
using UnityEngine;

namespace FrameCore {
    namespace FrameEffects {
        //[ExecuteInEditMode]
        public class BlackScreenFadeout : EffectPrefab {

            public bool toBlack;
            public bool end = false;
            private void OnEnable() {
                var color = GetComponent<SpriteRenderer>().color;
                color.a = 255;
                // FrameController.INPUT_BLOCK = true;
            }
            void Start() {
                /**Color objectColor = GetComponent<SpriteRenderer>().color;
                if (toBlack)
                    GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
                else
                    GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);

                FrameController.AddAnimationToQueue(gameObject.name, true);
                StartCoroutine(FadeBlackOut(toBlack, speed));**/
            }

            void Update() {
                if (Camera.main != null) {
                    this.transform.position = Camera.main.transform.position - new Vector3(0, 0, -10);
                    this.transform.rotation = Camera.main.transform.rotation;
                }

                /**if (toBlack) {
                    if (GetComponent<SpriteRenderer>().color.a >= 0.98f) {
                        end = true;
                        FrameController.RemoveAnimationFromQueue(gameObject.name);
                    }
                }
                else {
                    if (GetComponent<SpriteRenderer>().color.a <= 0.02f) {
                        end = true;
                        FrameController.RemoveAnimationFromQueue(gameObject.name);
                    }
                }
                if (!end) FrameController.INPUT_BLOCK = true;**/
            }
            public override void OnFrameKeyChanged() {

                var blackoutScreenFadeout = GetComponent<FrameEffects.BlackScreenFadeout>();
                Color objectColor = blackoutScreenFadeout.GetComponent<SpriteRenderer>().color;
                if (blackoutScreenFadeout.toBlack)
                    blackoutScreenFadeout.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
                else
                    blackoutScreenFadeout.GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);

                FrameController.AddAnimationToQueue(blackoutScreenFadeout.gameObject.name, true);
                blackoutScreenFadeout.StartCoroutine(blackoutScreenFadeout.FadeBlackOut(blackoutScreenFadeout.toBlack, blackoutScreenFadeout.speed));
            }
            public IEnumerator FadeBlackOut(bool fadeToBlack = true, float fadeSpeed = 1) {
                yield return new WaitForSeconds(animationDelay);

                Color objectColor = GetComponent<SpriteRenderer>().color;
                float fadeAmount;

                if (fadeToBlack) {
                    while (GetComponent<SpriteRenderer>().color.a < 1) {
                        fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        GetComponent<SpriteRenderer>().color = objectColor;
                        yield return null;
                    }
                }
                else {
                    while (GetComponent<SpriteRenderer>().color.a > 0) {
                        fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        GetComponent<SpriteRenderer>().color = objectColor;
                        yield return null;
                    }
                }
                FrameController.RemoveAnimationFromQueue(gameObject.name);
            }
        }
    }
}
