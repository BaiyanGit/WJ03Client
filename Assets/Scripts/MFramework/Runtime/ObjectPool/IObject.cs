
namespace Wx.Runtime.Pool
{
    public interface IObject
    {
        void OnSpawn(object userData);

        void OnRestore();
    }
}
