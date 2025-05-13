
namespace Wx.Runtime.Singleton
{
	public interface ISingleton
	{
		/// <summary>
		/// 创建单例
		/// </summary>
		void OnCreate(System.Object createParam);

		/// <summary>
		/// 更新单例
		/// </summary>
		void OnUpdate();

		void OnFixedUpdate();

		void OnLateUpdate();

		/// <summary>
		/// 销毁单例
		/// </summary>
		void OnDestroy();
	}
}