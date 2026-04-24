我已经将你提供的文档内容整理并转换为标准的 Markdown 格式。这份文档结构清晰，涵盖了 WPF 开发的十大核心模块。

以下是整理后的完整 Markdown 源码，你可以直接复制保存为 .md 文件：
# WPF 核心技术栈完整指南

> 基于 WpfXamlPlayground 项目实战总结

---

## 📚 目录

1. [MVVM 架构模式](#1-mvvm-架构模式)
2. [数据绑定系统](#2-数据绑定系统)
3. [依赖属性与附加属性](#3-依赖属性与附加属性)
4. [资源与样式系统](#4-资源与样式系统)
5. [模板系统](#5-模板系统)
6. [命令系统](#6-命令系统)
7. [触发器与动画](#7-触发器与动画)
8. [集合视图与分组](#8-集合视图与分组)
9. [值转换器](#9-值转换器)
10. [自定义控件](#10-自定义控件)

---

## 1. MVVM 架构模式

### 核心概念

MVVM（Model-View-ViewModel）是 WPF 的标准架构模式，实现 UI 与业务逻辑分离。


┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│    Model     │◄────│  ViewModel   │◄────│     View     │
│  (数据层)     │     │  (逻辑层)     │     │  (界面层)     │
└──────────────┘     └──────────────┘     └──────────────┘


### 项目结构


WpfXamlPlayground/
├── Models/              # 数据模型
│   └── TaskItem.cs
├── ViewModels/          # 视图模型
│   └── MainViewModel.cs
├── Views/               # 视图（XAML）
│   └── MainWindow.xaml
├── Infrastructure/      # 基础设施
│   ├── RelayCommand.cs  # 命令实现
│   └── MessageHub.cs    # 消息中心
├── Converters/          # 值转换器
├── Controls/            # 自定义控件
└── Helpers/             # 辅助类


### ViewModel 实现要点

csharp
public class MainViewModel : INotifyPropertyChanged
{
    // 1. 实现通知接口
    public event PropertyChangedEventHandler? PropertyChanged;

    // 2. 属性变更通知方法
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    // 3. 带通知的属性
    private string noteText = "示例文本";
    public string NoteText
    {
        get => noteText;
        set
        {
            if (noteText == value) return;
            noteText = value;
            OnPropertyChanged(); // 通知 UI 更新
        }
    }
}


---

## 2. 数据绑定系统

### 绑定模式

| 模式             | 说明      | 适用场景 |
| ---------------- | --------- | -------- |
| `OneWay`         | 源 → 目标 | 只读显示 |
| `TwoWay`         | 源 ↔ 目标 | 表单输入 |
| `OneTime`        | 仅初始化  | 静态数据 |
| `OneWayToSource` | 目标 → 源 | 反向同步 |

### 绑定方式

#### ① 基础绑定（DataContext）

xml
<!-- 绑定到 ViewModel 属性 -->
<TextBox Text="{Binding NoteText}" />


#### ② 双向绑定 + 实时更新

xml
<TextBox 
    Text="{Binding NoteText, 
           Mode=TwoWay, 
           UpdateSourceTrigger=PropertyChanged}" />

- `Mode=TwoWay`：双向同步
- `UpdateSourceTrigger=PropertyChanged`：每次按键都更新（默认是失去焦点时）

#### ③ ElementName 绑定

xml
<!-- 绑定到其他控件的属性 -->
<TextBlock 
    Text="{Binding ElementName=MySlider, Path=Value, 
           StringFormat=当前值：{0:F0}}" />
<Slider x:Name="MySlider" Minimum="0" Maximum="100" />


#### ④ RelativeSource 绑定

xml
<!-- Self：绑定到自己 -->
<TextBox 
    Text="{Binding InputText}"
    ToolTip="{Binding Text, RelativeSource={RelativeSource Self}}" />

<!-- TemplatedParent：模板中绑定到控件本身 -->
<TextBlock 
    Text="{Binding Score, 
           RelativeSource={RelativeSource TemplatedParent}}" />

<!-- AncestorType：绑定到父级元素 -->
<TextBlock 
    Text="{Binding DataContext.Title, 
           RelativeSource={RelativeSource AncestorType=Window}}" />


### 绑定调试技巧

xml
<!-- 使用 PresentationTraceSources 调试绑定 -->
<TextBox 
    Text="{Binding NoteText, 
           diag:PresentationTraceSources.TraceLevel=High}" />


---

## 3. 依赖属性与附加属性

### 依赖属性（DependencyProperty）

**用途**：支持绑定、样式、动画、触发器等 WPF 核心功能。

#### 创建步骤

csharp
public class RatingBadge : Control
{
    // 1. 注册依赖属性
    public static readonly DependencyProperty ScoreProperty =
        DependencyProperty.Register(
            nameof(Score),              // 属性名
            typeof(int),                // 属性类型
            typeof(RatingBadge),        // 所属类
            new PropertyMetadata(60)    // 默认值
        );

    // 2. 提供 CLR 包装器
    public int Score
    {
        get => (int)GetValue(ScoreProperty);
        set => SetValue(ScoreProperty, value);
    }
}


#### 带回调的依赖属性

csharp
public static readonly DependencyProperty BadgeBrushProperty =
    DependencyProperty.Register(
        nameof(BadgeBrush),
        typeof(Brush),
        typeof(RatingBadge),
        new PropertyMetadata(
            Brushes.SteelBlue,          // 默认值
            OnBadgeBrushChanged         // 变化回调
        )
    );

private static void OnBadgeBrushChanged(
    DependencyObject d, 
    DependencyPropertyChangedEventArgs e)
{
    // 属性变化时的处理逻辑
    var control = (RatingBadge)d;
    Console.WriteLine($"颜色从 {e.OldValue} 变为 {e.NewValue}");
}


### 附加属性（AttachedProperty）

**用途**：为现有控件添加额外功能，无需继承。

#### 创建步骤

csharp
public static class HoverHelper
{
    // 1. 注册附加属性
    public static readonly DependencyProperty HighlightOnHoverProperty =
        DependencyProperty.RegisterAttached(
            "HighlightOnHover",         // 属性名
            typeof(bool),               // 类型
            typeof(HoverHelper),        // 定义类
            new PropertyMetadata(false, OnHighlightOnHoverChanged)
        );

    // 2. Getter
    public static bool GetHighlightOnHover(DependencyObject obj) =>
        (bool)obj.GetValue(HighlightOnHoverProperty);
    
    // 3. Setter
    public static void SetHighlightOnHover(DependencyObject obj, bool value) =>
        obj.SetValue(HighlightOnHoverProperty, value);
    
    // 4. 变化回调
    private static void OnHighlightOnHoverChanged(
        DependencyObject d, 
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Control control) return;
    
        if ((bool)e.NewValue)
        {
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
        }
        else
        {
            control.MouseEnter -= Control_MouseEnter;
            control.MouseLeave -= Control_MouseLeave;
        }
    }
    
    private static void Control_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Control control)
        {
            control.BorderBrush = Brushes.Blue;
            control.BorderThickness = new Thickness(2);
        }
    }
    
    private static void Control_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Control control)
        {
            control.ClearValue(Control.BorderBrushProperty);
            control.ClearValue(Control.BorderThicknessProperty);
        }
    }
}


