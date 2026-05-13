using System.Collections;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Gestisce l'audio di tutte le collisioni.
    /// Ascolta Entity.OnVehicleHitPedestrian e Entity.OnVehicleHitVehicle.
    ///
    /// Setup: aggiungi questo script a un GameObject "AudioManager" in scena
    /// e assegna i clip nell'Inspector.
    /// </summary>
    public class CollisionAudioManager : MonoBehaviour
    {
        public static CollisionAudioManager Instance { get; private set; }

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

        [Header("Crash metallico (immediato)")]
        [Tooltip("SFX scontro tra macchine. Più clip = scelta casuale.")]
        [SerializeField] private AudioClip[] vehicleCrashClips;
        [Range(0f, 1f)][SerializeField] private float vehicleCrashVolume = 1f;

        // Tre AudioSource separate: crash pedone, voce team, crash macchine.
        // Volumi indipendenti, nessuna interferenza reciproca.
        private AudioSource _impactSource;
        private AudioSource _voiceSource;
        private AudioSource _vehicleSource;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            _impactSource = MakeSource();
            _voiceSource = MakeSource();
            _vehicleSource = MakeSource();

            Entity.OnVehicleHitPedestrian += HandlePedestrianHit;
            Entity.OnVehicleHitVehicle += HandleVehicleCrash;

            Debug.Log("[CollisionAudio] ✅ Inizializzato.");
        }

        private void OnDestroy()
        {
            Entity.OnVehicleHitPedestrian -= HandlePedestrianHit;
            Entity.OnVehicleHitVehicle -= HandleVehicleCrash;
        }

        private AudioSource MakeSource()
        {
            var s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.spatialBlend = 0f;
            return s;
        }

        // ── Handlers ─────────────────────────────────────────────────────────

        private void HandlePedestrianHit(Entity vehicle, Entity pedestrian)
        {
            string name = pedestrian != null ? pedestrian.gameObject.name : "NULL";
            Debug.Log($"[CollisionAudio] 🚗💥🚶 Pedone investito: '{name}'");
            StartCoroutine(PedestrianDeathSequence(name));
        }

        private void HandleVehicleCrash(Entity v1, Entity v2)
        {
            string n1 = v1 != null ? v1.gameObject.name : "NULL";
            string n2 = v2 != null ? v2.gameObject.name : "NULL";
            Debug.Log($"[CollisionAudio] 🚗💥🚗 Scontro: '{n1}' vs '{n2}'");
            PlayOneShot(_vehicleSource, vehicleCrashClips, vehicleCrashVolume, "CRASH VEHICLE", $"{n1} vs {n2}");
        }

        // ── Sequenza morte pedone ─────────────────────────────────────────────

        private IEnumerator PedestrianDeathSequence(string pedName)
        {
            PlayOneShot(_impactSource, pedestrianImpactClips, pedestrianImpactVolume, "IMPATTO PEDONE", pedName);

            Debug.Log($"[CollisionAudio] ⏱️ Attendo {bodyLandDelay}s per '{pedName}'...");
            // RealTime: funziona anche con Time.timeScale = 0 (GameOver di GameManager)
            yield return new WaitForSecondsRealtime(bodyLandDelay);

            PlayExclusive(_voiceSource, teamVoiceClips, voiceVolume, "VOCE TEAM", pedName);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        // Sovrappone i clip: ideale per impact SFX multipli ravvicinati
        private void PlayOneShot(AudioSource src, AudioClip[] clips, float vol, string tag, string ctx)
        {
            var clip = Pick(clips, tag, ctx);
            

            if (clip == null)
            {
                Debug.LogWarning($"[CollisionAudio] ⚠️ [{tag}] Nessun clip valido per '{ctx}'. Audio saltato.");
                return;
            }    
                
            src.PlayOneShot(clip, vol);
            Debug.Log($"[CollisionAudio] 🔊 [{tag}] '{clip.name}' {clip.length:F2}s vol:{vol} | '{ctx}'");
        }

        // Una sola voce alla volta: ferma la precedente se ancora in corso
        private void PlayExclusive(AudioSource src, AudioClip[] clips, float vol, string tag, string ctx)
        {
            var clip = Pick(clips, tag, ctx);
            if (clip == null) return;
            if (src.isPlaying) src.Stop();
            src.clip = clip;
            src.volume = vol;
            src.Play();
            Debug.Log($"[CollisionAudio] 🎙️ [{tag}] '{clip.name}' {clip.length:F2}s vol:{vol} | '{ctx}'");
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
        private void DbgPedestrian() => StartCoroutine(PedestrianDeathSequence("PEDONE_TEST"));

        [ContextMenu("TEST → Scontro macchine")]
        private void DbgVehicle() => PlayOneShot(_vehicleSource, vehicleCrashClips, vehicleCrashVolume, "CRASH VEHICLE", "TEST");

        [ContextMenu("TEST → Solo voce team")]
        private void DbgVoice() => PlayExclusive(_voiceSource, teamVoiceClips, voiceVolume, "VOCE TEAM", "TEST");
    }
}