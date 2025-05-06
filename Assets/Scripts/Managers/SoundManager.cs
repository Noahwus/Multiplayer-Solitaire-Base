using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    private void Awake() => NewInstance(this);

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Space]
    public bool MusicON = true;
    public bool SfxON = true;
    [Space]
    public AudioClip[] audioClips;
    public AudioClip[] musicClips;

    [SerializeField] protected AudioSource _backgroundMusic;
    protected List<AudioSource> _loopingSounds;
    private AudioSourcePool audioSourcePool;

    void Start()
    {
        audioSourcePool = new AudioSourcePool(10);
        _loopingSounds = new List<AudioSource>();
        PlayNextBackgroundMusic();
    }

    //public virtual void PlayBackgroundMusic(AudioSource Music)
    //{
    //    if (!MusicON)
    //        return;

    //    if (_backgroundMusic != null)
    //        _backgroundMusic.Stop();

    //    _backgroundMusic = Music;
    //    _backgroundMusic.volume = musicVolume;
    //    _backgroundMusic.Play();
    //}

    private int currentMusicIndex = 0;

    public void PlayNextBackgroundMusic()
    {
        if (!MusicON || musicClips.Length == 0)
            return;

        if (_backgroundMusic == null)
        {
            _backgroundMusic = gameObject.AddComponent<AudioSource>();
            _backgroundMusic.loop = false;
        }

        _backgroundMusic.clip = musicClips[currentMusicIndex];
        _backgroundMusic.volume = musicVolume;
        _backgroundMusic.Play();

        StartCoroutine(WaitForMusicEnd());
    }

    private IEnumerator WaitForMusicEnd()
    {
        while (_backgroundMusic.isPlaying)
        {
            yield return null;
        }

        currentMusicIndex = (currentMusicIndex + 1) % musicClips.Length;
        PlayNextBackgroundMusic();
    }


    #region PlayAudio /////////////////////////////////////////////

    public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioSource reuseSource = null, AudioMixerGroup audioGroup = null)
    {
        string buffer = "";
        if (!SfxON || sfx == null)
        {
            Debug.Log("sfxON or sfx returned null");
            return null;
        }

        var audioSource = reuseSource ?? audioSourcePool.Get();
        audioSource.transform.position = location;

        audioSource.time = 0.0f; // Reset time in case it's a reusable one.

        // Set the clip and other properties
        audioSource.clip = sfx; buffer += audioSource.clip.name + "\n";
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.panStereo = pan;
        audioSource.volume = sfxVolume * volumeMultiplier; buffer += $"{sfxVolume} * {volumeMultiplier} = {sfxVolume * volumeMultiplier}\n";
        audioSource.loop = loop;
        if (audioGroup) { audioSource.outputAudioMixerGroup = audioGroup; buffer += $"{audioSource.outputAudioMixerGroup.name}\n"; }

        // Play the sound
        audioSource.Play();

        if (!loop && reuseSource == null)
        {
            StartCoroutine(ReturnToPoolAfterPlaying(audioSource, sfx.length + .1f)); buffer += $"return after {sfx.length} + .1f";
        }

        if (loop)
        {
            _loopingSounds.Add(audioSource);
        }

        //Debug.Log(buffer);
        return audioSource;
    }
    public virtual AudioSource PlaySound(AudioClips sfx, Vector3 location, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioSource reuseSource = null, AudioMixerGroup audioGroup = null)
    {
        var myAudioClip = sfx switch
        {
            AudioClips.Zero => audioClips[0],
            AudioClips.One => audioClips[1],
            AudioClips.Two => audioClips[2],
            AudioClips.Boing => audioClips[3],
            AudioClips.UiClick => audioClips[4],
            AudioClips.WooshDown => audioClips[5],
            AudioClips.WooshUp => audioClips[6],
            AudioClips.Gunshot1 => audioClips[7],
            AudioClips.Gunshot2 => audioClips[8],
            AudioClips.Gunshot3 => audioClips[9],
            AudioClips.Gunshot4 => audioClips[10],
            AudioClips.ShieldSlam1 => audioClips[11],
            AudioClips.ShieldSlam2 => audioClips[12],
            AudioClips.ShieldSlam3 => audioClips[13],
            AudioClips.ShieldRecall1 => audioClips[14],
            AudioClips.ShieldRecall2 => audioClips[15],
            AudioClips.ShieldRecall3 => audioClips[16],
            AudioClips.Sparks1 => audioClips[17],
            AudioClips.Sparks2 => audioClips[18],
            AudioClips.PuffExp => audioClips[19],
            AudioClips.FabricSwipe1 => audioClips[20],
            AudioClips.FabricSwipe2 => audioClips[21],
            AudioClips.FabricSwipe3 => audioClips[22],
            AudioClips.FabricSwipe4 => audioClips[23],
            AudioClips.FabricSwipe5 => audioClips[24],
            AudioClips.CardShuffle  => audioClips[25],
            AudioClips.TommyGun => audioClips[26],
            AudioClips.Zapper   => audioClips[27],
            AudioClips.Casing1 => audioClips[28],
            AudioClips.Casing2 => audioClips[29],
            AudioClips.Casing3 => audioClips[30],
            AudioClips.TommyGet => audioClips[31],
            AudioClips.ZapperGet => audioClips[32],
            AudioClips.CatMeow1 => audioClips[33],
            AudioClips.CatMeow2 => audioClips[34],
            AudioClips.CatMeow3 => audioClips[35],
            //AudioClips.RecyclerFinish => audioClips[27],
            //AudioClips.RecyclerPopout => audioClips[28],
            //AudioClips.Shredder => audioClips[29], //SoundManager.Instance.PlaySound(SoundManager.Instance.audioClips[Utils.Rand(28,29,30)], Vector3.zero);

            _ => null
        };

        return PlaySound(myAudioClip, location, pitch, pan, spatialBlend, volumeMultiplier, loop, reuseSource, audioGroup);
    }

    public virtual AudioSource PlaySound(int i, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioSource reuseSource = null, AudioMixerGroup audioGroup = null)
    {
        return PlaySound(audioClips[i], Vector3.zero, pitch, pan, spatialBlend, volumeMultiplier, loop, reuseSource, audioGroup);
    }
    public virtual AudioSource PlaySound(int i, Vector3 location, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioSource reuseSource = null, AudioMixerGroup audioGroup = null)
    {
        return PlaySound(audioClips[i], location, pitch, pan, spatialBlend, volumeMultiplier, loop, reuseSource, audioGroup);
    }


    public void PlayAudioWithFading(int sfx, Vector3 location, float fadeDuration, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioMixerGroup audioGroup = null)
    {
        PlayAudioWithFading(audioClips[sfx], location, fadeDuration, pitch, pan, spatialBlend, volumeMultiplier, loop, audioGroup);
    }
    public void PlayAudioWithFading(AudioClip sfx, Vector3 location, float fadeDuration, float pitch = 1f, float pan = 0.0f, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false, AudioMixerGroup audioGroup = null)
    {
        var audioSource = PlaySound(sfx, location, pitch, pan, spatialBlend, volumeMultiplier, loop, null, audioGroup);
        if (audioSource != null)
        {
            StartCoroutine(FadeOutAndReturn(audioSource, fadeDuration));
        }
    }
    #endregion //////////////////////////////////////////////////////


    private IEnumerator FadeOutAndReturn(AudioSource source, float fadeDuration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        source.Stop();
        audioSourcePool.Return(source);
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSourcePool.Return(source);
    }

    public void StopLoopingSound(AudioSource source)
    {
        if (_loopingSounds.Contains(source))
        {
            source.Stop();
            _loopingSounds.Remove(source);
            audioSourcePool.Return(source);
        }
    }

    public enum AudioClips
    {
        Zero            = 0,
        One             = 1,
        Two             = 2, 
        Boing           = 3,
        UiClick         = 4,
        WooshDown       = 5, 
        WooshUp         = 6, 
        Gunshot1        = 7,
        Gunshot2        = 8,
        Gunshot3        = 9,
        Gunshot4        = 10,
        ShieldSlam1     = 11,
        ShieldSlam2     = 12,
        ShieldSlam3     = 13,
        ShieldRecall1   = 14,
        ShieldRecall2   = 15,
        ShieldRecall3   = 16,
        Sparks1         = 17,
        Sparks2         = 18,
        PuffExp         = 19,
        FabricSwipe1    = 20, 
        FabricSwipe2    = 21,
        FabricSwipe3    = 22,
        FabricSwipe4    = 23,
        FabricSwipe5    = 24,
        CardShuffle     = 25,
        TommyGun        = 26,
        Zapper          = 27,
        Casing1         = 28,
        Casing2         = 29,
        Casing3         = 30,
        TommyGet        = 31,
        ZapperGet       = 32,
        CatMeow1        = 33,
        CatMeow2        = 34,
        CatMeow3        = 35,
        //RecyclerFinish  = 27,
        //RecyclerPopout  = 28,
        //Shredder        = 29
    }
}


public class AudioSourcePool
{   private Queue<AudioSource> pool = new Queue<AudioSource>();
    private GameObject poolHost;

    public AudioSourcePool(int initialCapacity)
    {
        poolHost = new GameObject("AudioSourcePool");
        for (int i = 0; i < initialCapacity; i++)
        {
            var audioSource = CreateNewAudioSource();
            pool.Enqueue(audioSource);
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        var obj = new GameObject("PooledAudioSource");
        obj.transform.parent = poolHost.transform;
        return obj.AddComponent<AudioSource>();
    }

    public AudioSource Get()
    {
        if (pool.Count > 0)
        {
            var source = pool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            return CreateNewAudioSource();
        }
    }

    public void Return(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        pool.Enqueue(source);
    }
}
