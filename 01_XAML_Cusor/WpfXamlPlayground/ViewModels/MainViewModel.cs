using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Windows.Data;
using System.Windows.Input;
using WpfXamlPlayground.Infrastructure;
using WpfXamlPlayground.Models;

namespace WpfXamlPlayground.ViewModels;
//MainViewModel的作用
//1.	任务列表管理
//    public ObservableCollection<TaskItem> Tasks { get; }
//2.	用户操作响应
//public ICommand AddTaskCommand { get; }
//private void AddTask() { /* 添加新任务逻辑 */ }
//3.	界面状态同步
//public bool IsPrimaryTheme { get; set; }


//在 WPF MVVM 模式中，ViewModel 与 View 是分离的。当 ViewModel 中的数据发生变化时，需要一种机制通知 View 更新界面：
//┌─────────────┐    数据变更    ┌─────────────┐
//│ ViewModel   │ ────────────→ │    View     │
//│ (数据层)    │   事件通知     │  (界面层)   │
//└─────────────┘               └─────────────┘
//       ▲
//       │
//  INotifyPropertyChanged
//INotifyPropertyChanged 接口只要求实现一个事件：
//// 接口定义
//public event PropertyChangedEventHandler? PropertyChanged;

//// 你的实现
//public event PropertyChangedEventHandler? PropertyChanged;

//// 触发通知的方法
//private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
//    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
public class MainViewModel : INotifyPropertyChanged  //ViewModel层
{
    //缓存命令实例
    //•	这两个字段用于缓存命令实例（RelayCommand 是自定义的 ICommand 实现）。
    //•	使用 readonly 表示它们只能在构造函数中赋值，之后不可变。
    private readonly RelayCommand completeSelectedCommand;
    private readonly RelayCommand broadcastCommand;
    //支持数据绑定的后备字段（backing fields）,每个都对应一个 public 属性（在后面定义），
    //字段 用途说明
    //noteText 双向绑定文本框内容，用户修改会更新此字段，反之亦然
    //broadcastText   广播消息的输入内容
    //receiverA / receiverB 接收广播消息的显示文本
    //selectedTask 当前在 UI 中选中的任务项（如 ListBox 选中项）
    //isPrimaryTheme 控制当前使用主主题还是备选主题（用于动态切换样式）
    private string noteText = "修改我试试双向绑定";
    private string broadcastText = "这是一条广播消息";
    private string receiverA = "等待消息...";
    private string receiverB = "等待消息...";
    private TaskItem? selectedTask;
    private bool isPrimaryTheme = true;

