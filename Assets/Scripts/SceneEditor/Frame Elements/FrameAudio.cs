using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore {
    namespace Serialization {
        public class FrameAudioValues : Values {
            public FrameAudioData audioData;
            public FrameAudioValues(FrameAudio audio) {
                transformData = new TransformData {
                    position = audio.position,
                    activeStatus = audio.activeStatus,
                    rotation = audio.rotation.eulerAngles,
                    size = audio.size,
                };
                audioData = new FrameAudioData {
                    volume = audio.data.volume,
                }; 
            }
            public FrameAudioValues() { }
            [System.Serializable]
            public struct SerializedFrameAudioValues {
                public TransformData transformData;
                public FrameAudioData audioData;
            }
            [SerializeField]
            public SerializedFrameAudioValues serializedFrameAudioValues {
                get {
                    return new SerializedFrameAudioValues {
                        transformData = transformData,
                        audioData = audioData,
                    };
                }
            }
            public static void LoadSerializedFrameAudioValues(List<SerializedFrameAudioValues> serializedFrameAudioValues, List<Values> values) {
                foreach(var svalue in serializedFrameAudioValues) {
                    values.Add(new FrameAudioValues {
                        transformData = svalue.transformData,
                        audioData = svalue.audioData,
                    });
                }
            }
        }
    }
    public class FrameAudio : FrameElement {
        public Serialization.FrameAudioData data;
    }
}
