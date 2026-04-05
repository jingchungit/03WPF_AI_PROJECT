using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfXamlPlayground.Helpers;

public static class HoverHelper
{
    //附加属性使用流程：
// 1. 定义附加属性(C#)
//   ↓
//2. 注册 DependencyProperty
//   ↓
//3. 提供 Setter/Getter 方法
//   ↓
//4. 实现变化回调逻辑
//   ↓
//5. XAML 中使用
//   ↓
//6. 运行时生效


    // 1. 定义附加属性
    public static readonly DependencyProperty HighlightOnHoverProperty =
        DependencyProperty.RegisterAttached(
            //RegisterAttached    注册附加属性  HoverHelper.HighlightOnHover
            "HighlightOnHover",
            typeof(bool),
            typeof(HoverHelper),
            new PropertyMetadata(false, OnHighlightOnHoverChanged));
    // "HighlightOnHover"	属性名（XAML 中使用时不带 Property 后缀）
    //typeof(bool) 属性值类型
    //typeof(HoverHelper) 定义该属性的类
    //参数 4：元数据
    //false	默认值
    //OnHighlightOnHoverChanged   属性变化时的回调方法

    // 3. 获取器
    public static bool GetHighlightOnHover(DependencyObject obj) =>
        (bool)obj.GetValue(HighlightOnHoverProperty);

    // 2. 设置器
    public static void SetHighlightOnHover(DependencyObject obj, bool value) =>
        obj.SetValue(HighlightOnHoverProperty, value);

    // 4. 属性变化时的处理逻辑
    private static void OnHighlightOnHoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    //d DependencyObject    设置了附加属性的对象（如 Border、Button）
    //e DependencyPropertyChangedEventArgs  属性变化的详细信息
    //参数 e 的重要属性
    // e.OldValue  // 旧值（变化前的值）
    //e.NewValue  // 新值（变化后的值）
        if (d is not Control control)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
        }
        else
        {
            control.MouseEnter -= Control_MouseEnter;
            control.MouseLeave -= Control_MouseLeave;
            control.ClearValue(Control.BorderBrushProperty);
            control.ClearValue(Control.BorderThicknessProperty);
        }
    }

    // 5. 鼠标进入处理
    private static void Control_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Control control)
        {
            control.BorderBrush = new SolidColorBrush(Color.FromRgb(58, 122, 254));//设置边框颜色画刷
            control.BorderThickness = new Thickness(2);//设置边框宽度
        }
    }
    // 6. 鼠标离开处理
    private static void Control_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Control control)
        {
            control.ClearValue(Control.BorderBrushProperty);//清空边框颜色画刷
            control.ClearValue(Control.BorderThicknessProperty);//清空边框宽度
        }
    }
}

//二、普通 CLR 属性 vs 依赖属性
//    特性  普通 CLR 属性 依赖属性
//数据绑定	❌ 不支持	✅ 支持
//样式(Style)  ❌ 不支持	✅ 支持
//资源(Resource)   ❌ 不支持	✅ 支持
//触发器(Trigger)   ❌ 不支持	✅ 支持
//动画(Animation)  ❌ 不支持	✅ 支持
//默认值 ❌ 需手动设置	✅ 系统提供
//值继承 ❌ 不支持	✅ 支持（如字体）
//内存占用 每个实例都有字段    按需分配，更节省
//三、具体能力对比
//1. 数据绑定
//// ❌ 普通 CLR 属性 - 无法绑定
//public class MyClass
//{
//    public string Name { get; set; }  // 绑定不生效！
//}

//// ✅ 依赖属性 - 可以绑定
//public class MyClass : DependencyObject
//{
//    public static readonly DependencyProperty NameProperty =
//        DependencyProperty.Register("Name", typeof(string), typeof(MyClass));

