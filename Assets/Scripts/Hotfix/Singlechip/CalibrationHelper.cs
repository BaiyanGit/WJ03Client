using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationHelper  {

    /// <summary>
    /// CRC校验
    /// </summary>
    /// <param name="data">校验数据</param>
    /// <returns>高低8位</returns>
    public static byte[] CRCCalc(byte[] data)
    {

        //计算并填写CRC校验码
        int crc = 0xffff;
        int len = data.Length;
        for (int n = 0; n < len; n++)
        {
            byte i;
            crc = crc ^ data[n];
            for (i = 0; i < 8; i++)
            {
                int TT;
                TT = crc & 1;
                crc = crc >> 1;
                crc = crc & 0x7fff;
                if (TT == 1)
                {
                    crc = crc ^ 0xa001;
                }
                crc = crc & 0xffff;
            }

        }
        byte[] returnVal = new byte[2];
        returnVal[0] = (byte)((crc >> 8) & 0xff);
        returnVal[1] = (byte)((crc & 0xff));

        return returnVal;
    }

}

public class ByteHelper
{
    public static bool GetValue(byte value, int location)
    {
        // 检查参数
        if (location < 0 || location > 7)
        {
            return false;
        }
        var result = false;
        switch (location)
        {
            case 0:
                result = (value & 0x01) != 0;
                break;

            case 1:
                result = (value & 0x02) != 0;
                break;

            case 2:
                result = (value & 0x04) != 0;
                break;

            case 3:
                result = (value & 0x08) != 0;
                break;

            case 4:
                result = (value & 0x10) != 0;
                break;

            case 5:
                result = (value & 0x20) != 0;
                break;

            case 6:
                result = (value & 0x40) != 0;
                break;

            case 7:
                result = (value & 0x80) != 0;
                break;
        }
        return result;
    }
}