    public MainViewModel()
    {
        //ObservableCollection<GroupDescription>是什么？
        //这是一个 泛型集合，专门用来存储 GroupDescription 类型的对象（比如 PropertyGroupDescription），并且：
        //•	✅ 支持通知机制：当集合内容发生变化（如添加、删除分组规则）时，会自动通知 UI 更新。
        //•	✅ 是 WPF 集合视图（ICollectionView）的一部分：你通过 TaskView.GroupDescriptions 访问的就是这个集合。
        //🔹 GroupDescription（基类）
            //•	定义在 System.ComponentModel 命名空间。
            //•	是所有“分组描述器”的抽象基类。
            //•	它本身不直接使用，而是由子类实现具体分组逻辑。
        //🔹 ObservableCollection<T>
            //•	WPF 特有的集合类型。
            //•	继承自 Collection<T>，并实现了 INotifyCollectionChanged 和 INotifyPropertyChanged。
            //•	当你对集合执行 Add、Remove、Clear 等操作时，会触发 CollectionChanged 事件。
            //•	WPF 绑定系统监听此事件，自动刷新 UI。
        Tasks = new ObservableCollection<TaskItem>  // Tasks 是一个 ObservableCollection<TaskItem>，它是你的原始数据源。
        {
            new() { Title = "学习依赖属性", Category = "Dependency", Priority = 1, IsDone = false },
            new() { Title = "练习资源字典", Category = "Resources", Priority = 2, IsDone = true },
            new() { Title = "写一个 DataTemplate", Category = "Templates", Priority = 3, IsDone = false },
            new() { Title = "试试触发器和动画", Category = "Triggers", Priority = 2, IsDone = false }
        };

        TaskView = CollectionViewSource.GetDefaultView(Tasks);
        TaskView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(TaskItem.Category)));
        //•	TaskView 是 ICollectionView 类型。
        //•	它的 GroupDescriptions 属性就是 ObservableCollection<GroupDescription>。
        //•	调用.Add(...) 后：
        //•	分组规则被加入集合；
        //•	因为是 ObservableCollection，WPF 自动收到“集合变更”通知；
        //•	视图立即按新规则重新分组数据；
        //•	如果 XAML 支持分组显示，UI 会立刻更新。


        //按 TaskItem 对象的 Category 属性值对任务列表进行分组。
        //1.ICollectionView 与分组
        //TaskView 是通过 CollectionViewSource.GetDefaultView(Tasks) 获取的 ICollectionView 视图。WPF 的集合视图（如 ListCollectionView）支持排序、筛选和分组，而无需修改原始数据源（Tasks）。
        //2.PropertyGroupDescription 的作用                                          其他子类（较少用）：DataGridGroupDescription（用于 DataGrid）、自定义分组等。   
        //•	它告诉 WPF：“请根据指定属性的值，把集合中的项分成多个组”。
        //•	构造函数接收一个字符串参数，表示要用于分组的属性名称。
        //•	使用 nameof(TaskItem.Category) 是安全的做法（避免硬编码 "Category" 字符串），如果将来重命名 Category 属性，编译器会自动更新或报错。
        //3.UI 效果
        //    在 XAML 中，如果你使用 ItemsControl（如 ListBox 或 ListView）并配合 GroupStyle，WPF 会自动：
        //    •	将所有 Category == "Dependency" 的任务归为一组，
        //    •	所有 Category == "Resources" 的归为另一组，等等，
        //    •	并为每组显示一个组标题（如 “Dependency”、“Resources”）。

        //ObservableCollection<GroupDescription> 的优势：
        //   特性                说明
        //   自动通知         添加/ 移除分组规则 → UI 自动更新
        //   动态分组         可以在运行时切换分组方式（比如从按 Category 改为按 Priority）
        //   与 MVVM 兼容 符合“数据驱动 UI”的原则
        //场景：用户点击按钮切换分组方式
        // 清除当前分组
        //TaskView.GroupDescriptions.Clear();

        //// 改为按优先级分组
        //TaskView.GroupDescriptions.Add(
        //    new PropertyGroupDescription(nameof(TaskItem.Priority))
        //);

        //初始化命令（Commands）这是 WPF MVVM 模式中处理用户交互（如按钮点击）的核心机制。我们逐行详细解释：
        //🧩 背景知识：什么是 RelayCommand？
        AddTaskCommand = new RelayCommand(_ => AddTask());
        //_ 表示忽略传入的参数（因为 AddTask() 不需要参数）
        //当命令执行时，会调用 AddTask() 方法
        
        
        completeSelectedCommand = new RelayCommand(
            _ => CompleteSelected(),
            _ => SelectedTask is not null && !SelectedTask.IsDone);
        CompleteSelectedCommand = completeSelectedCommand; //私有化
        /*参数 1：_ => CompleteSelected()
        作用：当命令执行时要运行的方法
            含义：调用 CompleteSelected() 方法（把选中的任务标记为已完成）
        _：忽略传入的参数（因为不需要 CommandParameter）
        参数 2：_ => SelectedTask is not null && !SelectedTask.IsDone
        作用：判断命令当前是否可用（决定按钮是否禁用）
        含义：只有满足以下两个条件时，命令才可用：
            ✅ SelectedTask is not null - 有选中的任务
            ✅ !SelectedTask.IsDone - 该任务尚未完成*/
        // 命令状态何时刷新？SelectedTask
        
            
        // 参数 1：_ => BroadcastMessage()
        // 作用：当用户点击按钮时执行的方法
        //     调用：BroadcastMessage() 方法（第 274-278 行定义）
        // 参数 2：_ => !string.IsNullOrWhiteSpace(BroadcastText)
        // 启用条件：只有当 BroadcastText 不为空且不只包含空格时，命令才可用。
        broadcastCommand = new RelayCommand(
            _ => BroadcastMessage(), // 执行逻辑
            _ => !string.IsNullOrWhiteSpace(BroadcastText)); // 启用条件
        BroadcastMessageCommand = broadcastCommand;//赋值给公开属性

        // 订阅顺序决定执行顺序
        MessageHub.MessagePublished += OnMessagePublishedForReceiverA;
        MessageHub.MessagePublished += OnMessagePublishedForReceiverB;
    }

    //定义: 任务列表的数据源集合
    // 作用:
    // 存储所有的 TaskItem 对象(任务项)
    // ObservableCollection:WPF 专用集合,支持自动通知 UI 更新
    // 当添加/删除任务时,界面会自动刷新
    public ObservableCollection<TaskItem> Tasks { get; }

    // 定义: 任务列表的视图层(带分组、排序、筛选功能)
    // 作用:
    // 是 Tasks 集合的一个"投影"或"视图"
    // 不修改原始数据,只提供不同的展示方式
    //     支持分组(按 Category)、排序、筛选
    public ICollectionView TaskView { get; }

    // 定义: 添加任务的命令
    public ICommand AddTaskCommand { get; }

    // 定义: 完成选中任务的命令
    public ICommand CompleteSelectedCommand { get; }

    // 定义: 广播消息的命令(发布-订阅模式)
    public ICommand BroadcastMessageCommand { get; }

    // 定义: 笔记文本
    public string NoteText
    {
        get => noteText;
        set
        {
            if (noteText == value)
            {
                return;
            }

            noteText = value;
            OnPropertyChanged();
        }
    }

    
    // 🔄 命令状态何时刷新？
   // 1.SelectedTask 属性被更新
   //  2.调用 RaiseCanExecuteChanged() 通知 WPF 重新检查命令状态 
   //      3.按钮自动启用或禁用
    public TaskItem? SelectedTask
    {
        get => selectedTask;
        set
        {
            if (selectedTask == value)
            {
                return;
            }

            selectedTask = value;
            OnPropertyChanged();//通知 UI 更新显示的数据 刷新数据绑定
            completeSelectedCommand.RaiseCanExecuteChanged();//通知 UI 更新按钮的启用/禁用状态 刷新命令可用性
        }
    }

    // 定义广播消息
    public string BroadcastText
    {
        get => broadcastText;
        set//2️⃣ 属性 setter 被调用（第 183-197 行）
        {
            if (broadcastText == value)
            {
                return;
            }

            broadcastText = value;
            OnPropertyChanged();
            broadcastCommand.RaiseCanExecuteChanged();
        }
    }