#### XAML 中使用

xml
<Border 
    helpers:HoverHelper.HighlightOnHover="True"
    Padding="10"
    Background="LightGray">
    <TextBlock Text="鼠标悬停时显示蓝色边框" />
</Border>


---

## 4. 资源与样式系统

### 资源类型

| 类型     | 关键字            | 特点                 | 适用场景   |
| -------- | ----------------- | -------------------- | ---------- |
| 静态资源 | `StaticResource`  | 编译时解析，性能高   | 不变的资源 |
| 动态资源 | `DynamicResource` | 运行时解析，支持切换 | 主题切换   |

### 资源定义位置

xml
<!-- ① Application 级别（全局） -->
<Application.Resources>
    <SolidColorBrush x:Key="Brush.Primary" Color="#3A7AFE" />
</Application.Resources>

<!-- ② Window 级别（窗口内共享） -->
<Window.Resources>
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <!-- ... -->
    </Style>
</Window.Resources>

<!-- ③ 控件级别（局部） -->
<Button.Resources>
    <local:BoolToStatusTextConverter x:Key="StatusConverter" />
</Button.Resources>


### 样式（Style）

#### 隐式样式（自动应用到所有同类控件）

xml
<Style TargetType="Button">
    <Setter Property="FontSize" Value="14" />
    <Setter Property="Padding" Value="10,5" />
</Style>


#### 显式样式（通过 Key 引用）

