using UnityEngine;


public class AudioWaveTexture : MonoBehaviour
{
    public static AudioWaveTexture _instance;

    private int textureWidth = 386;      // 纹理总宽度
    private int textureHeight = 120;     // 纹理高度
    private float amplitudeScale = 1f; // 振幅缩放系数
    public Color waveformColor = Color.cyan;
    public int barWidth = 3;            // 波形条宽度（单位：像素列）
    public int gapWidth = 1;            // 间隙宽度（单位：像素列）

    private Texture2D waveformTexture;
    private Texture2D defaultTexture;
    private Color[] pixels;
    private int centerY;
    private int totalBlockWidth;        // 每个块的总宽度（Bar + Gap）

    public Color centerLineColor = Color.white; // 新增：中轴线颜色
    [Range(0, 1)] public float centerLineAlpha = 0.8f; // 新增：透明度

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
                //Debug.Log("错误数据");
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
        // 1. 左移整个波形块（Bar + Gap）
        ShiftPixelsLeft(totalBlockWidth);

        // 2. 在右侧生成新波形条
        float scaledAmplitude = Mathf.Clamp01(amplitude * amplitudeScale);
        //Debug.Log(scaledAmplitude);
        int waveHalfHeight = Mathf.FloorToInt(scaledAmplitude * centerY);

        // 填充Bar区域（右侧的 barWidth 列）
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
                    pixels[y * waveformTexture.width + x] = Color.clear; // 非波形区域透明
                }
            }
        }

        // 3. 在所有列的中心行绘制中轴线（新增部分）
        DrawCenterLine();

        waveformTexture.SetPixels(pixels);
        waveformTexture.Apply();
    }

    // 高效左移像素（无需逐像素操作）
    private void ShiftPixelsLeft(int shiftWidth)
    {
        for (int y = 0; y < waveformTexture.height; y++)
        {
            int rowStart = y * waveformTexture.width;
            System.Array.Copy(pixels, rowStart + shiftWidth, pixels, rowStart, waveformTexture.width - shiftWidth);
        }
        // 清空右侧空出的区域
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
            // 覆盖模式：直接设置颜色（会覆盖波形）
            pixels[centerY * waveformTexture.width + x] = lineColor;

            // 混合模式（保留波形颜色）：取消注释以下代码
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
        totalBlockWidth = barWidth + gapWidth; // 块宽度计算
        waveformTexture.wrapMode = TextureWrapMode.Clamp;
        pixels = new Color[waveformTexture.width * waveformTexture.height];
        ClearPixels();

        return waveformTexture;
    }

    public void SetDefaultTexture()
    {
        //防止数据采集过量
        ClearPixels();
        _stopGetValue = true;
        waveformTexture = defaultTexture;
    }
}