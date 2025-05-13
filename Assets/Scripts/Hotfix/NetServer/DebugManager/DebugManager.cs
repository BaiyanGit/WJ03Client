using System.Collections.Generic;
using UnityEngine;

namespace DebugManager
{
    using System;

    /// <summary>
    /// 车辆类型
    /// </summary>
    public enum MachineType
    {
        /// <summary>
        /// 叉车
        /// </summary>
        Loader = 1058,

        /// <summary>
        /// 吊车
        /// </summary>
        WheelCrane = 1052,

        /// <summary>
        /// 牵引车
        /// </summary>
        AircraftTractor = 1051
    }

    public class LogManager
    {
        private static LogManager _ins;
        public static LogManager Ins; //=> _ins ?? (_ins = new LogManager());

        public LogManager()
        {
            _ins = this;
        }

        private readonly LoaderSignalData _loaderSignalData = new LoaderSignalData();
        private readonly AircraftTractorSignalData _aircraftTractorSignalData = new AircraftTractorSignalData();
        private readonly WheelCraneSignalData _wheelCraneSignalData = new WheelCraneSignalData();

        ///// <summary>
        ///// 打印接收消息
        ///// </summary>
        ///// <param name="player"></param>
        ///// <param name="listData"></param>
        //public void PrintReceive(Player player, List<DicFloatValue> listData)
        //{
        //    var playerId = player.id; //用户ID
        //    var machineId = player.machineTypeId; //用户车辆ID
        //    var isOwner = player.isOwner; //是否房主
        //    var structData = StructGroup((MachineType)machineId, listData);
        //    var str =
        //        $"<color=green>玩家ID：</color>{playerId}  <color=green>车辆ID：</color>{machineId}  <color=green>是否房主：</color>{isOwner} \n{structData}";
        //    Debug.Log(str);
        //}

        /// <summary>
        /// 打印相应消息体
        /// </summary>
        /// <param name="machineType"></param>
        /// <param name="listData"></param>
        //private string StructGroup(MachineType machineType, List<DicFloatValue> listData)
        //{
        //    string str;
        //    switch (machineType)
        //    {
        //        case MachineType.Loader:
        //            str = _loaderSignalData.GetKeyDesc(listData);
        //            break;
        //        case MachineType.AircraftTractor:
        //            str = _aircraftTractorSignalData.GetKeyDesc(listData);
        //            break;
        //        case MachineType.WheelCrane:
        //            str = _wheelCraneSignalData.GetKeyDesc(listData);
        //            break;
        //        default:
        //            str = "<color=red>异常的车辆类型</color>";
        //            throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
        //    }

        //    return str;
        //}
    }

    public class SignalDataBase
    {
        /// <summary>
        /// 指令描述字典
        /// </summary>
        protected Dictionary<int, string> keyDescDictionary = new Dictionary<int, string>();

        /// <summary>
        /// 获取指令描述
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        //public string GetKeyDesc(List<DicFloatValue> listData)
        //{
        //    var desc = string.Empty;
        //    foreach (var data in listData)
        //    {
        //        var key = data.index;
        //        var value = data.value;
        //        desc += $"<color=red>Key：</color>{GetKeyDesc(key)}  <color=yellow>Value：</color>{value} \n";
        //    }

        //    return desc;
        //}

        /// <summary>
        /// 获取指令描述
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKeyDesc(int key)
        {
            return keyDescDictionary.TryGetValue(key, out var value) ? value : $"{key} 未找到对应的指令描述";
        }
    }

    /// <summary>
    /// 装载机指令
    /// </summary>
    public class LoaderSignalData : SignalDataBase
    {
        private const int Power = 0; //电源
        private const int Ignition = 1; //点火|1|
        private const int Steering = 22 + 10000; //方向盘|22|
        private const int Throttle = 16 + 10000; //油门|16|
        private const int Brake = 18 + 10000; //刹车|18|
        private const int Handbrake = 10; //手刹|10|
        private const int Horn = 15; //喇叭|15|
        private const int Gear1 = 6; //F1档|6|
        private const int Gear2 = 7; //F2档|7|
        private const int GearR = 9; //R档|9|
        private const int LeftLight = 2; //左转向灯|2|
        private const int RightLight = 3; //右转向灯|3|
        private const int HighLight = 5; //远光灯|5|
        private const int LowLight = 4; //近光灯|4|
        private const int WarningLight = 12; //双闪|12|
        private const int CheckBody = 13; //检查车身|13|
        private const int SafeLine = 14; //安全带|14|
        private const int ForkIn = 16; //向内|16|
        private const int ForkOut = 17; //向外|17|
        private const int RightAbout = 6 + 10000; //右手柄左右|6|
        private const int RightAround = 8 + 10000; //右手柄前后|8|

