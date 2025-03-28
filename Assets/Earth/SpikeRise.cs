using System.Collections;
using UnityEngine;

public class SpikeRiseWithEffects : MonoBehaviour
{
    [Header("Spike Movement")]
    public float riseHeight = 5f;
    public float riseDuration = 3f;
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;
<<<<<<< HEAD
    public AnimationCurve riseCurve;

    [Header("Timing Controls")]
    public float delayBeforeAttach = 1.5f;
    public float shakeTime = 1.2f;
    public float delayBeforeDrop = 2.5f;

    [Header("References")]
    public Transform houseTransform;
    public CameraShake cameraShake;
    public Rigidbody[] detachableParts;
    public AudioSource spikeAudio;
=======

    [Header("Timing Controls")]
    public float delayBeforeAttach = 1.5f;      // 穿透后绑定房子
    public float shakeTime = 1.2f;              // 摄像机震动
    public float delayBeforeDrop = 2.5f;        // 零件掉落时间

    [Header("References")]
    public Transform houseTransform;            // 拖入房子 Transform
    public CameraShake cameraShake;             // 拖入摄像机外壳的 CameraShake 脚本
    public Rigidbody[] detachableParts;         // 可掉落零件的 Rigidbody 组
>>>>>>> Max

    [Header("Camera Shake Settings")]
    public float camShakeDuration = 0.5f;
    public float camShakeMagnitude = 0.2f;

    private Vector3 startPos;
    private float elapsedTime = 0f;
    private bool rising = false;
    private bool hasTriggered = false;
    private bool hasAttachedHouse = false;
    private bool hasShaken = false;
    private bool partsDropped = false;
<<<<<<< HEAD
    private bool audioPlayed = false;
=======
>>>>>>> Max

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (rising)
        {
            elapsedTime += Time.deltaTime;
<<<<<<< HEAD
            float t = Mathf.Clamp01(elapsedTime / riseDuration);
            float curveValue = riseCurve.Evaluate(t);
            float newY = Mathf.Lerp(startPos.y, startPos.y + riseHeight, curveValue);

=======
            float progress = Mathf.Clamp01(elapsedTime / riseDuration);

            // 地刺上升高度
            float newY = Mathf.Lerp(startPos.y, startPos.y + riseHeight, progress);

            // 加入 Perlin Noise 抖动
>>>>>>> Max
            float xShake = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
            float zShake = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

            transform.position = new Vector3(startPos.x + xShake, newY, startPos.z + zShake);

<<<<<<< HEAD
            if (!audioPlayed)
            {
                if (spikeAudio != null)
                {
                    spikeAudio.Play();
                }
                audioPlayed = true;
            }

=======
            // 📌 穿透后绑定房子
>>>>>>> Max
            if (!hasAttachedHouse && elapsedTime >= delayBeforeAttach)
            {
                houseTransform.SetParent(this.transform);
                hasAttachedHouse = true;
<<<<<<< HEAD
            }

=======
                Debug.Log("🏠 House attached to spike.");
            }

            // 📸 摄像机震动
>>>>>>> Max
            if (!hasShaken && elapsedTime >= shakeTime)
            {
                if (cameraShake != null)
                {
                    cameraShake.TriggerShake(camShakeDuration, camShakeMagnitude);
<<<<<<< HEAD
=======
                    Debug.Log("📸 Camera shake triggered.");
>>>>>>> Max
                }
                hasShaken = true;
            }

<<<<<<< HEAD
=======
            // 🧱 零件掉落
>>>>>>> Max
            if (!partsDropped && elapsedTime >= delayBeforeDrop)
            {
                foreach (Rigidbody part in detachableParts)
                {
                    if (part != null)
                    {
                        part.isKinematic = false;
                        part.useGravity = true;
                        part.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
                    }
                }
                partsDropped = true;
<<<<<<< HEAD
            }

            if (t >= 1f)
=======
                Debug.Log("🪵 Detached parts dropped.");
            }

            // 上升完成，锁定到最终高度（可选）
            if (progress >= 1f)
>>>>>>> Max
            {
                rising = false;
                transform.position = new Vector3(startPos.x, startPos.y + riseHeight, startPos.z);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            rising = true;
<<<<<<< HEAD
=======
            Debug.Log("🔥 Spike rise triggered by player.");
>>>>>>> Max
        }
    }
}