xml
<Style x:Key="PrimaryButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="{DynamicResource Brush.Primary}" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="FontWeight" Value="Bold" />
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border 
                    Background="{TemplateBinding Background}"
                    CornerRadius="6"
                    Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center" 
                                    VerticalAlignment="Center" />
                </Border>
            </ControlTemplate>
        </Setter.Value>
    </Setter>

    <!-- 样式触发器 -->
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Opacity" Value="0.8" />
        </Trigger>
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Opacity" Value="0.5" />
        </Trigger>
    </Style.Triggers>
</Style>


#### 样式继承

xml
<Style x:Key="BaseButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="10,5" />
</Style>

<Style x:Key="LargeButtonStyle" 
       TargetType="Button" 
       BasedOn="{StaticResource BaseButtonStyle}">
    <Setter Property="FontSize" Value="18" />
</Style>


### 主题切换实战

csharp
// MainWindow.xaml.cs
private void OnSwitchThemeClick(object sender, RoutedEventArgs e)
{
    var viewModel = DataContext as MainViewModel;
    if (viewModel != null)
    {
        viewModel.IsPrimaryTheme = !viewModel.IsPrimaryTheme;
        
        // 切换资源字典
        var app = Application.Current;
        if (viewModel.IsPrimaryTheme)
        {
            app.Resources["Brush.Primary"] = new SolidColorBrush(Color.FromRgb(58, 122, 254));
        }
        else
        {
            app.Resources["Brush.Primary"] = new SolidColorBrush(Color.FromRgb(255, 107, 107));
        }
    }
}


xml
<!-- 使用 DynamicResource 实现动态切换 -->
<Button Background="{DynamicResource Brush.Primary}" 
        Content="主题色按钮" />


---

## 5. 模板系统

### ControlTemplate（控件模板）

**作用**：改变控件的外观结构。

xml
<ControlTemplate TargetType="Button">
    <Border 
        x:Name="border"
        Background="{TemplateBinding Background}"
        BorderBrush="{TemplateBinding BorderBrush}"
        BorderThickness="{TemplateBinding BorderThickness}"
        CornerRadius="8">
        <ContentPresenter 
            HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
            VerticalAlignment="{TemplateBinding VerticalContentAlignment}"
            Margin="{TemplateBinding Padding}" />
    </Border>
    
    <!-- 模板触发器 -->
    <ControlTemplate.Triggers>
        <Trigger Property="IsPressed" Value="True">
            <Setter TargetName="border" Property="RenderTransform">
                <Setter.Value>
                    <ScaleTransform ScaleX="0.95" ScaleY="0.95" />
                </Setter.Value>
            </Setter>
        </Trigger>
    </ControlTemplate.Triggers>
</ControlTemplate>


### DataTemplate（数据模板）

**作用**：定义数据对象的可视化表示。

xml
<DataTemplate x:Key="TaskTemplate" DataType="{x:Type local:TaskItem}">
    <Border Padding="12" Margin="0,4" 
            Background="White" 
            CornerRadius="8"
            BorderThickness="1"
            BorderBrush="#E0E0E0">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            
            <!-- 任务标题 -->
            <TextBlock 
                Text="{Binding Title}"
                FontWeight="Medium"
                VerticalAlignment="Center" />
            
            <!-- 优先级徽章 -->
            <controls:RatingBadge 
                Grid.Column="1"
                Margin="8,0,0,0"
                BadgeBrush="{DynamicResource Brush.Primary}"
                Score="{Binding Priority, StringFormat={}{0}0}" />
        </Grid>
    </Border>
</DataTemplate>


### 模板选择器（根据条件使用不同模板）

csharp
public class TaskTemplateSelector : DataTemplateSelector
{
    public DataTemplate HighPriorityTemplate { get; set; }
    public DataTemplate NormalTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is TaskItem task)
        {
            return task.Priority >= 3 ? HighPriorityTemplate : NormalTemplate;
        }
        return base.SelectTemplate(item, container);
    }
}


xml
<Window.Resources>
    <local:TaskTemplateSelector x:Key="TaskSelector"
        HighPriorityTemplate="{StaticResource HighPriorityTemplate}"
        NormalTemplate="{StaticResource NormalTemplate}" />
</Window.Resources>

<ListBox ItemTemplateSelector="{StaticResource TaskSelector}" />


---

## 6. 命令系统

