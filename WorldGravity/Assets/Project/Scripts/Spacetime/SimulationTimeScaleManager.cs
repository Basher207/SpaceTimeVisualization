using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GravityVisualiser
{
    public class SimulationTimeScaleManager : MonoBehaviour
    {
        public static float timeScale = 1f;
        public const float defaultFixedDeltaTime = 0.02f;

        [SerializeField] private Slider slider;

        [SerializeField] private float minTimeScale = 1f;
        [SerializeField] private float maxTimeScale = 30f;

        void Update()
        {
            float targetTimeScale = Mathf.Lerp(minTimeScale, maxTimeScale, slider.value);
            Time.fixedDeltaTime = defaultFixedDeltaTime / targetTimeScale;
            timeScale = Mathf.Lerp(timeScale, targetTimeScale, Time.deltaTime * 10f);
        }
    }
}