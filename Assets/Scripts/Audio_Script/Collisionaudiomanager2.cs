using System.Collections;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Gestisce tutto l'audio del gioco:
    ///   - Ambience in loop finché non scatta il GameOver
    ///   - Collisioni con unica AudioSource (sempre interrompe il precedente)
    /// </summary>
    public class CollisionAudioManager2 : MonoBehaviour
    {
        public static CollisionAudioManager2 Instance { get; private set; }

        [Header("══ AMBIENCE ══")]
        [Tooltip("Clip audio da loopare per tutta la durata della partita.")]
        [SerializeField] private AudioClip ambienceClip;
        [Range(0f, 1f)][SerializeField] private float ambienceVolume = 0.4f;

        [Header("══ VEHICLE → PEDESTRIAN ══")]

        [Header("Fase 1 — Crash (immediato)")]
        [Tooltip("SFX investimento. Più clip = scelta casuale.")]
        [SerializeField] private AudioClip[] pedestrianImpactClips;
        [Range(0f, 1f)][SerializeField] private float pedestrianImpactVolume = 1f;

        [Header("Fase 2 — Voce del team (dopo delay)")]
        [Tooltip("Voci campionate dal team. Scelta casuale ad ogni morte.")]
        [SerializeField] private AudioClip[] teamVoiceClips;
        [Range(0f, 1f)][SerializeField] private float voiceVolume = 0.9f;

        [Tooltip("Secondi tra crash e voce. Calibra in base all'animazione di caduta.")]
        [SerializeField] private float bodyLandDelay = 0.8f;

        [Header("══ VEHICLE → VEHICLE ══")]
        [Tooltip("SFX scontro tra macchine. Più clip = scelta casuale.")]
        [SerializeField] private AudioClip[] vehicleCrashClips;
        [Range(0f, 1f)][SerializeField] private float vehicleCrashVolume = 1f;

        // AudioSource per l'ambience: separata così il loop non viene
        // mai interrotto dai SFX delle collisioni.
        private AudioSource _ambienceSource;

        // AudioSource unica per tutti i SFX di collisione.
        private AudioSource _sfxSource;

        private Coroutine _activeSequence;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // AudioSource ambience — loop attivo, volume basso, parte subito
            _ambienceSource = gameObject.AddComponent<AudioSource>();
            _ambienceSource.playOnAwake = false;
            _ambienceSource.spatialBlend = 0f;
            _ambienceSource.loop = true;  // loopa finché non chiamiamo Stop()
            _ambienceSource.volume = ambienceVolume;

            // AudioSource SFX collisioni — no loop, gestita manualmente
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.spatialBlend = 0f;
            _sfxSource.loop = false;

            Entity.OnVehicleHitPedestrian += HandlePedestrianHit;
            Entity.OnVehicleHitVehicle += HandleVehicleCrash;
            GameManager.OnGameOver += HandleGameOver;  // ascolta il GameOver per fermare l'ambience

            Debug.Log("[CollisionAudio] ✅ Inizializzato.");
        }

        private void Start()
        {
            // Start invece di Awake: tutti i manager sono già inizializzati
            StartAmbience();
        }

        private void OnDestroy()
        {
            Entity.OnVehicleHitPedestrian -= HandlePedestrianHit;
            Entity.OnVehicleHitVehicle -= HandleVehicleCrash;
            GameManager.OnGameOver -= HandleGameOver;
        }

        // ── Ambience ──────────────────────────────────────────────────────────

        private void StartAmbience()
        {
            if (ambienceClip == null)
            {
                Debug.LogWarning("[CollisionAudio] ⚠️ Nessun ambienceClip assegnato nell'Inspector.");
                return;
            }

            _ambienceSource.clip = ambienceClip;
            _ambienceSource.Play();
            Debug.Log($"[CollisionAudio] 🎵 Ambience avviata: '{ambienceClip.name}' in loop.");
        }

        private void HandleGameOver(int savedLives)
        {
            // GameManager setta Time.timeScale = 0 al GameOver,
            // ma AudioSource.Stop() funziona indipendentemente dal timeScale.
            _ambienceSource.Stop();
            Debug.Log("[CollisionAudio] 🔇 Ambience fermata (GameOver).");
        }

        // ── Handlers collisioni ───────────────────────────────────────────────

        private void HandlePedestrianHit(Entity vehicle, Entity pedestrian)
        {
            string name = pedestrian != null ? pedestrian.gameObject.name : "NULL";
            Debug.Log($"[CollisionAudio] 🚗💥🚶 Pedone investito: '{name}'");
            StopActiveSequence();
            _activeSequence = StartCoroutine(PedestrianDeathSequence(name));
        }

        private void HandleVehicleCrash(Entity v1, Entity v2)
        {
            string n1 = v1 != null ? v1.gameObject.name : "NULL";
            string n2 = v2 != null ? v2.gameObject.name : "NULL";
            Debug.Log($"[CollisionAudio] 🚗💥🚗 Scontro: '{n1}' vs '{n2}'");
            StopActiveSequence();
            Play(vehicleCrashClips, vehicleCrashVolume, "CRASH VEHICLE", $"{n1} vs {n2}");
        }

        // ── Sequenza morte pedone ─────────────────────────────────────────────

        private IEnumerator PedestrianDeathSequence(string pedName)
        {
            Play(pedestrianImpactClips, pedestrianImpactVolume, "IMPATTO PEDONE", pedName);

            Debug.Log($"[CollisionAudio] ⏱️ Attendo {bodyLandDelay}s per '{pedName}'...");
            yield return new WaitForSecondsRealtime(bodyLandDelay);

            Play(teamVoiceClips, voiceVolume, "VOCE TEAM", pedName);
            _activeSequence = null;
        }

        // ── Core SFX ─────────────────────────────────────────────────────────

        private void Play(AudioClip[] clips, float volume, string tag, string ctx)
        {
            var clip = Pick(clips, tag, ctx);
            if (clip == null) return;

            if (_sfxSource.isPlaying)
            {
                Debug.Log($"[CollisionAudio] 🔇 Interrompo SFX in corso per '{clip.name}'.");
                _sfxSource.Stop();
            }

            _sfxSource.clip = clip;
            _sfxSource.volume = volume;
            _sfxSource.Play();

            Debug.Log($"[CollisionAudio] 🔊 [{tag}] '{clip.name}' {clip.length:F2}s vol:{volume} | '{ctx}'");
        }

        private void StopActiveSequence()
        {
            if (_activeSequence != null)
            {
                StopCoroutine(_activeSequence);
                _activeSequence = null;
            }
        }

        private AudioClip Pick(AudioClip[] clips, string tag, string ctx)
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning($"[CollisionAudio] ⚠️ [{tag}] Nessun clip assegnato per '{ctx}'.");
                return null;
            }
            var clip = clips[Random.Range(0, clips.Length)];
            if (clip == null)
                Debug.LogWarning($"[CollisionAudio] ⚠️ [{tag}] Clip null nell'array. Controlla l'Inspector.");
            return clip;
        }

        // ── Debug ─────────────────────────────────────────────────────────────

        [ContextMenu("TEST → Pedone investito (Play Mode)")]
        private void DbgPedestrian() => HandlePedestrianHit(null, null);

        [ContextMenu("TEST → Scontro macchine")]
        private void DbgVehicle() => HandleVehicleCrash(null, null);

        [ContextMenu("TEST → Solo voce team")]
        private void DbgVoice() => Play(teamVoiceClips, voiceVolume, "VOCE TEAM", "TEST");

        [ContextMenu("TEST → Stop ambience")]
        private void DbgStopAmbience() => HandleGameOver(0);
    }
}