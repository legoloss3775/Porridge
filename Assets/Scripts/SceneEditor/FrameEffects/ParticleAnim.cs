using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace FrameCore.FrameEffects {

    public class ParticleAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public VisualEffect particleEffect;
        public Button button;
        public float speed = 1000;
        public float spawnRate;
        private float _spawnRate;
        bool hoverOver;
        private void Awake() {
            particleEffect = GetComponentInChildren<VisualEffect>();
            button = GetComponent<Button>();
            particleEffect.SetFloat("spawnRate", spawnRate );
            _spawnRate = particleEffect.GetFloat("spawnRate");
        }
        private void OnEnable() {
            StartCoroutine(ParticlesOnEnableAnim(speed));
        }

        private void Update() {
            if (particleEffect.GetFloat("spawnRate") > spawnRate ) StopAllCoroutines();
            if (hoverOver) particleEffect.SetFloat("spawnRate", _spawnRate);
            else particleEffect.SetFloat("spawnRate", _spawnRate / 5);
        }
        public IEnumerator ParticlesOnEnableAnim(float speed) {
            float value = 0;
            while (particleEffect.GetFloat("spawnRate") <= spawnRate / 2) {
                particleEffect.SetFloat("spawnRate", value);
                value += 250;
                yield return null;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            hoverOver = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            hoverOver = false;
        }
    }
}
