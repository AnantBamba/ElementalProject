using UnityEngine;

namespace Sydewa
{
    [System.Serializable]
    [CreateAssetMenu(fileName ="Lighting Preset", menuName ="Scriptables/Lighting Preset",order =1)]
    public class LightingPreset : ScriptableObject
    {
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;

        // Add this to your LightingPreset class
        public Gradient SkyboxColor;  // This will control the skybox color during the day/night cycle
    }
}
