using UnityEngine;
using UnityEngine.UI;

public class TestVoice : MonoBehaviour
{
    public AudioWaveTexture aiduo;
    public RawImage rawImage;
    float waittime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 示例：获取音量并更新波形
        aiduo = GetComponent<AudioWaveTexture>();
    }

    // Update is called once per frame
    void Update()
    {
        waittime += Time.deltaTime;
        if (waittime > 0.2)
        {
            float currentVolume = UnityEngine.Random.Range(10, 80);
            float num = currentVolume / 80;
            //Debug.Log(currentVolume / 80);
            aiduo.UpdateWaveform(num);
            //rawImage.texture = aiduo.GetTexture();
            waittime = 0;
        }
        
    }
}
