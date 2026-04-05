using System.Security.Policy;
using System.Windows;

[assembly:ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
//参数 1：主题特定资源字典位置
//None 不使用主题特定资源	不查找 Themes\*.xaml 文件
//SourceAssembly	在当前程序集中查找	查找 Themes\Generic.xaml
//ExternalAssembly	在其他程序集中查找	用于自定义控件库
// 参数 2：通用资源字典位置
// SourceAssembly	在当前程序集中查找	查找本程序集的资源字典
//ExternalAssembly	在其他程序集中查找	用于跨程序集共享资源
//资源查找流程
//WPF 需要资源时
//    ↓
//1. 查找当前窗口/控件资源
//    ↓ 未找到
//2. 查找 App.xaml 全局资源
//    ↓ 未找到
//3. 根据 ThemeInfo 配置查找
//    ↓ 未找到
//4. 抛出资源未找到异常