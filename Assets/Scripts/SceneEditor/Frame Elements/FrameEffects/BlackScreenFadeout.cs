using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore {
    namespace FrameEffects {
        //[ExecuteInEditMode]
        public class BlackScreenFadeout : MonoBehaviour {
            public float speed { get { if (GetComponent<FrameEffect>() != null) return GetComponent<FrameEffect>().animationSpeed; else return 0.5f; } }
            public bool toBlack;
            public bool end = false;
            private void OnEnable() {
                FrameController.INPUT_BLOCK = true;
            }
            void Start() {
                Color objectColor = GetComponent<SpriteRenderer>().color;
                if (toBlack)
                    GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
                else
                    GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);

                FrameController.AddAnimationToQueue(name, true);
                StartCoroutine(FadeBlackOut(toBlack, speed));
            }

            void Update() {
                if (toBlack) {
                    if (GetComponent<SpriteRenderer>().color.a >= 0.98f) {
                        end = true;
                        FrameController.RemoveAnimationFromQueue(name);
                    }
                    if (!Application.isPlaying) {
                        Color objectColor = GetComponent<SpriteRenderer>().color;
                        GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
                        StartCoroutine(FadeBlackOut(toBlack, speed));
                    }
                }
                else {
                    if (GetComponent<SpriteRenderer>().color.a <= 0.02f) {
                        end = true;
                        FrameController.RemoveAnimationFromQueue(name);
                    }
                    if (!Application.isPlaying) {
                        Color objectColor = GetComponent<SpriteRenderer>().color;
                        GetComponent<SpriteRenderer>().color = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);
                        StartCoroutine(FadeBlackOut(toBlack, speed));
                    }
                }
                if (!end) FrameController.INPUT_BLOCK = true;
            }
            public IEnumerator FadeBlackOut(bool fadeToBlack = true, float fadeSpeed = 1) {
                Color objectColor = GetComponent<SpriteRenderer>().color;
                float fadeAmount;

                if (fadeToBlack) {
                    while(GetComponent<SpriteRenderer>().color.a < 1) {
                        fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        GetComponent<SpriteRenderer>().color = objectColor;
                        yield return null;
                    }
                }
                else {
                    while(GetComponent<SpriteRenderer>().color.a > 0) {
                        fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        GetComponent<SpriteRenderer>().color = objectColor;
                        yield return null;
                    }
                }
            }
        }
    }
}
