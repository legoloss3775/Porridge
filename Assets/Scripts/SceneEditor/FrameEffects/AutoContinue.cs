using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.FrameEffects {
    public class AutoContinue : EffectPrefab {

        public override void OnFrameKeyChanged() => StartCoroutine(AutoContinueFrame());
        public IEnumerator AutoContinueFrame() {
            yield return new WaitForSeconds(animationDelay);

            FrameManager.SetKey(GetComponent<FrameEffect>().keySequenceData.nextKeyID);
            yield return null;
        }
    }
}