### ICommand 接口

csharp
public interface ICommand
{
    event EventHandler CanExecuteChanged;
    bool CanExecute(object parameter);
    void Execute(object parameter);
}


### RelayCommand 实现

csharp
public class RelayCommand : ICommand
{
    private readonly Action<object?> execute;
    private readonly Predicate<object?>? canExecute;

    public event EventHandler? CanExecuteChanged;
    
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter) => 
        canExecute?.Invoke(parameter) ?? true;
    
    public void Execute(object? parameter) => 
        execute(parameter);
    
    // ⭐ 关键：手动触发命令状态刷新
    public void RaiseCanExecuteChanged() => 
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}


### ViewModel 中使用命令

csharp
public class MainViewModel : INotifyPropertyChanged
{
    // 命令属性
    public ICommand AddTaskCommand { get; }
    public ICommand CompleteSelectedCommand { get; }

    public MainViewModel()
    {
        // 初始化命令
        AddTaskCommand = new RelayCommand(_ => AddTask());
        
        CompleteSelectedCommand = new RelayCommand(
            _ => CompleteSelected(),
            _ => SelectedTask is not null && !SelectedTask.IsDone
        );
    }
    
    private void AddTask()
    {
        Tasks.Add(new TaskItem 
        { 
            Title = $"新任务 {Tasks.Count + 1}",
            IsDone = false 
        });
    }
    
