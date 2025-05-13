//登陆
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }
    //客户端发
    public string ICCard = "";
    public string id = "";
    public string pw = "";
    public string type = "";
    public string imagestr="";
    //服务端回（0-成功，1-失败）
    public int result = 0;
    //学生账号和姓名
    public string accountId;
    public string name;
}

/// <summary>
/// 导航进入具体模块 发动机/底盘/电气 模块
/// </summary>
public class MsgNavigationModule : MsgBase
{
    public MsgNavigationModule() { protoName = "MsgNavigationModule"; }

    // 编号 对应 发动机/底盘/电气 模块
    public int id = 0;
    // 说明 具体模块名称
    public string describe = "";
}

/// <summary>
/// 导航到发动机模块
/// </summary>

public class MsgNavEngine:MsgBase
{
    public MsgNavEngine() { protoName = "MsgNavEngine"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}

/// <summary>
/// 导航到发动机模块下的监测点
/// </summary>
public class MsgNavEnginePoint : MsgBase
{
    public MsgNavEnginePoint() { protoName = "MsgNavEnginePoint"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}


/// <summary>
/// 导航到底盘模块
/// </summary>
public class MsgNavChassis : MsgBase
{
    public MsgNavChassis() { protoName = "MsgNavChassis"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}

/// <summary>
/// 导航到底盘模块下的监测点
/// </summary>
public class MsgNavChassisPoint : MsgBase
{
    public MsgNavChassisPoint() { protoName = "MsgNavChassisPoint"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}


/// <summary>
/// 导航到电气模块
/// </summary>
public class MsgNavElectrical : MsgBase
{
    public MsgNavElectrical() { protoName = "MsgNavElectrical"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}

/// <summary>
/// 导航到具体模块下的监测点
/// </summary>
public class MsgNavElectricalPoint : MsgBase
{
    public MsgNavElectricalPoint() { protoName = "MsgNavElectricalPoint"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}



/// <summary>
/// 关闭当前的页面/返回上一级
/// </summary>
public class MsgCloseCurrent:MsgBase
{
    public MsgCloseCurrent() { protoName = "MsgCloseCurrent"; }

    // 编号 对应监测点
    public int id = 0;
    // 说明 具体监测点名称
    public string describe = "";
}

