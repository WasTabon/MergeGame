using UnityEngine;

public class SfxLibrary : MonoBehaviour
{
    public static SfxLibrary Instance { get; private set; }

    public AudioClip uiClick;
    public AudioClip shopBuy;
    public AudioClip pickaxePickup;
    public AudioClip pickaxeDrop;
    public AudioClip merge;
    public AudioClip miningHit;
    public AudioClip blockExplode;
    public AudioClip coinTick;
    public AudioClip zoneComplete;
    public AudioClip chestOpen;
    public AudioClip boosterActivate;
    public AudioClip achievementUnlock;
    public AudioClip dailyReward;
    public AudioClip popupOpen;
    public AudioClip popupClose;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GenerateAll();
    }

    private void GenerateAll()
    {
        uiClick = ProceduralAudio.GenerateTone(880f, 0.05f, 0.001f, 0.04f, 0.35f, WaveType.Square);
        shopBuy = ProceduralAudio.GenerateArpeggio(new float[] { 523f, 659f, 784f }, 0.06f, 0.4f, WaveType.Square);
        pickaxePickup = ProceduralAudio.GenerateTone(660f, 0.08f, 0.001f, 0.07f, 0.3f, WaveType.Triangle);
        pickaxeDrop = ProceduralAudio.GenerateTone(440f, 0.08f, 0.001f, 0.07f, 0.3f, WaveType.Triangle);
        merge = ProceduralAudio.GenerateArpeggio(new float[] { 523f, 659f, 784f, 1046f }, 0.05f, 0.45f, WaveType.Square);
        miningHit = ProceduralAudio.GenerateNoise(0.06f, 0.25f, 0.6f);
        blockExplode = ProceduralAudio.GenerateNoise(0.25f, 0.5f, 0.3f);
        coinTick = ProceduralAudio.GenerateTone(1320f, 0.04f, 0.001f, 0.03f, 0.25f, WaveType.Sine);
        zoneComplete = ProceduralAudio.GenerateArpeggio(new float[] { 523f, 659f, 784f, 1046f, 1318f }, 0.08f, 0.5f, WaveType.Square);
        chestOpen = ProceduralAudio.GenerateSweep(220f, 880f, 0.5f, 0.45f, WaveType.Square, true);
        boosterActivate = ProceduralAudio.GenerateSweep(440f, 1760f, 0.3f, 0.4f, WaveType.Saw, true);
        achievementUnlock = ProceduralAudio.GenerateArpeggio(new float[] { 659f, 784f, 988f, 1318f }, 0.07f, 0.45f, WaveType.Triangle);
        dailyReward = ProceduralAudio.GenerateArpeggio(new float[] { 523f, 659f, 784f, 1046f, 1318f, 1568f }, 0.08f, 0.5f, WaveType.Triangle);
        popupOpen = ProceduralAudio.GenerateSweep(440f, 880f, 0.15f, 0.3f, WaveType.Sine, false);
        popupClose = ProceduralAudio.GenerateSweep(880f, 440f, 0.12f, 0.25f, WaveType.Sine, false);
    }

    public void Play(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (SoundManager.Instance != null && clip != null) SoundManager.Instance.PlaySFX(clip, volumeScale, pitch);
    }
}
