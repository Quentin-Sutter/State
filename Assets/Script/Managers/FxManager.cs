using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-50)]
public class FxManager : MonoBehaviour
{
    // --- Singleton facultatif (pratique pour un prototype) ---
    public static FxManager Instance { get; private set; }

    [Header("=== VFX Registry ===")]
    [SerializeField] private List<VfxEntry> vfx = new List<VfxEntry>();

    [Header("=== SFX Registry ===")]
    [SerializeField] private List<SfxEntry> sfx = new List<SfxEntry>();
    [SerializeField] private AudioMixerGroup outputMixer; // optionnel

    [Header("=== Damage Numbers (optional) ===")]
    [SerializeField] private GameObject damageNumberPrefab;

    [Header("=== Screen Shake ===")]
    [SerializeField] private Transform shakeTarget; // par défaut, Camera.main.transform au Start
    [SerializeField] private float shakeFrequency = 35f;

    // --- pools internes ---
    readonly Dictionary<string, Queue<GameObject>> vfxPools = new();
    readonly Dictionary<string, VfxEntry> vfxMap = new();
    readonly Dictionary<string, SfxEntry> sfxMap = new();

    AudioSource oneShotSource; // fallback (pour sfx sans spatialisation)
    Vector3 shakeBasePos;
    Coroutine shakeRoutine;
    Coroutine hitStopRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (shakeTarget == null && Camera.main != null) shakeTarget = Camera.main.transform;
        if (shakeTarget != null) shakeBasePos = shakeTarget.localPosition;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        if (outputMixer) oneShotSource.outputAudioMixerGroup = outputMixer;
        oneShotSource.playOnAwake = false;