    private void CompleteSelected()
    {
        if (SelectedTask != null)
        {
            SelectedTask.IsDone = true;
            (CompleteSelectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
    
    // 选中项变化时刷新命令状态
    private TaskItem? selectedTask;
    public TaskItem? SelectedTask
    {
        get => selectedTask;
        set
        {
            selectedTask = value;
            OnPropertyChanged();
            (CompleteSelectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}


### XAML 中绑定命令

xml
<!-- 基础命令绑定 -->
<Button Command="{Binding AddTaskCommand}" 
        Content="新增任务" />

<!-- 带参数的命令 -->
<Button Command="{Binding DeleteCommand}" 
        CommandParameter="{Binding SelectedItem}"
        Content="删除" />


---

## 7. 触发器与动画

### 属性触发器（Property Trigger）

xml
<Style TargetType="Button">
    <Style.Triggers>
        <!-- 鼠标悬停 -->
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="LightBlue" />
        </Trigger>

        <!-- 禁用状态 -->
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Opacity" Value="0.5" />
        </Trigger>
        
        <!-- 多条件触发器 -->
        <MultiTrigger>
            <MultiTrigger.Conditions>
                <Condition Property="IsMouseOver" Value="True" />
                <Condition Property="IsPressed" Value="False" />
            </MultiTrigger.Conditions>
            <Setter Property="Cursor" Value="Hand" />
        </MultiTrigger>
    </Style.Triggers>
</Style>


### 事件触发器（EventTrigger）+ 动画

xml
<Button Content="播放动画">
    <Button.Triggers>
        <EventTrigger RoutedEvent="Button.Click">
            <BeginStoryboard>
                <Storyboard>
                    <!-- 进度条动画 -->
                    <DoubleAnimation 
                        Storyboard.TargetName="LearningProgress"
                        Storyboard.TargetProperty="Value"
                        From="0" To="100" Duration="0:0:2"
                        EasingFunction="{StaticResource EaseOut}" />
                    
                    <!-- 透明度动画 -->
                    <DoubleAnimation 
                        Storyboard.TargetProperty="Opacity"
                        From="1" To="0.5" To="1"
                        Duration="0:0:1"
                        AutoReverse="True" />
                </Storyboard>
            </BeginStoryboard>
        </EventTrigger>
    </Button.Triggers>
</Button>


### 常用缓动函数

xml
<Window.Resources>
    <!-- 指数缓出 -->
    <ExponentialEase x:Key="EaseOut" 
                     EasingMode="EaseOut" 
                     Exponent="3" />
    
    <!-- 弹性缓动 -->
    <BackEase x:Key="BackEase" 
              EasingMode="EaseOut" 
              Amplitude="0.5" />
</Window.Resources>


### 路由事件策略

| 策略           | 方向     | 示例             | 说明            |
| -------------- | -------- | ---------------- | --------------- |
| 冒泡（Bubble） | 子 → 父  | `Button.Click`   | 最常用          |
| 隧道（Tunnel） | 父 → 子  | `PreviewKeyDown` | 以 Preview 开头 |
| 直接（Direct） | 仅源元素 | `MouseEnter`     | 不传播          |

xml
<!-- 在父容器捕获子元素的事件（冒泡） -->
<StackPanel Button.Click="OnAnyButtonClick">
    <Button Content="按钮1" />
    <Button Content="按钮2" />
</StackPanel>


csharp
private void OnAnyButtonClick(object sender, RoutedEventArgs e)
{
    var button = e.OriginalSource as Button;
    MessageBox.Show($"点击了：{button?.Content}");
}


---

## 8. 集合视图与分组

### ICollectionView 三大功能

csharp
public class MainViewModel : INotifyPropertyChanged
{
    // 原始数据源
    public ObservableCollection<TaskItem> Tasks { get; }

    // 集合视图（支持分组、排序、筛选）
    public ICollectionView TaskView { get; }
    
    public MainViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>
        {
            new() { Title = "学习依赖属性", Category = "Dependency", Priority = 1 },
            new() { Title = "练习资源字典", Category = "Resources", Priority = 2 },
            new() { Title = "写一个 DataTemplate", Category = "Templates", Priority = 3 }
        };
    
        // 创建视图
        TaskView = CollectionViewSource.GetDefaultView(Tasks);
        
        // ① 分组
        TaskView.GroupDescriptions.Add(
            new PropertyGroupDescription(nameof(TaskItem.Category))
        );
        
        // ② 排序
        TaskView.SortDescriptions.Add(
            new SortDescription(nameof(TaskItem.Priority), ListSortDirection.Ascending)
        );
        
        // ③ 筛选
        TaskView.Filter = item =>
        {
            var task = item as TaskItem;
            return task != null && !task.IsDone; // 只显示未完成的任务
        };
    }
}


### XAML 中显示分组

xml
<ListBox ItemsSource="{Binding TaskView}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Title}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
    
    <!-- 分组样式 -->
    <ListBox.GroupStyle>
        <GroupStyle>
            <GroupStyle.HeaderTemplate>
                <DataTemplate>
                    <TextBlock 
                        Text="{Binding Name, StringFormat=分类：{0}}"
                        FontWeight="Bold"
                        FontSize="14"
                        Foreground="{DynamicResource Brush.Primary}"
                        Margin="0,10,0,4" />
                </DataTemplate>
            </GroupStyle.HeaderTemplate>
        </GroupStyle>
    </ListBox.GroupStyle>
</ListBox>


### 动态切换分组

csharp
// 清除当前分组
TaskView.GroupDescriptions.Clear();

// 改为按优先级分组
TaskView.GroupDescriptions.Add(
    new PropertyGroupDescription(nameof(TaskItem.Priority))
);


---

## 9. 值转换器

### IValueConverter 实现

csharp
public class BoolToStatusTextConverter : IValueConverter
{
    // 正向转换：bool → string
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool done)
        {
            return done ? "已完成" : "进行中";
        }
        return "未知状态";
    }

    // 反向转换：string → bool（用于 TwoWay 绑定）
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string text && text == "已完成";
    }
}


### XAML 中使用转换器

xml
<Window.Resources>
    <!-- 声明转换器 -->
    <local:BoolToStatusTextConverter x:Key="StatusConverter" />
</Window.Resources>

<!-- 在绑定中使用 -->
<TextBlock 
    Text="{Binding IsDone, Converter={StaticResource StatusConverter}}" />

<!-- 带参数和文化的转换器 -->
<TextBlock 
    Text="{Binding CreateTime, 
           Converter={StaticResource DateConverter},
           ConverterParameter='yyyy-MM-dd',
           ConverterCulture='zh-CN'}" />


### 常用转换器示例

#### ① 布尔到可见性转换

csharp
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = (bool)value;
        // 如果有参数，反转逻辑
        if (parameter is string invert && invert == "Invert")
            isVisible = !isVisible;
        
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Visibility)value == Visibility.Visible;
    }
}


#### ② 数字到星级转换

csharp
public class NumberToStarsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int rating)
        {
            return new string('★', rating) + new string('☆', 5 - rating);
        }
        return "☆☆☆☆☆";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


---

## 10. 自定义控件

### 完整创建流程

#### ① 创建控件类

