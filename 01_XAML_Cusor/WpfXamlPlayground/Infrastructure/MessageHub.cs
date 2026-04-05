namespace WpfXamlPlayground.Infrastructure;

public static class MessageHub
{
    // 定义事件:Action<string> 是 .NET 内置委托,接收一个 string 参数
    public static event Action<string>? MessagePublished;

    // 发布方法:向所有订阅者发送消息
    public static void Publish(string message)
    {
        MessagePublished?.Invoke(message);
        // ↑ 空条件运算符:如果没有订阅者,不会报错
    }
}
// 🔑 1. 为什么用 static?
// 确保全局只有一个消息中心
// 任何类都可以访问,无需依赖注入
// 适合小型应用的简单场景