//定义接受消息
    public string ReceiverA
    {
        get => receiverA;
        set
        {
            if (receiverA == value)
            {
                return;
            }

            receiverA = value;
            OnPropertyChanged();
        }
    }

    public string ReceiverB
    {
        get => receiverB;
        set
        {
            if (receiverB == value)
            {
                return;
            }

            receiverB = value;
            OnPropertyChanged();
        }
    }
  //当外部代码执行赋值操作时，等号右边的结果会自动传递给 set 块中的 value。	value 的类型必须与属性定义的类型一致。•	value 只能在 set 代码块内部使用。
    // 主题切换  MVVM 模式中典型的属性实现，包含数据绑定通知机制。
    public bool IsPrimaryTheme
    {
        get => isPrimaryTheme;//返回私有字段的当前值
        set
        {
            if (isPrimaryTheme == value)//检查新值与旧值是否相同
            {
                return;
            }

            isPrimaryTheme = value;//更新私有字段为新值
            OnPropertyChanged();//OnPropertyChanged();	通知 UI 属性已变更
            //•	OnPropertyChanged() 是 INotifyPropertyChanged 接口的实现
            //•	它会触发 PropertyChanged 事件
            //•	WPF 数据绑定系统监听此事件，自动更新界面上绑定该属性的元素

        }
    }