//    public string Name
//    {
//        get => (string)GetValue(NameProperty);
//        set => SetValue(NameProperty, value);
//    }
//}
//<!-- 只有依赖属性才能这样绑定 -->
//<TextBlock Text = "{Binding Name}" />
//2. 样式和触发器
//<!-- ✅ 依赖属性可以用 Style 和 Trigger -->
//<Style TargetType = "Button" >
//    < Setter Property="Background" Value="Blue"/>  ← 依赖属性
//    <Style.Triggers>
//        <Trigger Property = "IsMouseOver" Value="True">  ← 依赖属性
//            <Setter Property = "Background" Value="Red"/>
//        </Trigger>
//    </Style.Triggers>
//</Style>

//<!-- ❌ 普通 CLR 属性不能用 Style 和 Trigger -->
//3. 资源引用
//    <!-- ✅ 依赖属性可以引用资源 -->
//<Button Background = "{DynamicResource Brush.Primary}" />
//< !-- ❌ 普通 CLR 属性无法引用资源-- >

//4. 动画
//    <!-- ✅ 依赖属性可以做动画 -->
//<Storyboard>
//    <DoubleAnimation Storyboard.TargetProperty="Opacity" To= "0.5" />
//</ Storyboard >

//< !-- ❌ 普通 CLR 属性无法动画化 -->
//5. 值继承
//    <StackPanel FontSize = "16" >  ← 设置字体大小
//    <TextBlock Text = "继承 16px" />  ← 自动继承
//    <TextBlock Text = "继承 16px" />  ← 自动继承
//</StackPanel>

//<!-- ✅ 依赖属性支持值继承（如 FontSize、Foreground） -->
//<!-- ❌ 普通 CLR 属性不支持继承 -->

//    四、为什么需要依赖属性？
//WPF 的核心需求
//每个对象实例都有字段，即使值相同
//┌─────────────────────────────────────────────────────────────────┐
//│  WPF 需要这些功能                                                │
//│                                                                 │
//│  1. 数据绑定 → 需要监听属性变化                                 │
//│  2. 样式系统 → 需要统一设置属性                                 │
//│  3. 触发器   → 需要根据条件改变属性                             │
//│  4. 动画系统 → 需要动态修改属性值                               │
//│  5. 资源系统 → 需要引用外部资源                                 │
//│  6. 值继承   → 需要属性值在视觉树中传递                         │
//│                                                                 │
//│  ❌ 普通 CLR 属性无法满足这些需求                                │
//│  ✅ 依赖属性专门设计来满足这些需求                               │
//└─────────────────────────────────────────────────────────────────┘
//    五、技术实现差异
//普通 CLR 属性
//    public class Person
//{
//    private string _name;  // 每个实例都有这个字段
    
//    public string Name
//    {
//        get => _name;
//        set => _name = value;
//    }
//}

//// 问题：
//// 1. 无法通知外界值变化了
//// 2. 无法支持绑定、样式、动画等
//    依赖属性
//    public class Person : DependencyObject
//{
//    // 静态注册，所有实例共享
//    public static readonly DependencyProperty NameProperty =
//        DependencyProperty.Register("Name", typeof(string), typeof(Person),
//            new PropertyMetadata("默认值", OnNameChanged));  // 变化回调
    
//    public string Name
//    {
//        get => (string)GetValue(NameProperty);
//        set => SetValue(NameProperty, value);
//    }
    
//    private static void OnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        // 值变化时自动通知！
//        Console.WriteLine($"Name 从 {e.OldValue} 变为 {e.NewValue}");
//    }
//}

//// 优势：
//// 1. 自动通知变化
//// 2. 支持绑定、样式、动画等
//// 3. 内存更高效（按需分配）
//    六、内存效率对比
//    普通 CLR 属性
//    每个对象实例都有字段，即使值相同

//Person1: _name = "张三"  (占用内存)
//Person2: _name = "李四"  (占用内存)
//Person3: _name = "王五"  (占用内存)
//    依赖属性
//    默认值共享存储，只存储与默认值不同的部分

//Person1: 使用默认值  (不占用额外内存)
//Person2: 使用默认值  (不占用额外内存)
//Person3: "王五"     (只存储差异值)



