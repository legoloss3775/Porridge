using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace SanyaTheGame {
    public class MainMenu : MonoBehaviour {
        public GameObject logo;
        public SpriteRenderer background;
        Gradient gradient;
        private void Start() {
            var particleEffect = logo.GetComponentInChildren<VisualEffect>();
            particleEffect.SetFloat("spawnrate", 300000);
            particleEffect.SetFloat("blend", 600);
            particleEffect.SetFloat("intensity", -10);
            gradient = particleEffect.GetGradient("gradient");
            particleEffect.SetGradient("gradient", new Gradient { colorKeys = new GradientColorKey[1] { new GradientColorKey(Color.red, 1) }, alphaKeys = new GradientAlphaKey[1] { new GradientAlphaKey(255, 1) } });
            StartCoroutine(ParticlesOnEnableAnim());

        }

        public IEnumerator ParticlesOnEnableAnim() {
            var particleEffect = logo.GetComponentInChildren<VisualEffect>();
            /** float spawnrate = 0;
             float blend = 1;
             while (spawnrate < 300000f) {
                 spawnrate += 1000f;
                 blend -= 0.001f;
                 particleEffect.SetFloat("spawnrate", spawnrate);
                 particleEffect.SetFloat("blend", blend);
                 yield return null;
             }**/
            yield return new WaitForSeconds(0.5f);
            particleEffect.SetFloat("spawnrate", 120000);
            particleEffect.SetFloat("blend", 3f);
            particleEffect.SetFloat("intensity", 0.01f);
            particleEffect.SetGradient("gradient", gradient);
            yield return null;
        }

    }
}