csharp
namespace WpfXamlPlayground.Controls
{
    public class RatingBadge : Control
    {
        // 静态构造函数：注册默认样式
        static RatingBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RatingBadge),
                new FrameworkPropertyMetadata(typeof(RatingBadge))
            );
        }

        // 依赖属性：分数
        public int Score
        {
            get => (int)GetValue(ScoreProperty);
            set => SetValue(ScoreProperty, value);
        }
        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register(nameof(Score), typeof(int), typeof(RatingBadge), 
                new PropertyMetadata(60));
    
        // 依赖属性：背景色
        public Brush BadgeBrush
        {
            get => (Brush)GetValue(BadgeBrushProperty);
            set => SetValue(BadgeBrushProperty, value);
        }
        public static readonly DependencyProperty BadgeBrushProperty =
            DependencyProperty.Register(nameof(BadgeBrush), typeof(Brush), typeof(RatingBadge), 
                new PropertyMetadata(Brushes.SteelBlue));
    }
}


#### ② 定义默认样式（Themes/Generic.xaml）

xml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="clr-namespace:WpfXamlPlayground.Controls">

    <Style TargetType="{x:Type controls:RatingBadge}">
        <Setter Property="Width" Value="120" />
        <Setter Property="Height" Value="36" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type controls:RatingBadge}">
                    <Border 
                        CornerRadius="18"
                        Background="{TemplateBinding BadgeBrush}"
                        Padding="12,6"
                        SnapsToDevicePixels="True">
                        <TextBlock 
                            HorizontalAlignment="Center"
                            VerticalAlignment="Center"
                            Foreground="White"
                            FontWeight="Bold"
                            Text="{Binding Score, 
                                  RelativeSource={RelativeSource TemplatedParent}, 
                                  StringFormat=Score: {0}}" />
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>


#### ③ XAML 中使用

xml
<!-- 引入命名空间 -->
xmlns:controls="clr-namespace:WpfXamlPlayground.Controls"

<!-- 使用自定义控件 -->
<controls:RatingBadge 
    Score="{Binding Priority, StringFormat={}{0}0}"
    BadgeBrush="{DynamicResource Brush.Primary}" />


---

## 🎯 最佳实践总结

### 1. MVVM 原则

- ✅ ViewModel 不包含任何 UI 元素引用
- ✅ 使用命令而非事件处理
- ✅ 属性变更必须调用 `OnPropertyChanged()`
- ✅ 命令状态变化必须调用 `RaiseCanExecuteChanged()`

### 2. 性能优化

- ✅ 优先使用 `TemplateBinding`（比 `Binding` 快）
- ✅ 大数据列表使用虚拟化（`VirtualizingStackPanel`）
- ✅ 避免在频繁触发的事件中创建对象
- ✅ 使用 `x:Static` 代替静态资源引用

### 3. 内存管理

- ✅ 事件订阅后要取消订阅（防止内存泄漏）
- ✅ `ObservableCollection` 比 `List` 更适合绑定
- ✅ 大图片使用 `BitmapCacheOption.OnLoad`

### 4. 代码规范

- ✅ 依赖属性命名：`XXXProperty`（静态字段）
- ✅ 附加属性放在静态类中
- ✅ 转换器实现 `IValueConverter` 接口
- ✅ 使用 `nameof()` 避免硬编码字符串

---

## 📖 学习路线建议

### 初级阶段
1. 掌握基础数据绑定（OneWay、TwoWay）
2. 理解 INotifyPropertyChanged
3. 学会使用 StaticResource 和 DynamicResource
4. 掌握基础样式和模板

### 中级阶段
1. 深入理解依赖属性机制
2. 掌握命令模式和 RelayCommand
3. 学会创建值转换器
4. 理解集合视图和分组

### 高级阶段
1. 创建自定义控件
2. 使用附加属性扩展功能
3. 复杂动画和触发器
4. 自定义面板和布局

---

## 🔗 参考资源

- [Microsoft WPF 官方文档](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [WPF 数据绑定概述](https://docs.microsoft.com/dotnet/desktop/wpf/data/)
- [依赖属性概述](https://docs.microsoft.com/dotnet/desktop/wpf/properties/dependency-properties-overview/)

---

**最后更新**：2026-04-24  
**基于项目**：WpfXamlPlayground


如果你希望我针对其中某个章节（例如 自定义控件 或 命令系统）提供更具体的代码示例，或者将其改写为适合新人阅读的教程版本，随时告诉我。