        public LoaderSignalData()
        {
            keyDescDictionary = new Dictionary<int, string>
            {
                { Power, "电源" },
                { Ignition, "点火" },
                { Steering, "方向盘" },
                { Throttle, "油门" },
                { Brake, "刹车" },
                { Handbrake, "手刹" },
                { Horn, "喇叭" },
                { Gear1, "F1档" },
                { Gear2, "F2档" },
                { GearR, "R档" },
                { LeftLight, "左转向灯" },
                { RightLight, "右转向灯" },
                { HighLight, "远光灯" },
                { LowLight, "近光灯" },
                { WarningLight, "双闪" },
                { CheckBody, "检查车身" },
                { SafeLine, "安全带" },
                { ForkIn, "向内" },
                { ForkOut, "向外" },
                { RightAbout, "右手柄左右" },
                { RightAround, "右手柄前后" }
            };
        }
    }

    /// <summary>
    /// 牵引车指令
    /// </summary>
    public class AircraftTractorSignalData : SignalDataBase
    {
        private const int PowerA = 0; //电源开关|0|
        private const int Ignition = 1; //点火|1|
        private const int SteeringA = 22 + 10000; //方向盘A|22|
        private const int ThrottleA = 16 + 10000; //油门A|16|
        private const int BrakeA = 17; //刹车A|17|
        private const int HandbrakeA = 15; //手刹|15|
        private const int HornA = 24; //喇叭|24|
        private const int LeftLightA = 10; //左转向灯|10|
        private const int RightLightA = 11; //右转向灯|11|
        private const int WarningLightA = 31; //示廓灯|31|
        private const int LowLightA = 12; //近光灯|12|
        private const int RearSteeringA = 13; //后转向|13|
        private const int SteeringLimitA = 16; //转向限位|16|
        private const int GlideA = 17; //滑行|17|
        private const int HeadstockUpA = 22; //车头升高|22|
        private const int HeadstockDownA = 23; //车头降落|23|
        private const int HighGearA = 18; //高档|18|
        private const int LowGearA = 19; //低档|19|
        private const int GearForwardA = 20; //前进档|20|
        private const int GearBackA = 21; //后退档|21|
        private const int EmergencyStopB = 116; //操作台急停|116|
        private const int HoldWheelUpB = 100; //抱轮上升|100|
        private const int HoldWheelDownB = 102; //抱轮下降|102|
        private const int CompactB = 101; //压紧|101|
        private const int ReleaseB = 103; //松开|103|
        private const int WorktopGearForwardB = 106; //操作台前进挡|106|
        private const int WorktopGearBacB = 105; //操作台后退挡|105|
        private const int WorktopSteeringB = 122 + 10000; //方向盘A|122|
        private const int WorktopThrottleB = 116 + 10000; //油门A|116|
        private const int WorktopBrakeB = 107; //刹车A|107|
        private const int BackBigLightB = 104; //后照灯|104|

        public AircraftTractorSignalData()
        {
            keyDescDictionary = new Dictionary<int, string>
            {
                { PowerA, "电源开关" },
                { Ignition, "点火" },
                { SteeringA, "方向盘A" },
                { ThrottleA, "油门A" },
                { BrakeA, "刹车A" },
                { HandbrakeA, "手刹" },
                { HornA, "喇叭" },
                { LeftLightA, "左转向灯" },
                { RightLightA, "右转向灯" },
                { WarningLightA, "示廓灯" },
                { LowLightA, "近光灯" },
                { RearSteeringA, "后转向" },
                { SteeringLimitA, "转向限位" },
                { GlideA, "滑行" },
                { HeadstockUpA, "车头升高" },
                { HeadstockDownA, "车头降落" },
                { HighGearA, "高档" },
                { LowGearA, "低档" },
                { GearForwardA, "前进档" },
                { GearBackA, "后退档" },
                { EmergencyStopB, "操作台急停" },
                { HoldWheelUpB, "抱轮上升" },
                { HoldWheelDownB, "抱轮下降" },
                { CompactB, "压紧" },
                { ReleaseB, "松开" },
                { WorktopGearForwardB, "操作台前进挡" },
                { WorktopGearBacB, "操作台后退挡" },
                { WorktopSteeringB, "方向盘A" },
                { WorktopThrottleB, "油门A" },
                { WorktopBrakeB, "刹车A" },
                { BackBigLightB, "后照灯" }
            };
        }
    }

