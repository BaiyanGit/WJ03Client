using UnityEngine;

public class CarSensorsValueTool 
{
    private static readonly CarSensorsValueTool instance = new CarSensorsValueTool();

    private CarSensorsValueTool()
    {

    }

    public static CarSensorsValueTool Instance => instance;

    /// <summary>
    /// 温度-----电阻对照表
    /// </summary>
    private float[] resistances=new float[] 
    {
        1.3f,1.7f,2.2f,2.8f,3.6f,4.6f,5.9f,7.5f,9.5f,12.0f,15.0f,
        19.0f,24.0f,30.0f,38.0f,47.0f,60.0f,75.0f,95.0f,120.0f,150.0f
    };

    /// <summary>
    /// 压力-----电阻对照表
    /// </summary>
    private float[] resistances_ = new float[]{10f,82f,151f,184f};
    private float[] pressures = new float[] { 0f, 200f, 400f, 500f };

    /// <summary>
    /// 获得压力传感器电阻值所在区间范围
    /// </summary>
    /// <param name="pre">电阻</param>
    /// <returns>范围下标</returns>
    private int GetPressuresIndex(float pre)
    {
        int index = -1;
        for (int i = 0; i < resistances_.Length; i++)
        {
            if (resistances_[i] >= pre)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    /// <summary>
    /// 获得电阻值所在区间范围
    /// </summary>
    /// <param name="res">电阻值 欧姆</param>
    /// <returns>范围下标</returns>
    private int GetTemperatureIndex(float res)
    {
        int index = -1;

        for(int i = 0;i<resistances.Length;i++) 
        {
            if (resistances[i] >= res)
            {  
                index = i; 
                break; 
            }
        }

        return index;
    }



    /// <summary>
    /// 根据当前电阻值，计算温度
    /// </summary>
    /// <param name="res">电阻值</param>
    /// <param name="index">电阻区间范围</param>
    /// <returns>温度值</returns>
    private float GetTemperature(float res)
    {
        float temperature = 0.0f;

        int index = GetTemperatureIndex(res);

        if(index == -1)
        {
            return 200f;
        }

        if(index == 0)
        {
            return 0f;
        }

        float minValue = resistances[index-1];
        float maxValue = resistances[index];
        float minMeasure = 0f + 10f * (index - 1);
        float maxMeasure = 0f + 10f * (index);

        temperature = (res - minValue) * (maxMeasure - minMeasure) / (maxValue - minValue) + minMeasure;

        return temperature;
    }

    /// <summary>
    /// 根据当前电阻值，计算温度
    /// </summary>
    /// <param name="res">电阻值</param>
    /// <param name="zoom">电阻值缩放系数</param>
    /// <returns></returns>
    public float GetTemperature(float res,float zoom=1f)
    {
        float temperature = 0.0f;

        res = res * zoom;

        temperature = GetTemperature(res);

        return temperature;
    }

    /// <summary>
    /// 根据电阻值计算压力单位千帕
    /// </summary>
    /// <param name="res">电阻</param>
    /// <returns>压力</returns>
    private float GetPressures(float res)
    {
        float pressure = 0.0f;

        int index = GetPressuresIndex(res);

        if (index == -1)
        {
            return pressures[pressures.Length-1];
        }
        
        if (index == 0)
        {
            return pressures[0];
        }


        float minValue = resistances_[index - 1];
        float maxValue = resistances_[index];
        float minMeasure = pressures[index - 1];
        float maxMeasure = pressures[index];

        pressure = (res - minValue) * (maxMeasure - minMeasure) / (maxValue - minValue) + minMeasure;

        return pressure;
    }

    /// <summary>
    /// 根据电阻值计算压力单位千帕
    /// </summary>
    /// <param name="res">电阻</param>
    /// <param name="zoom">系数</param>
    /// <returns>压力(千帕)</returns>
    public float GetPressures(float res, float zoom = 1f)
    {
        float pressure = 0.0f;

        res = res * zoom;

        pressure = GetPressures(res);

        return pressure;
    }
}
