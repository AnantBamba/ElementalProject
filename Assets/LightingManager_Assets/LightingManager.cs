using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sydewa
{
    [ExecuteAlways]
    public class LightingManager : MonoBehaviour
    {
        #region Parameters
        // In your LightingManager script, add a Color variable for the custom color
        [SerializeField] private Color customSkyboxColor = new Color(1f, 0.6f, 0.2f);  // This is an example custom color (can be adjusted in the Inspector)

        // Scene References
        [SerializeField] private Light SunDirectionalLight;
        [SerializeField] private LightingPreset Preset; // Make sure it's assigned in the inspector

        // Rotation axis
        public enum RotationAxis { X, Y }
        [SerializeField] private RotationAxis rotationAxis = RotationAxis.X;

        [Space(10)]
        // Everything needed for the day cycle
        [Header("Day Cycle Parameters")]
        public bool IsDayCycleOn = true;
        public bool RandomStartTime;
        [Range(0, 24)] public float TimeOfDay = 12f;
        [Range(0, 24)] public float StartTime = 12f;
        // How long the day cycle will be in seconds
        [Range(1, 600)] public float CycleDuration = 360f;
        public Vector2 morningInterval = new Vector2(0f, 0.5f);
        public Vector2 afterNoonInterval = new Vector2(0.5f, 1f);
        public Vector2 lightIntensity = new Vector2(0f, 1f);
        private float intensity;

        [Space(10)]
        [Header("Shadows Parameters")]
        public bool IsShadowChangeOn;
        [Range(0f, 1f)] public float shadowStrength = 0.5f;
        private float _shadowStrength;

        [Space(10)]
        // Skybox Parameters
        [Header("Skybox Parameters")]
        public bool IsSkyBoxOn;
        public Material skyboxMat; // Changed to Material (not Skybox component)
        public string customPropertyName;
        private float skyboxParam;

        [Space(10)]
        [Header("Moon Parameters")]
        public bool IsMoonActive;
        public Light MoonDirectionalLight;
        public bool IsMoonRotationOn;
        public Vector2 MoonIntensity = new Vector2(0f, 1f);
        [Range(0f, 1f)] public float MoonShadowStrength = 0.5f;

        [Space(10)]
        [Header("Events")]
        // Enable or disable the events
        public bool IsEventsOn;
        // Create events
        public List<EventInfo> events;
        [SerializeField] private float eventsTolerance = 0.2f;
        [SerializeField][Range(0f, 24f)] private float ResetEventsTime = 0.1f;
        private bool DayCycleCompleted;

        #endregion

        private void Start()
        {
            if (IsDayCycleOn)
            {
                if (RandomStartTime)
                {
                    TimeOfDay = Random.Range(0f, 24f);
                    Debug.Log("Random Start Time: " + TimeOfDay);
                }
                else if (!RandomStartTime)
                {
                    TimeOfDay = StartTime;
                    TimeOfDay %= 24;
                }
            }

            if (IsEventsOn)
            {
                ResetEvents();
            }
        }

        private void Update()
        {
            if (Preset == null)
                return;

            if (Application.isPlaying)
            {
                if (IsDayCycleOn)
                {
                    TimeOfDay += (Time.deltaTime / CycleDuration) * 24f;
                    TimeOfDay %= 24; // Modulus to ensure always between 0-24
                }
                UpdateLighting(TimeOfDay / 24f);

                if (IsMoonActive && MoonDirectionalLight != null)
                {
                    UpdateMoonLighting(TimeOfDay / 24f);
                }
            }
            else
            {
                UpdateLighting(TimeOfDay / 24f);
            }

            // Detects when an event should trigger
            if (IsEventsOn)
            {
                foreach (var eventInfo in events)
                {
                    float timeDifference = Mathf.Abs(eventInfo.Time - TimeOfDay);
                    if (timeDifference <= eventsTolerance && !eventInfo.executed)
                    {
                        eventInfo.executed = true;
                        eventInfo.Event.Invoke();
                        Debug.Log("Event: " + eventInfo.eventName);
                    }
                }

                if (!DayCycleCompleted && TimeOfDay < ResetEventsTime)
                {
                    DayCycleCompleted = true;
                    ResetEvents();

                    Debug.Log("Day completed + reset");
                }
                else if (TimeOfDay > ResetEventsTime)
                {
                    DayCycleCompleted = false;
                }
            }
        }

        public void ResetEvents()
        {
            foreach (var eventInfo in events)
            {
                eventInfo.executed = false;
            }
        }

        private void UpdateLighting(float timePercent)
        {
            // Set ambient and fog colors from the LightingPreset gradients
            RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

            if (SunDirectionalLight != null)
            {
                // Update the directional light color using the LightingPreset gradient
                Color lightColor = Preset.DirectionalColor.Evaluate(timePercent);

                // Adjust the directional light color to have a yellowish tint closer to noon
                Color yellowishTint = Color.Lerp(lightColor, new Color(1f, 0.9f, 0.6f), Mathf.Abs(0.5f - timePercent));
                SunDirectionalLight.color = yellowishTint;

                // Set directional light intensity based on the time of day
                float lightIntensityValue = Mathf.Lerp(lightIntensity.x, lightIntensity.y, timePercent);
                SunDirectionalLight.intensity = lightIntensityValue;

                // Update the rotation of the sun based on the time of day (simulating the sun's position)
                Vector3 rotationEuler = new Vector3((timePercent * 360f) - 90f, 0, 0);
                SunDirectionalLight.transform.rotation = Quaternion.Euler(rotationEuler);

                // **Ensure shadows are enabled and configured correctly**
                SunDirectionalLight.shadows = LightShadows.Soft;
                SunDirectionalLight.shadowStrength = 1.0f;  // Ensure shadows are fully visible
                SunDirectionalLight.shadowBias = 0.05f;
                SunDirectionalLight.shadowNormalBias = 0.4f;

                // Gradually change the skybox color based on the time of day
                Color skyboxColor = Preset.SkyboxColor.Evaluate(timePercent);
                float distanceFromNoon = Mathf.Abs(0.5f - timePercent);
                float darknessFactor = Mathf.Lerp(1f, 0f, distanceFromNoon);  // Darker as time moves away from noon
                skyboxColor *= darknessFactor;  // Apply darkness to the color

                if (skyboxMat != null)
                {
                    skyboxMat.SetColor("_Tint", skyboxColor);  // Assuming you have a "_Tint" property in your skybox material
                }
            }

            // Make ambient light change gradually based on time
            RenderSettings.ambientLight = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), Preset.AmbientColor.Evaluate(timePercent), timePercent);
        }

        private void RotateSkyboxFaces(float rotation)
        {
            // Rotate the skybox textures based on the given rotation
            if (skyboxMat.HasProperty("_FrontTex"))
            {
                // Adjust texture offsets for each face (assuming the skybox material uses these properties)
                skyboxMat.SetTextureOffset("_FrontTex", new Vector2(rotation, 0));
                skyboxMat.SetTextureOffset("_BackTex", new Vector2(-rotation, 0));
                skyboxMat.SetTextureOffset("_LeftTex", new Vector2(rotation, 0));
                skyboxMat.SetTextureOffset("_RightTex", new Vector2(-rotation, 0));
                skyboxMat.SetTextureOffset("_UpTex", new Vector2(0, rotation));
                skyboxMat.SetTextureOffset("_DownTex", new Vector2(0, -rotation));
            }
            else
            {
                Debug.LogWarning("Skybox material does not have expected texture properties.");
            }
        }

        private void UpdateMoonLighting(float timePercent)
        {
            if (timePercent < morningInterval.x || timePercent > afterNoonInterval.y)
            {
                // Night
                MoonDirectionalLight.intensity = MoonIntensity.y;
                MoonDirectionalLight.shadowStrength = MoonShadowStrength;
            }
            else if (timePercent >= morningInterval.x && timePercent <= morningInterval.y)
            {
                // Morning
                float morningNormalizedTime = (timePercent - morningInterval.x) / (morningInterval.y - morningInterval.x);
                float morningIntensity = Mathf.Lerp(MoonIntensity.y, MoonIntensity.x, morningNormalizedTime);
                float morningShadowStrength = Mathf.Lerp(MoonShadowStrength, 1f, morningNormalizedTime);
                MoonDirectionalLight.intensity = morningIntensity;
                MoonDirectionalLight.shadowStrength = morningShadowStrength;
            }
            else if (timePercent > morningInterval.y && timePercent < afterNoonInterval.x)
            {
                // Day
                MoonDirectionalLight.intensity = MoonIntensity.x;
                MoonDirectionalLight.shadowStrength = 0f;
            }
            else if (timePercent >= afterNoonInterval.x && timePercent <= afterNoonInterval.y)
            {
                // Afternoon
                float afternoonNormalizedTime = (timePercent - afterNoonInterval.x) / (afterNoonInterval.y - afterNoonInterval.x);
                float afternoonIntensity = Mathf.Lerp(MoonIntensity.x, MoonIntensity.y, afternoonNormalizedTime);
                float afternoonShadowStrength = Mathf.Lerp(0f, MoonShadowStrength, afternoonNormalizedTime);
                MoonDirectionalLight.intensity = afternoonIntensity;
                MoonDirectionalLight.shadowStrength = afternoonShadowStrength;
            }

            if (IsMoonRotationOn)
            {
                Vector3 rotationEuler = Vector3.zero;
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        rotationEuler = new Vector3((timePercent * 360f) + 90f, MoonDirectionalLight.transform.localRotation.y, MoonDirectionalLight.transform.localRotation.z);
                        break;
                    case RotationAxis.Y:
                        rotationEuler = new Vector3(MoonDirectionalLight.transform.localRotation.x, (timePercent * 360f) + 90f, MoonDirectionalLight.transform.localRotation.z);
                        break;
                }
                MoonDirectionalLight.transform.localRotation = Quaternion.Euler(rotationEuler);
            }
        }

        // Try to find a directional light and skybox material to use if we haven't set one
        private void OnValidate()
        {
            //---------------------------Directional Light ----------------------------
            if (SunDirectionalLight != null)
                return;

            if (RenderSettings.sun == null)
            {
                SunDirectionalLight = RenderSettings.sun;
            }
            else
            {
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        SunDirectionalLight = light;
                        return;
                    }
                }
            }

            //--------------------------Skybox-------------------------------
            if (skyboxMat != null)
                return;

            if (RenderSettings.skybox != null)
            {
                skyboxMat = RenderSettings.skybox;
            }

            //------Moon
            if (IsMoonActive && MoonDirectionalLight != null)
            {
                UpdateMoonLighting(TimeOfDay / 24f);
            }
        }
    }
}
