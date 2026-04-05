using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfXamlPlayground.Controls;

// 第 1 步：C# 代码中声明
public class RatingBadge : Control   // 继承自 Control 基类
{
    // 静态构造函数：建立“身份认证
    
    /*
     *1. 避免“重复注册”报错
     * DefaultStyleKeyProperty.OverrideMetadata 这行代码的作用是向 WPF 系统注册：“嘿，RatingBadge 这个类型的默认样式在这里找。”
     * 如果是普通构造函数：每当你 new RatingBadge() 创建一个新徽章时，它都会尝试注册一次。WPF 会报错：“这个类型已经注册过了，你不能重复注册！” 💥因为是静态构造函数：它在整个程序运行期间只执行一次（通常是第一次创建该控件时）。这样就保证了注册动作只发生一次，既安全又高效。
     * 2. 它是给“类”看的，不是给“对象”看的
        这个构造函数的目的不是为了初始化某个具体的徽章（比如设置它的分数是 60 还是 80），而是为了初始化 RatingBadge 这个类型本身。
        普通构造函数：负责处理“个体”差异（比如这个徽章是红的，那个是蓝的）。
        静态构造函数：负责处理“群体”规则（比如所有 RatingBadge 都要去 Generic.xaml 找样式）。
        3. C# 的语言特性保证
        C# 规定静态构造函数由 CLR（公共语言运行时） 自动调用，且保证：
        自动触发：你不需要手动去调它。
        线程安全：即使多个线程同时创建控件，它也只会安全地执行一次。
        优先执行：它会在任何实例创建之前执行，确保“规矩”先定好，再开始“干活”。
     */
   // ⭐ 静态构造函数：告诉 WPF "我的样式在 Generic.xaml" /*在 WPF 自定义控件中，构造函数必须是静态的（static）*/
    static RatingBadge()
    {
        // ⭐ 关键代码：告诉 WPF "我的默认样式在 Generic.xaml 中"
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingBadge), new FrameworkPropertyMetadata(typeof(RatingBadge)));
    }

    // 依赖属性1：Score 属性：分数的“存取窗口
    public int Score
    {
        get => (int)GetValue(ScoreProperty);
        set => SetValue(ScoreProperty, value);
    }
    public static readonly DependencyProperty ScoreProperty =
        DependencyProperty.Register(nameof(Score), typeof(int), typeof(RatingBadge), new PropertyMetadata(60));

    /*依赖属性2： BadgeBrush 属性：背景颜色（默认钢蓝色） 颜色的“存取窗口” */
    //类型是画刷（颜色）  效果：让你能在 XAML 里通过 BadgeBrush="Red" 或者绑定主题色来动态改变徽章的颜色。
    public Brush BadgeBrush
    {
        get => (Brush)GetValue(BadgeBrushProperty);
        set => SetValue(BadgeBrushProperty, value);
    }

    public static readonly DependencyProperty BadgeBrushProperty =
        DependencyProperty.Register(nameof(BadgeBrush), typeof(Brush), typeof(RatingBadge), new PropertyMetadata(Brushes.SteelBlue));
}
