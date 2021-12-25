using UnityEngine;
using UnityEngine.VFX;

public class DialogueSkip : MonoBehaviour {
    public VisualEffect particleEffect;
    public float speed = 1000;
    public float spawnRate;
    public float intensity;
    private float _spawnRate;
    private float _intensity;
    bool hoverOver;
    float value = 0;
    private void Awake() {
        particleEffect = GetComponentInChildren<VisualEffect>();
        particleEffect.SetFloat("spawnRate", spawnRate);
        _spawnRate = particleEffect.GetFloat("spawnRate");
        _intensity = particleEffect.GetFloat("intensity");
    }
    private void OnEnable() {

    }

    private void Update() {
        if (FrameCore.FrameController.INPUT_BLOCK == false &&
            gameObject.transform.parent.GetComponent<FrameCore.UI.Dialogue>().autoContinue != true) {
            if (particleEffect.GetFloat("spawnRate") <= 60000) {
                value = 60000;
                particleEffect.SetFloat("spawnRate", value);
                particleEffect.SetFloat("intensity", 1);
            }
        }
        else {
            value = 0;
            particleEffect.SetFloat("spawnRate", value);
            particleEffect.SetFloat("intensity", 10);
        }
    }
}
