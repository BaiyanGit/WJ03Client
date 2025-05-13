using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorRange
{
    public int MaxValue;
    public int MinValue;
    public int MaxMeasure;
    public int MinMeasure;
}


public class SensorRangeInfos
{
    public List<SensorRange> ResistanceSensor;
    public List<SensorRange> VoltageSensor;
    public List<SensorRange> FrequencySensor;
}

