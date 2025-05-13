using System.Collections.Generic;

/// <summary>
/// 心跳包
/// </summary>
public class MsgHeartbeatData : MsgBase
{
    public MsgHeartbeatData() => protoName = nameof(MsgHeartbeatData);
}

/// <summary>
/// 通用单个按钮操作数据
/// </summary>
public class MsgCommonBtnOperationData : MsgBase
{
    public MsgCommonBtnOperationData() => protoName = nameof(MsgCommonBtnOperationData);
    public int btnType; // 按钮操作类型，1:首页 2:信号切换 3:考核 4:实训成绩提交 5:返回上页 6:退出应用
}

/// <summary>
/// 页面UI导航消息
/// </summary>
public class MsgUINavigationData : MsgBase
{
    public MsgUINavigationData() => protoName = nameof(MsgUINavigationData);

    public List<int> uiLevel; // UI进入的深度层级
    public int uiAreaType = 0; // 点击区域类型，0上部UI、1下部分UI
    public int optionIndex = 0; // 当前列表项选择的索引
}

/// <summary>
/// UI输入用户信息
/// </summary>
public class MsgInputUserInfoData : MsgBase
{
    public MsgInputUserInfoData() => protoName = nameof(MsgInputUserInfoData);

    public string userName; // 用户名
    public string userNum; // 用户编号
    public string userEvaluation; // 用户评价
    public string userScore; // 用户评分
}

/// <summary>
/// UI设置结算信息
/// </summary>
public class MsgUISettlementInfoData : MsgBase
{
    public MsgUISettlementInfoData() => protoName = nameof(MsgUISettlementInfoData);

    public string totalTime; // 总用时
    public string totalScore; // 总分数
    public List<MsgUISettlementDetailData> detailList; // 结算详情列表
}

/// <summary>
/// 结算Item数据
/// </summary>
public class MsgUISettlementDetailData : MsgBase
{
    public MsgUISettlementDetailData() => protoName = nameof(MsgUISettlementDetailData);
    public string subScoreType; // 扣分类型
    public string subScoreReason; // 扣分原因
    public string subScoreValue; // 扣分数值
}


/// <summary>
/// UI导航轨迹消息
/// </summary>
public class MsgTrackUINavigationData : MsgBase
{
    public MsgTrackUINavigationData() => protoName = nameof(MsgTrackUINavigationData);

    public List<NavigationData> navigationDataList; // 导航数据列表
}

public struct NavigationData
{
    public int pageId; // 页面Id
    public int optionId; // 选项Id

    public NavigationData(int pageId, int optionId)
    {
        this.pageId = pageId;
        this.optionId = optionId;
    }
}

/// <summary>
/// 模型操作(包含toggle开关)
/// </summary>
public class MsgOperationData : MsgBase
{
    public MsgOperationData() => protoName = nameof(MsgOperationData);

    public int btnType; // 按钮操作类型，1:左移 2:右移 3:上移 4:下移 5:左转 6:右转 7:上转 8:下转 9：考核
    public bool state; // 按钮状态
}

/// <summary>
/// 请求UI导航消息
/// </summary>
public class MsgRequestUINavigationData : MsgBase
{
    public MsgRequestUINavigationData() => protoName = nameof(MsgRequestUINavigationData);
}

/// <summary>
/// 监视器切换
/// </summary>
public class MsgMonitorSwitchData : MsgBase
{
    public MsgMonitorSwitchData() => protoName = nameof(MsgMonitorSwitchData);
    public int monitorIndex; // 监视器索引
}