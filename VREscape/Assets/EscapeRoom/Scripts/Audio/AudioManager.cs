using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public sealed class AudioEntry
    {
        public AudioCue cue;
        public AudioClip clip;
        [Range(0f, 1f)] public float defaultVolume = 1f;
        [Range(-3f, 3f)] public float defaultPitch = 1f;

        [Header("3D Settings")]
        [Range(0f, 1f)] public float spatialBlend = 1f;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 1f;
        public float maxDistance = 20f;
        [Range(0f, 360f)] public float spread = 0f;
        [Range(0f, 5f)] public float dopplerLevel = 1f;
    }

    private sealed class FollowTargetProxy : MonoBehaviour
    {
        public Transform target;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.position;
        }
    }

    [Header("Audio Database")]
    [SerializeField] private List<AudioEntry> audioEntries = new List<AudioEntry>();

    [Header("OneShot")]
    [SerializeField] private AudioSource oneShotSourcePrefab;
    [SerializeField] private int oneShotPoolSize = 8;

    [Header("2D Loop")]
    [SerializeField] private AudioSource loop2DSourcePrefab;

    [Header("3D Loop")]
    [SerializeField] private AudioSource loop3DSourcePrefab;

    private readonly Dictionary<AudioCue, AudioEntry> _audioMap = new Dictionary<AudioCue, AudioEntry>();
    private readonly Dictionary<AudioCue, AudioSource> _loop2DSources = new Dictionary<AudioCue, AudioSource>();
    private readonly HashSet<AudioCue> _paused2DLoops = new HashSet<AudioCue>();

    private readonly List<AudioSource> _oneShotSources = new List<AudioSource>();
    private int _oneShotCursor = 0;

    private readonly Dictionary<int, AudioSource> _loop3DSources = new Dictionary<int, AudioSource>();
    private readonly HashSet<int> _paused3DLoops = new HashSet<int>();
    private int _next3DLoopHandle = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildAudioMap();
        CreateOneShotSources();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void BuildAudioMap()
    {
        _audioMap.Clear();

        for (int i = 0; i < audioEntries.Count; i++)
        {
            AudioEntry entry = audioEntries[i];
            if (entry == null)
            {
                continue;
            }

            if (entry.cue == AudioCue.None)
            {
                Debug.LogWarning("[AudioManager] AudioCue.None cannot be used in audioEntries.");
                continue;
            }

            if (entry.clip == null)
            {
                Debug.LogWarning("[AudioManager] Missing clip for cue: " + entry.cue);
                continue;
            }

            if (_audioMap.ContainsKey(entry.cue))
            {
                Debug.LogWarning("[AudioManager] Duplicate cue found: " + entry.cue + ". Keeping first entry.");
                continue;
            }

            if (entry.minDistance < 0f)
            {
                entry.minDistance = 0f;
            }

            if (entry.maxDistance < entry.minDistance)
            {
                entry.maxDistance = entry.minDistance;
            }

            _audioMap.Add(entry.cue, entry);
        }
    }

    private void CreateOneShotSources()
    {
        _oneShotSources.Clear();

        int count = Mathf.Max(1, oneShotPoolSize);

        for (int i = 0; i < count; i++)
        {
            AudioSource source = CreateAudioSourceInstance(oneShotSourcePrefab, "OneShotSource_" + i);
            source.playOnAwake = false;
            source.loop = false;
            _oneShotSources.Add(source);
        }
    }

    private AudioSource CreateAudioSourceInstance(AudioSource prefab, string objectName)
    {
        AudioSource source;

        if (prefab != null)
        {
            source = Instantiate(prefab, transform);
        }
        else
        {
            GameObject go = new GameObject(objectName);
            go.transform.SetParent(transform);
            source = go.AddComponent<AudioSource>();
        }

        source.name = objectName;
        source.playOnAwake = false;
        return source;
    }

    private AudioSource GetOneShotSource()
    {
        for (int i = 0; i < _oneShotSources.Count; i++)
        {
            AudioSource source = _oneShotSources[i];
            if (source != null && !source.isPlaying)
            {
                return source;
            }
        }

        AudioSource fallback = _oneShotSources[_oneShotCursor];
        _oneShotCursor = (_oneShotCursor + 1) % _oneShotSources.Count;

        if (fallback != null)
        {
            fallback.Stop();
            ClearFollowTarget(fallback);
        }

        return fallback;
    }

    private bool TryGetEntry(AudioCue cue, out AudioEntry entry)
    {
        if (_audioMap.TryGetValue(cue, out entry))
        {
            return true;
        }

        Debug.LogWarning("[AudioManager] AudioCue not found: " + cue);
        return false;
    }

    private static void ApplyCommonSettings(AudioSource source, AudioEntry entry, float volumeScale, bool loop)
    {
        source.clip = entry.clip;
        source.volume = entry.defaultVolume * Mathf.Clamp01(volumeScale);
        source.pitch = entry.defaultPitch;
        source.loop = loop;
    }

    private static void Apply2DSettings(AudioSource source)
    {
        source.spatialBlend = 0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1f;
        source.maxDistance = 500f;
        source.spread = 0f;
        source.dopplerLevel = 0f;
    }

    private static void Apply3DSettings(AudioSource source, AudioEntry entry)
    {
        source.spatialBlend = Mathf.Clamp01(entry.spatialBlend);
        source.rolloffMode = entry.rolloffMode;
        source.minDistance = Mathf.Max(0f, entry.minDistance);
        source.maxDistance = Mathf.Max(entry.minDistance, entry.maxDistance);
        source.spread = Mathf.Clamp(entry.spread, 0f, 360f);
        source.dopplerLevel = Mathf.Max(0f, entry.dopplerLevel);
    }

    private static FollowTargetProxy GetOrAddFollowTarget(AudioSource source)
    {
        FollowTargetProxy proxy = source.GetComponent<FollowTargetProxy>();
        if (proxy == null)
        {
            proxy = source.gameObject.AddComponent<FollowTargetProxy>();
        }

        return proxy;
    }

    private static void SetFollowTarget(AudioSource source, Transform target)
    {
        FollowTargetProxy proxy = GetOrAddFollowTarget(source);
        proxy.target = target;
    }

    private static void ClearFollowTarget(AudioSource source)
    {
        FollowTargetProxy proxy = source.GetComponent<FollowTargetProxy>();
        if (proxy != null)
        {
            proxy.target = null;
        }
    }

    private AudioSource GetOrCreate2DLoopSource(AudioCue cue)
    {
        AudioSource existingSource;
        if (_loop2DSources.TryGetValue(cue, out existingSource) && existingSource != null)
        {
            return existingSource;
        }

        AudioSource newSource = CreateAudioSourceInstance(loop2DSourcePrefab, "Loop2DSource_" + cue);
        newSource.loop = true;
        Apply2DSettings(newSource);

        _loop2DSources[cue] = newSource;
        return newSource;
    }

    private int Create3DLoopSource(AudioCue cue, AudioEntry entry, float volumeScale)
    {
        int handle = _next3DLoopHandle;
        _next3DLoopHandle++;

        AudioSource source = CreateAudioSourceInstance(loop3DSourcePrefab, "Loop3DSource_" + handle + "_" + cue);
        ApplyCommonSettings(source, entry, volumeScale, true);
        Apply3DSettings(source, entry);

        _loop3DSources.Add(handle, source);
        return handle;
    }

    public void PlayOneShot(AudioCue cue, float volumeScale = 1f)
    {
        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return;
        }

        AudioSource source = GetOneShotSource();
        if (source == null)
        {
            Debug.LogWarning("[AudioManager] No available one-shot source.");
            return;
        }

        ClearFollowTarget(source);
        source.transform.position = transform.position;

        ApplyCommonSettings(source, entry, volumeScale, false);
        Apply2DSettings(source);
        source.Play();
    }

    public void PlayOneShot3D(AudioCue cue, Vector3 worldPosition, float volumeScale = 1f)
    {
        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return;
        }

        AudioSource source = GetOneShotSource();
        if (source == null)
        {
            Debug.LogWarning("[AudioManager] No available one-shot source.");
            return;
        }

        ClearFollowTarget(source);
        source.transform.position = worldPosition;

        ApplyCommonSettings(source, entry, volumeScale, false);
        Apply3DSettings(source, entry);
        source.Play();
    }

    public void PlayOneShotAttached(AudioCue cue, Transform followTarget, float volumeScale = 1f)
    {
        if (followTarget == null)
        {
            Debug.LogWarning("[AudioManager] PlayOneShotAttached failed. followTarget is null.");
            return;
        }

        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return;
        }

        AudioSource source = GetOneShotSource();
        if (source == null)
        {
            Debug.LogWarning("[AudioManager] No available one-shot source.");
            return;
        }

        source.transform.position = followTarget.position;
        SetFollowTarget(source, followTarget);

        ApplyCommonSettings(source, entry, volumeScale, false);
        Apply3DSettings(source, entry);
        source.Play();
    }

    public void PlayLoop(AudioCue cue, float volumeScale = 1f, bool restartIfPlaying = false)
    {
        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return;
        }

        AudioSource source = GetOrCreate2DLoopSource(cue);
        bool isSameClip = source.clip == entry.clip;
        bool isPaused = _paused2DLoops.Contains(cue);

        if (isPaused && isSameClip && !restartIfPlaying)
        {
            source.volume = entry.defaultVolume * Mathf.Clamp01(volumeScale);
            source.pitch = entry.defaultPitch;
            source.UnPause();
            _paused2DLoops.Remove(cue);
            return;
        }

        if (source.isPlaying && isSameClip && !restartIfPlaying)
        {
            return;
        }

        ClearFollowTarget(source);
        source.transform.position = transform.position;

        ApplyCommonSettings(source, entry, volumeScale, true);
        Apply2DSettings(source);
        source.Play();

        _paused2DLoops.Remove(cue);
    }

    public void StopLoop(AudioCue cue)
    {
        AudioSource source;
        if (_loop2DSources.TryGetValue(cue, out source) && source != null)
        {
            source.Stop();
            _paused2DLoops.Remove(cue);
        }
    }

    public void PauseLoop(AudioCue cue)
    {
        AudioSource source;
        if (_loop2DSources.TryGetValue(cue, out source) && source != null && source.isPlaying)
        {
            source.Pause();
            _paused2DLoops.Add(cue);
        }
    }

    public void ResumeLoop(AudioCue cue)
    {
        AudioSource source;
        if (!_loop2DSources.TryGetValue(cue, out source) || source == null)
        {
            return;
        }

        if (!_paused2DLoops.Contains(cue))
        {
            return;
        }

        if (source.clip == null)
        {
            Debug.LogWarning("[AudioManager] Cannot resume 2D loop. No clip assigned for cue: " + cue);
            return;
        }

        source.UnPause();
        _paused2DLoops.Remove(cue);
    }

    public void RestartLoop(AudioCue cue, float volumeScale = 1f)
    {
        PlayLoop(cue, volumeScale, true);
    }

    public int PlayLoop3D(AudioCue cue, Vector3 worldPosition, float volumeScale = 1f)
    {
        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return -1;
        }

        int handle = Create3DLoopSource(cue, entry, volumeScale);
        AudioSource source = _loop3DSources[handle];

        ClearFollowTarget(source);
        source.transform.position = worldPosition;
        source.Play();

        _paused3DLoops.Remove(handle);
        return handle;
    }

    public int PlayLoop3D(AudioCue cue, Transform followTarget, float volumeScale = 1f)
    {
        if (followTarget == null)
        {
            Debug.LogWarning("[AudioManager] PlayLoop3D failed. followTarget is null.");
            return -1;
        }

        AudioEntry entry;
        if (!TryGetEntry(cue, out entry))
        {
            return -1;
        }

        int handle = Create3DLoopSource(cue, entry, volumeScale);
        AudioSource source = _loop3DSources[handle];

        source.transform.position = followTarget.position;
        SetFollowTarget(source, followTarget);
        source.Play();

        _paused3DLoops.Remove(handle);
        return handle;
    }

    public void StopLoop3D(int handle)
    {
        AudioSource source;
        if (!_loop3DSources.TryGetValue(handle, out source) || source == null)
        {
            return;
        }

        source.Stop();
        _paused3DLoops.Remove(handle);
        _loop3DSources.Remove(handle);
        Destroy(source.gameObject);
    }

    public void PauseLoop3D(int handle)
    {
        AudioSource source;
        if (!_loop3DSources.TryGetValue(handle, out source) || source == null)
        {
            return;
        }

        if (!source.isPlaying)
        {
            return;
        }

        source.Pause();
        _paused3DLoops.Add(handle);
    }

    public void ResumeLoop3D(int handle)
    {
        AudioSource source;
        if (!_loop3DSources.TryGetValue(handle, out source) || source == null)
        {
            return;
        }

        if (!_paused3DLoops.Contains(handle))
        {
            return;
        }

        if (source.clip == null)
        {
            Debug.LogWarning("[AudioManager] Cannot resume 3D loop. Missing clip on handle: " + handle);
            return;
        }

        source.UnPause();
        _paused3DLoops.Remove(handle);
    }

    public void RestartLoop3D(int handle)
    {
        AudioSource source;
        if (!_loop3DSources.TryGetValue(handle, out source) || source == null)
        {
            return;
        }

        if (source.clip == null)
        {
            return;
        }

        source.Stop();
        source.Play();
        _paused3DLoops.Remove(handle);
    }

    public bool IsLoopPlaying(AudioCue cue)
    {
        AudioSource source;
        if (_loop2DSources.TryGetValue(cue, out source) && source != null)
        {
            return source.isPlaying;
        }

        return false;
    }

    public bool IsLoopPaused(AudioCue cue)
    {
        return _paused2DLoops.Contains(cue);
    }

    public bool IsLoop3DPlaying(int handle)
    {
        AudioSource source;
        if (_loop3DSources.TryGetValue(handle, out source) && source != null)
        {
            return source.isPlaying;
        }

        return false;
    }

    public bool IsLoop3DPaused(int handle)
    {
        return _paused3DLoops.Contains(handle);
    }

    public void StopAllSounds()
    {
        for (int i = 0; i < _oneShotSources.Count; i++)
        {
            AudioSource source = _oneShotSources[i];
            if (source == null)
            {
                continue;
            }

            source.Stop();
            ClearFollowTarget(source);
        }

        foreach (KeyValuePair<AudioCue, AudioSource> pair in _loop2DSources)
        {
            AudioSource source = pair.Value;
            if (source == null)
            {
                continue;
            }

            source.Stop();
        }

        List<int> handles = new List<int>(_loop3DSources.Keys);
        for (int i = 0; i < handles.Count; i++)
        {
            StopLoop3D(handles[i]);
        }

        _paused2DLoops.Clear();
        _paused3DLoops.Clear();
    }
}