// 定义: 属性变更通知事件
    public event PropertyChangedEventHandler? PropertyChanged;

    // 定义: 添加任务的方法
    private void AddTask()
    {
        Tasks.Add(new TaskItem
        {
            Title = $"新任务 {Tasks.Count + 1}",
            Category = "Practice",
            Priority = (Tasks.Count % 3) + 1,
            IsDone = false
        });
    }

    //TODO
    // 1. IsDone 属性被修改
    // ↓
    // 2. TaskItem 类触发 OnPropertyChanged("IsDone")
    // (假设 TaskItem 实现了 INotifyPropertyChanged)
    // ↓
    // 3. UI 上绑定的 CheckBox/图标自动更新
    // ↓
    // 4. 复选框从未选中变为选中状态 ✓
    private void CompleteSelected()
    {
        if (SelectedTask is null)
        {
            return;
        }

        SelectedTask.IsDone = true;
        TaskView.Refresh();//强制重新评估整个视图(包括分组、排序、筛选)
        completeSelectedCommand.RaiseCanExecuteChanged();//作用: 通知 WPF 重新检查命令的启用条件
    }

    // 定义: 广播消息的方法
    // 作用：
    // 获取当前时间（格式：HH:mm:ss，如 14:30:25）
    // 拼接用户输入的广播消息（BroadcastText）
    // 通过 MessageHub.Publish() 发布消息
    private void BroadcastMessage()
    {
        var message = $"{DateTime.Now:HH:mm:ss} - {BroadcastText}";
        MessageHub.Publish(message);
    }

    // 订阅者A
    private void OnMessagePublishedForReceiverA(string message)
    {
        ReceiverA = $"接收器A收到: {message}";
    }

    // 订阅者B
    private void OnMessagePublishedForReceiverB(string message)
    {
        ReceiverB = $"接收器B收到: {message}";
    }

    // 创建事件参数对象
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        //触发事件(调用所有订阅者)
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    //1. [CallerMemberName] 特性
    //[CallerMemberName]
    //string? propertyName = null
    //    •	作用：自动填入调用该方法的属性名
    //    •	好处：无需手动传参，避免写错字符串
    //// 在 NoteText 属性中调用
    //OnPropertyChanged();  // 自动传入 "NoteText"

    //// 等价于
    //OnPropertyChanged("NoteText");  // 手动传入

    //2. 空条件运算符?.
    //PropertyChanged?.Invoke(...)
    //•	作用：只有当 PropertyChanged 不为 null 时才触发
    //•	防止：没有订阅者时抛出空引用异常    
    //    ┌─────────────────────────────────────────────┐
    //│  PropertyChanged == null  →  什么都不做     │
    //│  PropertyChanged != null  →  触发事件      │
    //└─────────────────────────────────────────────┘

    //3. 事件触发
    //Invoke(this, new PropertyChangedEventArgs(propertyName))
    //•	this：事件发送者（当前 ViewModel 实例）
    //•	PropertyChangedEventArgs：携带变更的属性名

    //4.Invoke 是委托（Delegate）的调用方法，用于触发事件。

    //┌──────────────────────────────────────────────────────────────┐
    //│  1. 属性值被修改                                              │
    //│     noteText = "新值";                                        │
    //└─────────────────────┬────────────────────────────────────────┘
    //                      ▼
    //┌──────────────────────────────────────────────────────────────┐
    //│  2. 调用 OnPropertyChanged()                                  │
    //│     [CallerMemberName] 自动填入 "NoteText"                    │
    //└─────────────────────┬────────────────────────────────────────┘
    //                      ▼
    //┌──────────────────────────────────────────────────────────────┐
    //│  3. 触发 PropertyChanged 事件                                 │
    //│     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteText"))
    //└─────────────────────┬────────────────────────────────────────┘
    //                      ▼
    //┌──────────────────────────────────────────────────────────────┐
    //│  4. WPF 绑定系统收到通知                                      │
    //│     自动更新界面上绑定 NoteText 的所有元素                     │
    //└──────────────────────────────────────────────────────────────┘


    //等价的传统写法

    //// 表达式体（当前写法）
    //private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    //// 传统写法（功能完全相同）
    //private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    //{
    //    if (PropertyChanged != null)
    //    {
    //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}





