
namespace Wx.Runtime.Machine
{
    public interface IStateNode
    {
        void OnCreate(WMachine machine);
        
        void OnEnter(params object[] datas);

        void OnUpdate();

        void OnFixedUpdate();

        void OnLateUpdate();

        void OnExit();

        void OnDestroy();
    }
}
