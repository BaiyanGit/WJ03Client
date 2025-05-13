using UnityEngine;


public class AudioWaveTexture : MonoBehaviour
{
    public static AudioWaveTexture _instance;

    private int textureWidth = 386;      // �����ܿ��
    private int textureHeight = 120;     // ����߶�
    private float amplitudeScale = 1f; // �������ϵ��
    public Color waveformColor = Color.cyan;
    public int barWidth = 3;            // ��������ȣ���λ�������У�
    public int gapWidth = 1;            // ��϶��ȣ���λ�������У�

    private Texture2D waveformTexture;
    private Texture2D defaultTexture;
    private Color[] pixels;
    private int centerY;
    private int totalBlockWidth;        // ÿ������ܿ�ȣ�Bar + Gap��

    public Color centerLineColor = Color.white; // ��������������ɫ
    [Range(0, 1)] public float centerLineAlpha = 0.8f; // ������͸����

    bool _stopGetValue = true;
    float waittime;
    bool _currentDefaultValue;
    bool _isFake;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        defaultTexture = waveformTexture;
    }

    private void Update()
    {
        waittime += Time.deltaTime;

        if (_stopGetValue || waittime < .044f) return;

        float currentVolume;
        if (_isFake)
        {
            currentVolume = UnityEngine.Random.Range(20, 80);
            if (_currentDefaultValue)
            {
                currentVolume += 20;
                //Debug.Log("��������");
            }
        }
        else
        {
            currentVolume = NoiseSensorControl.Instance.GetNoiseValue();
        }
        
        float num = currentVolume / 120;
        
        waittime = 0;
        UpdateWaveform(num);

    }


    public void UpdateWaveform(float amplitude)
    {
        // 1. �����������ο飨Bar + Gap��
        ShiftPixelsLeft(totalBlockWidth);

        // 2. ���Ҳ������²�����
        float scaledAmplitude = Mathf.Clamp01(amplitude * amplitudeScale);
        //Debug.Log(scaledAmplitude);
        int waveHalfHeight = Mathf.FloorToInt(scaledAmplitude * centerY);

        // ���Bar�����Ҳ�� barWidth �У�
        for (int x = waveformTexture.width - barWidth; x < waveformTexture.width; x++)
        {
            for (int y = 0; y < waveformTexture.height; y++)
            {
                bool isInRange = (y >= centerY - waveHalfHeight && y <= centerY + waveHalfHeight);
                if (isInRange)
                {
                    float distance = Mathf.Abs(y - centerY);
                    float alpha = Mathf.Clamp01(1 - distance / (float)waveHalfHeight);
                    pixels[y * waveformTexture.width + x] = waveformColor * new Color(1, 1, 1, alpha);
                }
                else
                {
                    pixels[y * waveformTexture.width + x] = Color.clear; // �ǲ�������͸��
                }
            }
        }

        // 3. �������е������л��������ߣ��������֣�
        DrawCenterLine();

        waveformTexture.SetPixels(pixels);
        waveformTexture.Apply();
    }

    // ��Ч�������أ����������ز�����
    private void ShiftPixelsLeft(int shiftWidth)
    {
        for (int y = 0; y < waveformTexture.height; y++)
        {
            int rowStart = y * waveformTexture.width;
            System.Array.Copy(pixels, rowStart + shiftWidth, pixels, rowStart, waveformTexture.width - shiftWidth);
        }
        // ����Ҳ�ճ�������
        for (int x = waveformTexture.width - shiftWidth; x < waveformTexture.width; x++)
        {
            for (int y = 0; y < waveformTexture.height; y++)
            {
                pixels[y * waveformTexture.width + x] = Color.clear;
            }
        }
    }

    private void DrawCenterLine()
    {
        Color lineColor = new Color(
            centerLineColor.r,
            centerLineColor.g,
            centerLineColor.b,
            centerLineAlpha
        );

        for (int x = 0; x < waveformTexture.width; x++)
        {
            // ����ģʽ��ֱ��������ɫ���Ḳ�ǲ��Σ�
            pixels[centerY * waveformTexture.width + x] = lineColor;

            // ���ģʽ������������ɫ����ȡ��ע�����´���
            Color current = pixels[centerY * waveformTexture.width + x];
            pixels[centerY * waveformTexture.width + x] = Color.Lerp(current, lineColor, lineColor.a);
        }
    }

    private void ClearPixels()
    {
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
    }

    public Texture2D GetTexture(Texture2D texture, bool value, bool useFake = true)
    {
        waveformTexture = texture;
        _stopGetValue = false;
        _currentDefaultValue = value;
        _isFake = useFake;

        centerY = waveformTexture.height / 2;
        totalBlockWidth = barWidth + gapWidth; // ���ȼ���
        waveformTexture.wrapMode = TextureWrapMode.Clamp;
        pixels = new Color[waveformTexture.width * waveformTexture.height];
        ClearPixels();

        return waveformTexture;
    }

    public void SetDefaultTexture()
    {
        //��ֹ���ݲɼ�����
        ClearPixels();
        _stopGetValue = true;
        waveformTexture = defaultTexture;
    }
}