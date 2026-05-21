using System;
using UnityEngine;

public static class ProceduralAudio
{
    private const int SAMPLE_RATE = 44100;

    public static AudioClip GenerateTone(float frequency, float duration, float attack = 0.005f, float decay = 0.05f, float volume = 0.5f, WaveType wave = WaveType.Sine)
    {
        int sampleCount = Mathf.RoundToInt(SAMPLE_RATE * duration);
        float[] data = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float env = Envelope(t, duration, attack, decay);
            data[i] = SampleWave(wave, frequency, t) * env * volume;
        }
        return CreateClipFromData("tone_" + frequency + "_" + duration, data);
    }

    public static AudioClip GenerateSweep(float startFreq, float endFreq, float duration, float volume = 0.5f, WaveType wave = WaveType.Sine, bool exp = false)
    {
        int sampleCount = Mathf.RoundToInt(SAMPLE_RATE * duration);
        float[] data = new float[sampleCount];
        float phase = 0f;
        float dt = 1f / SAMPLE_RATE;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float p = t / duration;
            float freq = exp ? Mathf.Lerp(Mathf.Log(startFreq), Mathf.Log(endFreq), p) : 0f;
            float actualFreq = exp ? Mathf.Exp(freq) : Mathf.Lerp(startFreq, endFreq, p);
            phase += actualFreq * dt;
            float env = Envelope(t, duration, 0.005f, 0.05f);
            data[i] = SampleWavePhase(wave, phase) * env * volume;
        }
        return CreateClipFromData("sweep_" + startFreq + "_" + endFreq, data);
    }

    public static AudioClip GenerateNoise(float duration, float volume = 0.4f, float lowPass = 0.5f)
    {
        int sampleCount = Mathf.RoundToInt(SAMPLE_RATE * duration);
        float[] data = new float[sampleCount];
        System.Random rng = new System.Random();
        float lastSample = 0f;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float n = (float)(rng.NextDouble() * 2.0 - 1.0);
            lastSample = lastSample * (1f - lowPass) + n * lowPass;
            float env = Envelope(t, duration, 0.001f, 0.4f);
            data[i] = lastSample * env * volume;
        }
        return CreateClipFromData("noise_" + duration, data);
    }

    public static AudioClip GenerateChord(float[] frequencies, float duration, float volume = 0.35f, WaveType wave = WaveType.Sine)
    {
        int sampleCount = Mathf.RoundToInt(SAMPLE_RATE * duration);
        float[] data = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float s = 0f;
            for (int f = 0; f < frequencies.Length; f++)
            {
                s += SampleWave(wave, frequencies[f], t);
            }
            s /= frequencies.Length;
            float env = Envelope(t, duration, 0.005f, 0.15f);
            data[i] = s * env * volume;
        }
        return CreateClipFromData("chord_" + duration, data);
    }

    public static AudioClip GenerateArpeggio(float[] notes, float perNoteDuration, float volume = 0.4f, WaveType wave = WaveType.Sine)
    {
        int perNoteSamples = Mathf.RoundToInt(SAMPLE_RATE * perNoteDuration);
        int totalSamples = perNoteSamples * notes.Length;
        float[] data = new float[totalSamples];
        for (int n = 0; n < notes.Length; n++)
        {
            for (int i = 0; i < perNoteSamples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float env = Envelope(t, perNoteDuration, 0.005f, 0.04f);
                data[n * perNoteSamples + i] = SampleWave(wave, notes[n], t) * env * volume;
            }
        }
        return CreateClipFromData("arp", data);
    }

    private static float Envelope(float t, float total, float attack, float decay)
    {
        if (t < attack) return t / attack;
        float decayStart = total - decay;
        if (t > decayStart)
        {
            float p = (t - decayStart) / decay;
            return 1f - p;
        }
        return 1f;
    }

    private static float SampleWave(WaveType wave, float freq, float t)
    {
        float phase = freq * t;
        return SampleWavePhase(wave, phase);
    }

    private static float SampleWavePhase(WaveType wave, float phase)
    {
        float p = phase - Mathf.Floor(phase);
        switch (wave)
        {
            case WaveType.Sine: return Mathf.Sin(phase * 2f * Mathf.PI);
            case WaveType.Square: return p < 0.5f ? 1f : -1f;
            case WaveType.Triangle: return p < 0.5f ? -1f + 4f * p : 3f - 4f * p;
            case WaveType.Saw: return 2f * p - 1f;
        }
        return 0f;
    }

    private static AudioClip CreateClipFromData(string name, float[] data)
    {
        AudioClip clip = AudioClip.Create(name, data.Length, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }
}

public enum WaveType { Sine, Square, Triangle, Saw }
