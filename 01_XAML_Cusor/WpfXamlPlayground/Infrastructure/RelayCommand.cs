using System.CodeDom.Compiler;
using System.Reflection.Metadata;
using System.Windows.Input;//1. 基础声明

namespace WpfXamlPlayground.Infrastructure;

//🧩 背景知识：什么是 RelayCommand？ 是 WPF MVVM 模式中实现“命令绑定”的核心工具。
//RelayCommand 是一个自定义的命令类，它实现了 WPF 的 ICommand 接口，让你能把 普通方法（如 AddTask()） 和 启用条件（如 “输入框非空”） 包装成一个可被 XAML 绑定的“命令”，从而实现按钮点击、菜单操作等用户交互。
public class RelayCommand : ICommand
{
    private readonly Action<object?> execute;
    //•	execute：一个委托，指向你要执行的方法（比如 AddTask）。
            //•	Action<object?> 表示：这是一个无返回值的方法，接受一个可为空的对象参数（对应 XAML 中 CommandParameter）。
    //•	canExecute：一个可选的委托，用于判断命令当前是否可用（比如“只有选中任务才能完成”）。
                //•	Predicate<object?> 表示：这是一个返回 bool 的方法，也接受一个参数。
    private readonly Predicate<object?>? canExecute; //Predicate<T>	bool Method(T arg)	表示判断条件（谓词）	固定为 bool
    //•	为了通用性，ICommand 接口把参数类型定义为最顶层的基类：object。

    //3. 构造函数：初始化命令
    //•	必填：要执行的方法（execute）
    //•	可选：启用条件（canExecute），默认为 null（表示始终可用）
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    //4. 实现 ICommand 接口
    //(1) CanExecuteChanged 事件
    //•	这是 ICommand 要求的事件。
    //•	作用：当命令的启用状态可能发生变化时，通知 WPF 重新调用 CanExecute 检查。
    //•	WPF 的按钮等控件会自动订阅此事件。
    public event EventHandler? CanExecuteChanged;

    //(2) CanExecute 方法
//      •  WPF 在以下时机调用此方法：
//      •	初始加载 UI 时
//      •	收到 CanExecuteChanged 事件后
    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
    //##CanExecute解析 
    //•	这是一个公共方法，名字叫 CanExecute。
    //•	返回类型是 bool（true 表示命令可用，false 表示禁用）。
    //•	参数是 object? parameter：
    //    •	object：C# 中所有类型的基类，可以代表任何对象（字符串、数字、自定义类等）。
    //    •	? 表示“可为空”（这是 C# 8.0+ 的可空引用类型特性）。
    //    •	object? = “这个参数可以是任意对象，也可以是 null”。
    //    •	对比：object（不带?）理论上不能为 null（但在旧代码中仍可能为 null，带? 更安全明确）。
    //        ✅ 所以：object? parameter 的意思是
    //       “我接受一个任意类型的参数，它可能是某个值，也可能是 null”。

    //?. 是什么？ → 空条件运算符（Null-Conditional Operator）
    //    •	作用：只有当左边不为 null 时，才执行右边的操作。
    //    •	如果 canExecute 是 null，整个 canExecute?.Invoke(...) 就直接返回 null，不会抛出空引用异常。
    //Invoke 是什么？
    //    •	Invoke 是委托（Delegate）的标准**调用**方法。
    //    •	因为 canExecute 是一个委托（比如 _ => SelectedTask != null），要执行它，就得调用.Invoke()。
    //    •	Invoke(parameter) = “用 parameter 作为参数，运行这个委托”。
    //Invoke 的作用是：执行一个委托（Delegate），并传入参数。
    //?? 是什么？ → 空合并运算符（Null-Coalescing Operator）
    //    •	作用：如果左边是 null，就返回右边的值；否则返回左边的值。
    //    •	语法：a ?? b → 如果 a 不为 null，返回 a；否则返回 b。

    //(3) Execute 方法
    //•	当用户点击绑定此命令的按钮时，WPF 会调用此方法。
    //•	它直接调用你传入的 execute 委托，并把 CommandParameter（如果有）作为参数传进去。
    public void Execute(object? parameter) => execute(parameter);
    //5. 主动刷新命令状态：RaiseCanExecuteChanged()
//    •	这是关键！
//      •	因为 WPF 不会自动知道你的启用条件何时变化（比如 SelectedTask 改变了）。
//      •	所以你需要在 ViewModel 中手动调用此方法，告诉 WPF：“请重新检查这个命令能不能用”。
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

//✅ 为什么需要 RelayCommand？
//问题 解决方案
//XAML 不能直接绑定方法   通过 ICommand 间接调用
//按钮启用状态需动态变化 提供 canExecute +RaiseCanExecuteChanged
//避免代码重复 封装成通用命令类，到处复用