    /// <summary>
    /// 吊车指令
    /// </summary>
    public class WheelCraneSignalData : SignalDataBase
    {
        private const int LFZhiTuiH = 106; //左前支腿水平|106|
        private const int LFZhiTuiV = 107; //左前支腿垂直|107|
        private const int RfZhiTuiH = 108; //右前支腿水平|108|
        private const int RfZhiTuiV = 109; //右前支腿垂直|109|
        private const int LBZhiTuiH = 110; //左后支腿水平|110|
        private const int LBZhiTuiV = 111; //左后支腿垂直|111|
        private const int RbZhiTuiH = 112; //右后支腿水平|112|
        private const int RbZhiTuiV = 113; //右后支腿垂直|113|
        private const int ZhiTuiShen = 114; //支腿伸出|114|
        private const int ZhiTuiSuo = 115; //支腿缩回|115|
        private const int ZhiTuiPower = 8; //支腿油门电源开关|8|
        private const int ZhiTuiLock = 4; //支腿锁|4|
        private const int ZhiTui = 106 + 10000; //支腿油门|106|
        private const int PowerDown = 118; //下车电源|118|
        private const int IgnitionDown = 119; //下车打火|119|
        private const int EngineStopDown = 1010; //下车熄火|1010|
        private const int SteeringDown = 1022; //下车方向盘|1022|
        private const int ThrottleDown = 1016; //下车油门|1016|
        private const int BrakeDown = 1017 + 10000; //下车刹车|1017|
        private const int HandleBrakeDown = 1009; //下车手刹|1009|
        private const int ClutchDown = 1018 + 10000; //下车离合|1018|
        private const int Gear1 = 1024; //1档|1024|
        private const int Gear2 = 1023; //2档|1023|
        private const int Gear3 = 1026; //3档|1026|
        private const int Gear4 = 1025; //4档|1025|
        private const int Gear5 = 1031; //5档|1031|
        private const int GearR = 1027; //R档|1027|
        private const int QuLi = 1011; //取力开关|1011|
        private const int HornDown = 1014; //下车喇叭|1014|
        private const int SafeLine = 2; //安全带|2|
        private const int LowLight = 1002; //近光灯|1002|
        private const int BigLight = 1003; //远光灯|1003|
        private const int LeftLightA = 1000; //左转向灯|1000|
        private const int RightLightA = 1001; //右转向灯|1001|
        private const int RotateSite = 100 + 10000; //转台旋转|100|
        private const int ArmBigOut = 101 + 10000; //大臂伸缩|101|
        private const int ArmUp = 102 + 10000; //大臂升降|102|
        private const int DiaoGouUp = 103 + 10000; //吊钩升降|103|
        private const int RotateLock = 103; //回转机构制动器|103|
        private const int EmergencySwitch = 117; //应急开关|117|
        private const int HornUp = 100; //上车喇叭|100|
        private const int PowerUp = 0; //上车电源|0|
        private const int IgnitionUp = 1; //上车打火|1|
        private const int EngineStopUp = 3; //上车熄火|3|
        private const int BrakeUp = 117 + 10000; //上车刹车|117|
        private const int CheckBody = 101; //检查车身|101|
        private const int GoodsBangDing = 104; //挂起解绑货物|104|
        private const int UseFuJuan = 116; //使用复卷|116|
        private const int ThrottleUp = 116; //上车油门|116|

        public WheelCraneSignalData()
        {
            keyDescDictionary = new Dictionary<int, string>
            {
                { LFZhiTuiH, "左前支腿水平" },
                { LFZhiTuiV, "左前支腿垂直" },
                { RfZhiTuiH, "右前支腿水平" },
                { RfZhiTuiV, "右前支腿垂直" },
                { LBZhiTuiH, "左后支腿水平" },
                { LBZhiTuiV, "左后支腿垂直" },
                { RbZhiTuiH, "右后支腿水平" },
                { RbZhiTuiV, "右后支腿垂直" },
                { ZhiTuiShen, "支腿伸出" },
                { ZhiTuiSuo, "支腿缩回" },
                { ZhiTuiPower, "支腿油门电源开关" },
                { ZhiTuiLock, "支腿锁" },
                { ZhiTui, "支腿油门" },
                { PowerDown, "下车电源" },
                { IgnitionDown, "下车打火" },
                { EngineStopDown, "下车熄火" },
                { SteeringDown, "下车方向盘" },
                { ThrottleDown, "下车油门" },
                { BrakeDown, "下车刹车" },
                { HandleBrakeDown, "下车手刹" },
                { ClutchDown, "下车离合" },
                { Gear1, "1档" },
                { Gear2, "2档" },
                { Gear3, "3档" },
                { Gear4, "4档" },
                { Gear5, "5档" },
                { GearR, "R档" },
                { QuLi, "取力开关" },
                { HornDown, "下车喇叭" },
                { SafeLine, "安全带" },
                { LowLight, "近光灯" },
                { BigLight, "远光灯" },
                { LeftLightA, "左转向灯" },
                { RightLightA, "右转向灯" },
                { RotateSite, "转台旋转" },
                { ArmBigOut, "大臂伸缩" },
                { ArmUp, "大臂升降" },
                { DiaoGouUp, "吊钩升降" },
                { RotateLock, "回转机构制动器" },
                { EmergencySwitch, "应急开关" },
                { HornUp, "上车喇叭" },
                { PowerUp, "上车电源" },
                { IgnitionUp, "上车打火" },
                { EngineStopUp, "上车熄火" },
                { BrakeUp, "上车刹车" },
                { CheckBody, "检查车身" },
                { GoodsBangDing, "挂起解绑货物" },
                { UseFuJuan, "使用复卷" },
                { ThrottleUp, "上车油门" }
            };
        }
    }
}