        // Build maps + prewarm pools
        foreach (var e in vfx)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.key) || e.prefab == null) continue;
            vfxMap[e.key] = e;
            vfxPools[e.key] = new Queue<GameObject>();
            for (int i = 0; i < Mathf.Max(0, e.prewarm); i++)
                vfxPools[e.key].Enqueue(CreateVfxInstance(e));
        }

        foreach (var a in sfx)
            if (a != null && !string.IsNullOrWhiteSpace(a.key) && a.clip != null) sfxMap[a.key] = a;
    }

    // --- PUBLIC API ----------------------------------------------------------

    /// <summary>
    /// Joue un VFX par clé (poolé). Retourne l'instance.
    /// </summary>
    public GameObject PlayVFX(
        string key,
        Vector3 position,
        Quaternion? rotation = null,
        Transform parent = null,
        float scale = 1f,
        Color? colorOverride = null,
        float? lifetimeOverride = null)
    {
        if (!vfxMap.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"[FxManager] VFX key not found: {key}");
            return null;
        }

        var go = GetFromPool(entry);
        var t = go.transform;
        t.SetParent(parent, worldPositionStays: true);
        t.position = position;
        t.rotation = rotation ?? Quaternion.identity;
        t.localScale = Vector3.one * scale;

        // Optionnel : applique une couleur si un SpriteRenderer est présent
        if (colorOverride.HasValue)
        {
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr) sr.color = colorOverride.Value;
        }

        // (ré)active et (re)lance les systèmes
        go.SetActive(true);
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true)) { ps.Clear(true); ps.Play(true); }

        float life = lifetimeOverride ?? entry.lifetime;
        if (entry.autoReturnToPool && life > 0f) StartCoroutine(ReturnAfter(go, entry.key, life));

        return go;
    }

    /// <summary>
    /// Joue un SFX par clé. Si position fourni -> AudioSource 3D temporaire.
    /// </summary>
    public void PlaySFX(string key, Vector3? worldPos = null, float volume = 1f, float pitch = 1f)
    {
        if (!sfxMap.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"[FxManager] SFX key not found: {key}");
            return;
        }

        if (worldPos.HasValue)
        {
            // Crée un AudioSource 3D éphémère
            var go = new GameObject($"SFX_{key}");
            go.transform.position = worldPos.Value;
            var src = go.AddComponent<AudioSource>();
            if (outputMixer) src.outputAudioMixerGroup = outputMixer;
            src.spatialBlend = 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.maxDistance = 25f;
            src.pitch = pitch;
            src.volume = volume * entry.volume;
            src.clip = entry.clip;
            src.Play();
            Destroy(go, entry.clip.length / Mathf.Max(0.01f, pitch));
        }
        else
        {
            oneShotSource.pitch = pitch;
            oneShotSource.volume = volume * entry.volume;
            oneShotSource.PlayOneShot(entry.clip);
        }
    }

    /// <summary> Petit écran shake (caméra) non-cumulatif. </summary>
    public void ScreenShake(float intensity, float duration)
    {
        if (shakeTarget == null || intensity <= 0f || duration <= 0f) return;
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(DoScreenShake(intensity, duration));
    }

    /// <summary> Hit-stop : ralentit le temps brièvement. </summary>
    public void HitStop(float timeScale, float duration)
    {
        if (hitStopRoutine != null) StopCoroutine(hitStopRoutine);
        hitStopRoutine = StartCoroutine(DoHitStop(Mathf.Clamp(timeScale, 0.01f, 1f), Mathf.Max(0f, duration)));
    }

    /// <summary> Flash rapide d'un SpriteRenderer (dommages, parry…). </summary>
    public void FlashSprite(SpriteRenderer sr, Color color, float duration = 0.08f)
    {
        if (!sr) return;
        StartCoroutine(DoFlash(sr, color, duration));
    }

    /// <summary> Damage numbers simplistes (nécessite damageNumberPrefab avec un Text/TMP). </summary>
    public void SpawnDamageNumber(int amount, Vector3 worldPos, Color? color = null)
    {
        if (!damageNumberPrefab) return;
        var go = Instantiate(damageNumberPrefab, worldPos, Quaternion.identity);
        go.transform.localScale = Vector3.one;

        // Support TextMeshPro OU TextMesh
        var tmp = go.GetComponentInChildren<TMPro.TMP_Text>();
        if (tmp)
        {
            tmp.text = amount.ToString();
            if (color.HasValue) tmp.color = color.Value;
        }
        else
        {
            var text = go.GetComponentInChildren<TextMesh>();
            if (text)
            {
                text.text = amount.ToString();
                if (color.HasValue) text.color = color.Value;
            }
        }

        // Mini anim ascendante
        StartCoroutine(DoDamageNumberRise(go, 0.6f, 1.2f));
    }

    // --- Helpers privés ------------------------------------------------------

    GameObject CreateVfxInstance(VfxEntry e)
    {
        var go = Instantiate(e.prefab);
        go.SetActive(false);
        go.name = $"VFX_{e.key}";
        return go;
    }

    GameObject GetFromPool(VfxEntry e)
    {
        var pool = vfxPools[e.key];
        if (pool.Count == 0) pool.Enqueue(CreateVfxInstance(e));
        return pool.Dequeue();
    }

    IEnumerator ReturnAfter(GameObject go, string key, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReturnToPool(go, key);
    }

    void ReturnToPool(GameObject go, string key)
    {
        if (!go) return;
        go.SetActive(false);
        go.transform.SetParent(transform, worldPositionStays: false);
        if (!vfxPools.TryGetValue(key, out var q)) return;
        q.Enqueue(go);
    }

    IEnumerator DoScreenShake(float intensity, float duration)
    {
        var t = 0f;
        var freq = shakeFrequency;
        while (t < duration)
        {
            t += Time.deltaTime;
            float decay = 1f - (t / duration);
            float x = (Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(1f, Time.time * freq) - 0.5f) * 2f;
            shakeTarget.localPosition = shakeBasePos + new Vector3(x, y, 0f) * intensity * decay;
            yield return null;
        }
        shakeTarget.localPosition = shakeBasePos;
        shakeRoutine = null;
    }

    IEnumerator DoHitStop(float ts, float duration)
    {
        var original = Time.timeScale;
        Time.timeScale = ts;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = original;
        hitStopRoutine = null;
    }

    IEnumerator DoFlash(SpriteRenderer sr, Color flash, float duration)
    {
        var original = sr.color;
        sr.color = flash;
        yield return new WaitForSeconds(duration);
        if (sr) sr.color = original;
    }

    IEnumerator DoDamageNumberRise(GameObject go, float duration, float height)
    {
        if (!go) yield break;
        var t = 0f;
        var start = go.transform.position;
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = t / duration; // 0->1
            // Ease OutCubic inverse (monte puis s’estompe)
            float y = Mathf.Lerp(0f, height, 1f - Mathf.Pow(1f - u, 3f));
            go.transform.position = start + Vector3.up * y;
            // fade si TMP
            var tmp = go.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp) tmp.alpha = 1f - u;
            yield return null;
        }
        Destroy(go);
    }

    // --- Types de données ----------------------------------------------------

    [System.Serializable]
    public class VfxEntry
    {
        public string key;
        public GameObject prefab;
        public int prewarm = 0;
        public bool autoReturnToPool = true;
        public float lifetime = 0.6f;
    }

    [System.Serializable]
    public class SfxEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }
}
