using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfXamlPlayground.ViewModels;

namespace WpfXamlPlayground;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();
        //创建 ViewModel 实例（数据层）
        viewModel = new MainViewModel(); //设置数据上下文
         //设置数据上下文（连接 UI 与数据）
        DataContext = viewModel;  //设置数据上下文
        //继承链：
        //DependencyObject
        //    └── UIElement
        //        └── FrameworkElement  ← DataContext 定义在这里
        //            ├── Window        ← 您的 MainWindow 继承自 Window
        //            ├── UserControl
        //            ├── Grid
        //            ├── Button
        //            ├── TextBox
        //            └── 所有 WPF 控件...
        //所以 DataContext 是以下所有类的属性：
        //DataContext 是什么？	一个属性，存储绑定的默认数据源
        //DataContext 是谁的？	FrameworkElement 及其所有子类的属性

    }
    //两种Binding方式：
    //    ✅ 方式 1：使用 DataContext（最常见）
    //    <!-- Window 或 UserControl 设置 DataContext -->
    //    <Window DataContext = "{StaticResource MyViewModel}" >
    //        < !--绑定直接写属性名，自动从 DataContext 查找 -->
    //        <TextBox Text = "{Binding NoteText}" />
    //        < TextBlock Text="{Binding ReceiverA}"/>
    //    </Window>
    //  ✅ 方式 2：使用 ElementName（您项目中的进度条）
    //      <!-- 没有 DataContext，指定具体控件 -->
    //<ProgressBar x:Name="LearningProgress" Value="30"/>
    //<TextBlock Text = "{Binding ElementName=LearningProgress, Path=Value}" />

    // <!-- 这些绑定没有 ElementName，说明使用了 DataContext -->
    //<TextBox Text = "{Binding NoteText, Mode=TwoWay}" />
    //< Button Command="{Binding AddTaskCommand}"/>
    //<TextBlock Text = "{Binding ReceiverA}" />
    //< TextBlock Text="{Binding ReceiverB}"/>

    //5. DataContext 继承机制
    //    <Window DataContext="{StaticResource ViewModel}">
    //        <Grid>  <!-- 自动继承 Window 的 DataContext -->
    //            <StackPanel>  <!-- 自动继承 Grid 的 DataContext -->
    //                <TextBox Text = "{Binding NoteText}" />  < !--继承 StackPanel 的 DataContext -->
    //            </StackPanel>
    //        </Grid>
    //    </Window>

    /// <summary>
    /// Switch theme
    /// 鼠标移到按钮上，点击切换主题
    /// </summary>
    private void OnSwitchThemeClick(object sender, RoutedEventArgs e)
    {
        viewModel.IsPrimaryTheme = !viewModel.IsPrimaryTheme;//切换主题状态
        var key = "Brush.Primary";//定义资源健名，指定应用程序资源字典中需要更新的颜色资源键名为 "Brush.Primary"
        var targetColor = viewModel.IsPrimaryTheme ? Color.FromRgb(58, 122, 254) : Color.FromRgb(0, 168, 143);
        //使用三元表达式选择主题颜色
        //3.	选择目标颜色
        Application.Current.Resources[key] = new SolidColorBrush(targetColor);
        //•	将新创建的纯色画刷赋值给全局应用程序资源。
       // •	这会立即触发 WPF 的资源系统刷新，使界面上所有绑定该资源的元素颜色发生变更
    }
}