//完整调用链（谁调用谁）
//    ┌─────────────────────────────────────────────────────────────────┐
//    │                      完整调用流程                               │
//    └─────────────────────────────────────────────────────────────────┘

//      步骤 1: 用户修改数据（代码或 UI 输入）
//      ┌──────────────────┐
//      │  NoteText = "新值" │
//      └────────┬─────────┘
//               │
//               ▼  你写的代码
//      步骤 2: 属性 set 块执行
//      ┌──────────────────┐
//      │  set {           │
//      │    noteText = value;
//      │    OnPropertyChanged();  ← 你主动调用
//      │  }               │
//      └────────┬─────────┘
//               │
//               ▼  你写的工具方法
//      步骤 3: OnPropertyChanged 执行
//      ┌──────────────────┐
//      │  OnPropertyChanged() {
//      │    PropertyChanged?.Invoke(...);  ← 触发事件
//      │  }               │
//      └────────┬─────────┘
//               │
//               ▼  事件机制
//      步骤 4: PropertyChanged 事件触发
//      ┌──────────────────┐
//      │  所有订阅者收到通知 │
//      │  (WPF 绑定系统)    │
//      └──────────────────┘

//1 什么是委托（Delegate）？
//    委托 = 方法的类型，就像 int 是数字的类型一样。
//    概念 类比  说明
//    int 数字类型    可以存储 1, 2, 3...
//    string 文本类型    可以存储 "hello"...
//    Delegate 方法类型    可以存储 方法A, 方法 B...
//    ┌─────────────────────────────────────────────────────────┐
//│                    委托的本质                            │
//├─────────────────────────────────────────────────────────┤
//│                                                         │
//│  委托 = 一个"容器"，里面可以装方法                        │
//│                                                         │
//│  就像变量存储数据，委托存储方法                          │
//│                                                         │
//└─────────────────────────────────────────────────────────┘
// 2 委托的定义和使用
//    定义委托类型
//    // 定义一个委托类型（规定方法的签名）
//public delegate void 我的委托 (string 消息);

//使用委托
//// 1. 定义方法
//void 方法 A(string 消息) { Console.WriteLine("A: " + 消息); }
//void 方法 B(string 消息) { Console.WriteLine("B: " + 消息); }

//// 2. 创建委托实例（把方法装进容器）
//我的委托 d = 方法 A;

//// 3. 调用委托（执行里面的方法）
//d("你好");  // 输出：A: 你好

//// 4. 可以装多个方法（委托链）
//d += 方法 B;
//d("大家好");  // 输出：A: 大家好  B: 大家好

//3 Invoke 是干什么的？
//    Invoke 是委托的调用方法，用来执行委托里存储的方法。
//    // 两种调用方式（完全等价）
//    d("你好");           // 直接调用
//    d.Invoke("你好");    // 使用 Invoke 方法

//    场景         说明
//    可读性 明确表示"这是委托调用"
//    链式调用 可以配合 ?. 空条件运算符
//    安全性 d?.Invoke() 避免空引用异常

// 安全调用（事件常用）
//PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteText"));

//// 等价于
//if (PropertyChanged != null)
//{
//    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NoteText"));
//}

//5.完整示例（从委托到事件）
//    // 1. 委托类型定义（.NET 已内置，无需自己写） 内置委托
//// public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

//// 2. 声明事件（基于委托）
//public event PropertyChangedEventHandler? PropertyChanged;

//// 3. 订阅事件（外部代码，如 WPF）
//viewModel.PropertyChanged += (sender, e) => {
//    Console.WriteLine($"属性 {e.PropertyName} 变更了");
//};

//// 4. 触发事件（内部代码，你的 ViewModel）
//PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoteText"));
//// ↑ 这里调用 Invoke this代表当前实例对象本身

//// 5. 结果：所有订阅者的方法都被执行