我来为你生成一份全面的 C# 语言技术栈指南文档！

# C# 语言核心技术栈完整指南

> 从基础到高级，C# 开发者必备知识全景图

---

## 📚 目录

1. [语言基础](#1-语言基础)
2. [面向对象编程](#2-面向对象编程)
3. [泛型与集合](#3-泛型与集合)
4. [LINQ 查询](#4-linq-查询)
5. [异步编程](#5-异步编程)
6. [委托与事件](#6-委托与事件)
7. [反射与元数据](#7-反射与元数据)
8. [特性（Attributes）](#8-特性attributes)
9. [模式匹配](#9-模式匹配)
10. [记录类型与不可变性](#10-记录类型与不可变性)
11. [可空引用类型](#11-可空引用类型)
12. [表达式树](#12-表达式树)
13. [源生成器](#13-源生成器)
14. [Span<T> 与内存管理](#14-spant-与内存管理)
15. [设计模式实战](#15-设计模式实战)

---

## 1. 语言基础

### 变量与数据类型

```
csharp
// 值类型
int age = 25;
double price = 19.99;
bool isActive = true;
char grade = 'A';
decimal money = 100.50m;
DateTime now = DateTime.Now;

// 引用类型
string name = "Lucien";
object obj = "anything";
dynamic dyn = 10; // 动态类型

// 隐式类型
var list = new List<string>(); // 编译器推断为 List<string>

// 可空值类型
int? nullableInt = null;
double? nullableDouble = 3.14;

// 元组
var person = (Name: "Lucien", Age: 25, City: "Beijing");
Console.WriteLine(person.Name); // "Lucien"

// 解构
(string name, int age) = person;
```
### 字符串操作

```
csharp
// 字符串插值（推荐）
string name = "Lucien";
int age = 25;
string message = $"Hello, {name}! You are {age} years old.";

// 逐字字符串
string path = @"C:\Users\Lucien\Desktop";

// StringBuilder（高性能拼接）
var sb = new StringBuilder();
sb.Append("Hello");
sb.AppendLine(" World");
string result = sb.ToString();

// 常用方法
string text = "  Hello World  ";
text.Trim();           // "Hello World"
text.ToUpper();        // "  HELLO WORLD  "
text.ToLower();        // "  hello world  "
text.Contains("Hello"); // true
text.StartsWith("  ");  // true
text.EndsWith("  ");    // true
text.Replace("World", "C#"); // "  Hello C#  "
text.Split(' ');       // string[]

// 字符串格式化
string formatted = string.Format("{0:D2}/{1:D2}/{2:yyyy}", 1, 15, 2024);
// "01/15/2024"
```
### 控制流

```
csharp
// if-else
if (age >= 18)
{
    Console.WriteLine("Adult");
}
else if (age >= 13)
{
    Console.WriteLine("Teenager");
}
else
{
    Console.WriteLine("Child");
}

// switch 表达式（C# 8+）
string category = age switch
{
    < 13 => "Child",
    < 18 => "Teenager",
    < 65 => "Adult",
    _ => "Senior"
};

// for 循环
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}

// foreach
var names = new[] { "Alice", "Bob", "Charlie" };
foreach (var name in names)
{
    Console.WriteLine(name);
}

// while
int count = 0;
while (count < 5)
{
    Console.WriteLine(count++);
}

// do-while
do
{
    Console.WriteLine("At least once");
} while (false);
```
### 异常处理

```
csharp
try
{
    var result = 10 / 0;
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"除零错误：{ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"未知错误：{ex.Message}");
}
finally
{
    Console.WriteLine("总是执行");
}

// 自定义异常
public class TaskNotFoundException : Exception
{
    public int TaskId { get; }

    public TaskNotFoundException(int taskId)
        : base($"Task with ID {taskId} not found")
    {
        TaskId = taskId;
    }
}

// 抛出异常
throw new TaskNotFoundException(123);

// 异常过滤器（C# 6+）
try
{
    // 可能抛出异常的代码
}
catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
{
    // 只捕获特定异常
}
```
---

## 2. 面向对象编程

### 类与对象

```
csharp
// 基础类
public class Person
{
    // 字段
    private string _name;
    
    // 属性
    public string Name 
    { 
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public int Age { get; set; }
    public string Email { get; private set; } = string.Empty;
    
    // 自动属性
    public string Phone { get; set; } = string.Empty;
    
    // 只读属性
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    // 构造函数
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
    
    // 方法
    public void Introduce()
    {
        Console.WriteLine($"Hi, I'm {Name}, {Age} years old.");
    }
    
    // 静态方法
    public static Person CreateDefault()
    {
        return new Person("Unknown", 0);
    }
}

// 使用
var person = new Person("Lucien", 25);
person.Introduce();
```
### 继承

```
csharp
// 基类
public class Animal
{
    public string Name { get; set; }
    
    public virtual void Speak()
    {
        Console.WriteLine($"{Name} makes a sound");
    }
    
    public void Sleep()
    {
        Console.WriteLine($"{Name} is sleeping");
    }
}

// 派生类
public class Dog : Animal
{
    public string Breed { get; set; }
    
    // 重写虚方法
    public override void Speak()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
    
    // 新方法
    public void Fetch()
    {
        Console.WriteLine($"{Name} is fetching");
    }
}

// 密封类（不能被继承）
public sealed class GoldenRetriever : Dog
{
    // ...
}

// 使用
Dog dog = new Dog { Name = "Buddy", Breed = "Labrador" };
dog.Speak(); // "Buddy says: Woof!"
```
### 抽象类与接口

```
csharp
// 抽象类
public abstract class Shape
{
    public string Color { get; set; }
    
    // 抽象方法（必须在派生类中实现）
    public abstract double Area();
    
    // 虚方法（可以提供默认实现）
    public virtual void Draw()
    {
        Console.WriteLine($"Drawing a {Color} shape");
    }
}

// 接口
public interface IDrawable
{
    void Draw();
    int GetVertexCount();
    
    // 默认实现（C# 8+）
    string GetDescription()
    {
        return $"Drawable with {GetVertexCount()} vertices";
    }
}

public interface IScalable
{
    void Scale(double factor);
}

// 实现
public class Circle : Shape, IDrawable, IScalable
{
    public double Radius { get; set; }
    
    public override double Area() => Math.PI * Radius * Radius;
    
    public void Draw()
    {
        Console.WriteLine($"Drawing a circle with radius {Radius}");
    }
    
    public int GetVertexCount() => 0;
    
    public void Scale(double factor)
    {
        Radius *= factor;
    }
}

public class Rectangle : Shape, IDrawable
{
    public double Width { get; set; }
    public double Height { get; set; }
    
    public override double Area() => Width * Height;
    
    public void Draw()
    {
        Console.WriteLine($"Drawing a rectangle {Width}x{Height}");
    }
    
    public int GetVertexCount() => 4;
}
```
### 多态

```
csharp
// 运行时多态
List<Shape> shapes = new List<Shape>
{
    new Circle { Radius = 5, Color = "Red" },
    new Rectangle { Width = 10, Height = 5, Color = "Blue" }
};

foreach (var shape in shapes)
{
    Console.WriteLine($"Area: {shape.Area():F2}");
    shape.Draw();
}

// 接口多态
List<IDrawable> drawables = new List<IDrawable>
{
    new Circle { Radius = 5 },
    new Rectangle { Width = 10, Height = 5 }
};

foreach (var drawable in drawables)
{
    drawable.Draw();
}
```
### 访问修饰符

| 修饰符 | 说明 |
|--------|------|
| `public` | 任何代码都可访问 |
| `private` | 仅类内部可访问 |
| `protected` | 类和派生类可访问 |
| `internal` | 同一程序集内可访问 |
| `protected internal` | protected OR internal |
| `private protected` | 同一程序集的派生类 |

---

## 3. 泛型与集合

### 泛型基础

```
csharp
// 泛型类
public class Repository<T> where T : class
{
    private readonly List<T> _items = new();
    
    public void Add(T item) => _items.Add(item);
    public T? GetById(int id) => _items.FirstOrDefault();
    public List<T> GetAll() => _items;
}

// 泛型约束
public class GenericService<T> where T : IEquatable<T>, new()
{
    public bool AreEqual(T a, T b) => a.Equals(b);
    public T CreateInstance() => new T();
}

// 多个约束
public class ComplexService<T, TKey> 
    where T : class, IDisposable 
    where TKey : IComparable<TKey>
{
    // ...
}
```
### 常用集合

```
csharp
// List<T> - 动态数组
var list = new List<int> { 1, 2, 3, 4, 5 };
list.Add(6);
list.Remove(1);
list.Sort();

// Dictionary<TKey, TValue> - 哈希表
var dict = new Dictionary<string, int>
{
    ["Alice"] = 25,
    ["Bob"] = 30
};
dict["Charlie"] = 35;
if (dict.TryGetValue("Alice", out int age))
{
    Console.WriteLine(age);
}

// HashSet<T> - 唯一元素集合
var unique = new HashSet<int> { 1, 2, 2, 3, 3, 3 }; // {1, 2, 3}
unique.Add(4);
unique.IntersectWith(new[] { 2, 3, 4 }); // {2, 3, 4}

// Queue<T> - 队列（FIFO）
var queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
var first = queue.Dequeue(); // "First"

// Stack<T> - 栈（LIFO）
var stack = new Stack<string>();
stack.Push("First");
stack.Push("Second");
var top = stack.Pop(); // "Second"

// LinkedList<T> - 双向链表
var linked = new LinkedList<int>();
linked.AddLast(1);
linked.AddLast(2);
linked.AddFirst(0);

// SortedList<TKey, TValue> - 排序字典
var sorted = new SortedList<string, int>
{
    ["B"] = 2,
    ["A"] = 1,
    ["C"] = 3
};
// 按键排序：A=1, B=2, C=3
```
### 线程安全集合

```
csharp
using System.Collections.Concurrent;

// ConcurrentDictionary
var concurrentDict = new ConcurrentDictionary<string, int>();
concurrentDict.TryAdd("key1", 1);
concurrentDict.TryUpdate("key1", 2, 1);
concurrentDict.AddOrUpdate("key2", 1, (k, v) => v + 1);

// ConcurrentQueue
var concurrentQueue = new ConcurrentQueue<int>();
concurrentQueue.Enqueue(1);
concurrentQueue.TryDequeue(out int item);

// ConcurrentBag（无序）
var bag = new ConcurrentBag<int>();
bag.Add(1);
bag.TryTake(out int result);

// BlockingCollection（生产者-消费者）
var blocking = new BlockingCollection<int>(boundedCapacity: 10);
// 生产者
Task.Run(() =>
{
    for (int i = 0; i < 100; i++)
    {
        blocking.Add(i);
    }
    blocking.CompleteAdding();
});

// 消费者
foreach (var item in blocking.GetConsumingEnumerable())
{
    Console.WriteLine(item);
}
```
---

## 4. LINQ 查询

### 方法语法 vs 查询语法

```
csharp
var numbers = Enumerable.Range(1, 100).ToList();

// 方法语法（推荐）
var result = numbers
    .Where(n => n % 2 == 0)
    .OrderByDescending(n => n)
    .Select(n => n * 2)
    .Take(10)
    .ToList();

// 查询语法
var query = from n in numbers
            where n % 2 == 0
            orderby n descending
            select n * 2;
```
### 常用 LINQ 操作

```
csharp
var people = new List<Person>
{
    new("Alice", 25),
    new("Bob", 30),
    new("Charlie", 35),
    new("David", 25)
};

// 过滤
var adults = people.Where(p => p.Age >= 18);
var firstAdult = people.FirstOrDefault(p => p.Age >= 18);

// 投影
var names = people.Select(p => p.Name);
var anonymous = people.Select(p => new { p.Name, IsAdult = p.Age >= 18 });

// 排序
var sorted = people.OrderBy(p => p.Age).ThenBy(p => p.Name);
var descending = people.OrderByDescending(p => p.Age);

// 分组
var grouped = people.GroupBy(p => p.Age);
foreach (var group in grouped)
{
    Console.WriteLine($"Age {group.Key}: {group.Count()} people");
}

// 聚合
var totalAge = people.Sum(p => p.Age);
var avgAge = people.Average(p => p.Age);
var maxAge = people.Max(p => p.Age);
var minAge = people.Min(p => p.Age);
var count = people.Count();

// 连接
var orders = new List<Order> { ... };
var query = from p in people
            join o in orders on p.Name equals o.CustomerName
            select new { Person = p, Order = o };

// 去重
var distinct = people.DistinctBy(p => p.Age);

// 分页
var page = people.Skip(10).Take(5);

// 检查条件
bool hasAdult = people.Any(p => p.Age >= 18);
bool allAdults = people.All(p => p.Age >= 18);
bool containsAlice = people.Any(p => p.Name == "Alice");

// 集合操作
var list1 = new[] { 1, 2, 3, 4 };
var list2 = new[] { 3, 4, 5, 6 };
var union = list1.Union(list2);           // {1, 2, 3, 4, 5, 6}
var intersect = list1.Intersect(list2);   // {3, 4}
var except = list1.Except(list2);         // {1, 2}
```
### LINQ to Objects vs LINQ to Entities

```
csharp
// LINQ to Objects（内存中）
var inMemory = people.Where(p => p.Age > 25).ToList();

// LINQ to Entities（数据库查询）
var fromDb = dbContext.People
    .Where(p => p.Age > 25)
    .OrderBy(p => p.Name)
    .ToListAsync();

// ⚠️ 注意：避免在 EF Core 中使用不支持的方法
// ❌ 可能导致客户端评估
var bad = dbContext.People
    .Where(p => SomeComplexMethod(p.Name))
    .ToList();

// ✅ 先获取数据再处理
var good = dbContext.People
    .Where(p => p.Age > 25)
    .ToList()
    .Where(p => SomeComplexMethod(p.Name));
```
---

## 5. 异步编程

### async/await 基础

```
csharp
// 异步方法
public async Task<string> GetDataAsync()
{
    await Task.Delay(1000); // 模拟异步操作
    return "Data";
}

// 调用异步方法
string data = await GetDataAsync();

// 带返回值的异步方法
public async Task<int> CalculateAsync()
{
    await Task.Delay(100);
    return 42;
}

// 无返回值的异步方法
public async Task ProcessAsync()
{
    await Task.Delay(100);
    Console.WriteLine("Done");
}
```
### 并行执行

```
csharp
// 串行执行（慢）
var result1 = await GetDataAsync();
var result2 = await GetOtherDataAsync();

// 并行执行（快）✅
var task1 = GetDataAsync();
var task2 = GetOtherDataAsync();
await Task.WhenAll(task1, task2);
var r1 = await task1;
var r2 = await task2;

// 取第一个完成的任务
var tasks = new[] { task1, task2, task3 };
var first = await Task.WhenAny(tasks);
```
### 取消令牌

```
csharp
public async Task LongRunningOperationAsync(CancellationToken ct = default)
{
    for (int i = 0; i < 100; i++)
    {
        ct.ThrowIfCancellationRequested();
        
        await Task.Delay(100, ct);
        Console.WriteLine($"Progress: {i}%");
    }
}

// 使用
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    await LongRunningOperationAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("操作已取消");
}
```
### 异步最佳实践

```
csharp
// ✅ 正确：异步方法命名以 Async 结尾
public async Task SaveAsync() { }

// ✅ 正确：使用 ConfigureAwait(false) 在库代码中
public async Task ProcessAsync()
{
    await DoWorkAsync().ConfigureAwait(false);
}

// ❌ 错误：避免 async void（除事件处理外）
public async void BadMethod() { }

// ❌ 错误：避免 Result/Wait
var result = GetDataAsync().Result; // 可能死锁
GetDataAsync().Wait(); // 阻塞线程

// ✅ 正确：异步 Main（C# 7.1+）
static async Task Main(string[] args)
{
    await RunAsync();
}
```
### ValueTask（高性能场景）

```
csharp
// 当结果可能立即可用时使用 ValueTask
public async ValueTask<int> GetValueAsync()
{
    if (_cache.TryGetValue("key", out int value))
    {
        return value; // 同步返回，无分配
    }
    
    return await FetchFromDatabaseAsync();
}
```
---

## 6. 委托与事件

### 委托类型

```
csharp
// ① Action（无返回值）
Action action = () => Console.WriteLine("Hello");
Action<string> print = msg => Console.WriteLine(msg);
action();
print("World");

// ② Func（有返回值）
Func<int, int, int> add = (a, b) => a + b;
int result = add(3, 4); // 7

Func<string> getName = () => "Lucien";

// ③ Predicate（返回 bool）
Predicate<int> isEven = x => x % 2 == 0;

// ④ 自定义委托
public delegate void MessageHandler(string message);
public event MessageHandler OnMessage;
```
### Lambda 表达式

```
csharp
// 语句 lambda
Action greet = () =>
{
    Console.WriteLine("Hello");
    Console.WriteLine("World");
};

// 表达式 lambda
Func<int, int> square = x => x * x;

// 多参数
Func<int, int, int> multiply = (x, y) => x * y;

// 闭包
int multiplier = 5;
Func<int, int> multiplyBy = x => x * multiplier;
```
### 事件

```
csharp
public class Publisher
{
    // 定义事件
    public event EventHandler<string>? MessagePublished;
    
    public void Publish(string message)
    {
        Console.WriteLine($"Publishing: {message}");
        MessagePublished?.Invoke(this, message);
    }
}

public class Subscriber
{
    public void Subscribe(Publisher publisher)
    {
        publisher.MessagePublished += OnMessageReceived;
    }
    
    private void OnMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Received: {message}");
    }
    
    public void Unsubscribe(Publisher publisher)
    {
        publisher.MessagePublished -= OnMessageReceived;
    }
}

// 使用
var pub = new Publisher();
var sub = new Subscriber();
sub.Subscribe(pub);
pub.Publish("Hello World");
```
### 内置委托

```
csharp
// EventHandler
public event EventHandler<EventArgs>? Click;

// EventHandler<TEventArgs>
public class TaskEventArgs : EventArgs
{
    public string TaskName { get; set; }
}
public event EventHandler<TaskEventArgs>? TaskCompleted;

// 触发事件
OnTaskCompleted?.Invoke(this, new TaskEventArgs { TaskName = "Task1" });
```
---

## 7. 反射与元数据

### 类型信息

```
csharp
Type type = typeof(string);
Console.WriteLine(type.Name);           // "String"
Console.WriteLine(type.FullName);       // "System.String"
Console.WriteLine(type.Namespace);      // "System"

// 获取类型的成员
var methods = type.GetMethods();
var properties = type.GetProperties();
var fields = type.GetFields();

// 实例的类型
string text = "Hello";
Type instanceType = text.GetType();
```
### 动态创建对象

```
csharp
// Activator
var list = Activator.CreateInstance<List<int>>();

// 通过类型
Type type = Type.GetType("System.Collections.Generic.List`1[System.Int32]");
var instance = Activator.CreateInstance(type);

// 调用方法
var method = type.GetMethod("Add");
method?.Invoke(instance, new object[] { 42 });
```
### 读取属性

```
csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var person = new Person { Name = "Lucien", Age = 25 };
Type type = person.GetType();

foreach (var prop in type.GetProperties())
{
    Console.WriteLine($"{prop.Name}: {prop.GetValue(person)}");
}
// Name: Lucien
// Age: 25

// 设置属性
type.GetProperty("Name")?.SetValue(person, "Alice");
```
### 泛型反射

```
csharp
public class Repository<T>
{
    public void Save(T item) { }
}

var repoType = typeof(Repository<>);
var concreteType = repoType.MakeGenericType(typeof(Person));
var repo = Activator.CreateInstance(concreteType);
```
---

## 8. 特性（Attributes）

### 内置特性

```
csharp
// Obsolete - 标记过时
[Obsolete("Use NewMethod instead")]
public void OldMethod() { }

// Conditional - 条件编译
[Conditional("DEBUG")]
public void DebugLog(string message)
{
    Console.WriteLine(message);
}

// AttributeUsage - 限制特性使用
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAttribute : Attribute { }

// CallerMemberName - 自动获取调用者名称
public void Log([CallerMemberName] string memberName = "")
{
    Console.WriteLine($"Called from: {memberName}");
}

// Other common attributes
[Serializable]
[NonSerialized]
[Flags]
[StructLayout(LayoutKind.Sequential)]
```
### 自定义特性

```
csharp
[AttributeUsage(AttributeTargets.Property)]
public class ValidateAttribute : Attribute
{
    public bool Required { get; set; }
    public int MinLength { get; set; }
    public int MaxLength { get; set; }

    public ValidateAttribute(bool required = false)
    {
        Required = required;
    }
}

// 使用
public class User
{
    [Validate(Required = true, MinLength = 3, MaxLength = 50)]
    public string Name { get; set; }
    
    [Validate(Required = true)]
    public string Email { get; set; }
}

// 验证器
public class Validator
{
    public static List<string> Validate(object obj)
    {
        var errors = new List<string>();
        var type = obj.GetType();
        
        foreach (var prop in type.GetProperties())
        {
            var attr = prop.GetCustomAttribute<ValidateAttribute>();
            if (attr == null) continue;
            
            var value = prop.GetValue(obj) as string;
            
            if (attr.Required && string.IsNullOrEmpty(value))
            {
                errors.Add($"{prop.Name} is required");
            }
            
            if (value != null)
            {
                if (value.Length < attr.MinLength)
                    errors.Add($"{prop.Name} too short");
                if (value.Length > attr.MaxLength)
                    errors.Add($"{prop.Name} too long");
            }
        }
        
        return errors;
    }
}
```
---

## 9. 模式匹配

### 类型模式

```
csharp
object obj = "Hello";

// is 表达式
if (obj is string text)
{
    Console.WriteLine(text.Length);
}

// switch 表达式
string description = obj switch
{
    string s => $"String: {s}",
    int i => $"Integer: {i}",
    null => "Null",
    _ => "Unknown"
};
```
### 位置模式

```
csharp
public record Point(int X, int Y);

Point point = new Point(3, 4);

// 解构模式
if (point is (3, 4))
{
    Console.WriteLine("Origin nearby");
}

// switch
string quadrant = point switch
{
    (0, 0) => "Origin",
    (> 0, > 0) => "Quadrant I",
    (< 0, > 0) => "Quadrant II",
    (< 0, < 0) => "Quadrant III",
    (> 0, < 0) => "Quadrant IV",
    _ => "Axis"
};
```
### 关系模式（C# 9+）

```
csharp
int age = 25;

string category = age switch
{
    < 13 => "Child",
    < 20 => "Teenager",
    < 65 => "Adult",
    >= 65 => "Senior"
};
```
### 逻辑模式

```
csharp
// and, or, not
if (obj is string { Length: > 5 } and not "test")
{
    Console.WriteLine("Long string, not 'test'");
}

// 组合
string result = value switch
{
    >= 0 and <= 100 => "Valid",
    < 0 or > 100 => "Invalid",
    _ => "Unknown"
};
```
### 列表模式（C# 11+）

```
csharp
int[] numbers = { 1, 2, 3 };

bool matches = numbers is [1, 2, 3];
bool startsWithOne = numbers is [1, ..];
bool endsWithThree = numbers is [.., 3];
bool hasThreeElements = numbers is [_, _, _];
```
---

## 10. 记录类型与不可变性

### Record

```
csharp
// 位置记录
public record Person(string Name, int Age);

// 使用
var person1 = new Person("Lucien", 25);
var person2 = new Person("Lucien", 25);

Console.WriteLine(person1 == person2); // true（值相等）
Console.WriteLine(person1.Equals(person2)); // true

// with 表达式（非破坏性变更）
var older = person1 with { Age = 26 };
Console.WriteLine(older); // Person { Name = Lucien, Age = 26 }

// 传统记录
public record Student
{
    public string Name { get; init; }
    public int Age { get; init; }
    public string Major { get; set; } // 可变
    
    public Student(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
```
### Record Struct（C# 10+）

```
csharp
public record struct Point(int X, int Y);

// 值类型，性能更好
var p1 = new Point(1, 2);
var p2 = p1 with { X = 3 };
```
### 不可变集合

```
csharp
using System.Collections.Immutable;

// ImmutableList
var list = ImmutableList.Create(1, 2, 3);
var newList = list.Add(4); // 创建新列表

// ImmutableDictionary
var dict = ImmutableDictionary.Create<string, int>()
    .Add("Alice", 25)
    .Add("Bob", 30);

// ImmutableHashSet
var set = ImmutableHashSet.Create(1, 2, 3);
```
---

## 11. 可空引用类型

### 启用可空上下文

```
csharp
// 在项目文件中启用
// <Nullable>enable</Nullable>

#nullable enable

string nonNullable = "Hello"; // 不能为 null
string? nullable = null;      // 可以为 null

// 警告
// string s = null; // ⚠️ 警告

// 空值检查
if (nullable != null)
{
    Console.WriteLine(nullable.Length); // ✅ 安全
}

// 空合并运算符
string result = nullable ?? "default";
string result2 = nullable ?? throw new ArgumentNullException();

// 空条件运算符
int? length = nullable?.Length;

// 断言不为 null
void Process(string? value)
{
    if (value is null)
        throw new ArgumentNullException();
    
    Console.WriteLine(value.Length); // ✅ 编译器知道不为 null
}

// ! 运算符（空原谅）
string definitelyNotNull = nullable!;
```
### 可空注解

```
csharp
public class UserService
{
    // 可能返回 null
    public User? FindById(int id) { ... }
    
    // 参数不能为 null
    public void CreateUser(string name) { ... }
    
    // 参数和返回值都可能为 null
    public string? FormatName(string? name) { ... }
}
```
---

## 12. 表达式树

### 基础

```
csharp
using System.Linq.Expressions;

// Lambda 表达式
Expression<Func<int, int>> expr = x => x * 2;

// 编译执行
var func = expr.Compile();
Console.WriteLine(func(5)); // 10

// 手动构建表达式树
ParameterExpression param = Expression.Parameter(typeof(int), "x");
ConstantExpression constant = Expression.Constant(2);
BinaryExpression multiply = Expression.Multiply(param, constant);
Expression<Func<int, int>> lambda = Expression.Lambda<Func<int, int>>(multiply, param);
```
### 动态查询

```
csharp
public IQueryable<T> Filter<T>(IQueryable<T> query, string propertyName, object value)
{
    var parameter = Expression.Parameter(typeof(T), "x");
    var property = Expression.Property(parameter, propertyName);
    var constant = Expression.Constant(value);
    var equality = Expression.Equal(property, constant);
    var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);
    
    return query.Where(lambda);
}

// 使用
var filtered = Filter(dbContext.Users, "Name", "Lucien");
```
---

## 13. 源生成器

### 安装

```
bash
dotnet add package Microsoft.CodeAnalysis.CSharp
```
### 简单示例

```
csharp
[Generator]
public class HelloWorldGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }
    
    public void Execute(GeneratorExecutionContext context)
    {
        string source = @"
namespace Generated
{
    public static class Greeter
    {
        public static void SayHello()
        {
            System.Console.WriteLine(""Hello from generated code!"");
        }
    }
}";
        context.AddSource("Greeter.g.cs", source);
    }
}
```
### 使用生成的代码

```
csharp
Generated.Greeter.SayHello();
```
---

## 14. Span<T> 与内存管理

### Span<T> 基础

```
csharp
// 栈上分配，零拷贝
int[] array = { 1, 2, 3, 4, 5 };
Span<int> span = array.AsSpan();

// 切片（无分配）
Span<int> slice = span[1..3]; // {2, 3}

// ReadOnlySpan
ReadOnlySpan<char> text = "Hello World".AsSpan();
ReadOnlySpan<char> substring = text[0..5]; // "Hello"
```
### Memory<T>

```
csharp
// 可以存储在字段中
public class BufferManager
{
    private Memory<byte> _buffer;
    
    public void SetBuffer(Memory<byte> buffer)
    {
        _buffer = buffer;
    }
}

// 异步支持
async Task ProcessAsync(Memory<byte> buffer)
{
    await stream.ReadAsync(buffer);
}
```
### 高性能字符串操作

```
csharp
// 避免字符串分配
bool StartsWithVowel(ReadOnlySpan<char> text)
{
    return text.Length > 0 && "aeiouAEIOU".AsSpan().IndexOf(text[0]) >= 0;
}

// 解析
int ParseNumber(ReadOnlySpan<char> text)
{
    return int.Parse(text);
}
```
### ArrayPool

```
csharp
// 复用数组，减少 GC 压力
var pool = ArrayPool<byte>.Shared;
byte[] buffer = pool.Rent(1024);

try
{
    // 使用 buffer
}
finally
{
    pool.Return(buffer);
}
```
---

## 15. 设计模式实战

### 单例模式

```
csharp
public sealed class Singleton
{
    private static readonly Lazy<Singleton> _instance = 
        new(() => new Singleton());
    
    public static Singleton Instance => _instance.Value;
    
    private Singleton() { }
}
```
### 工厂模式

```
csharp
public interface IProduct { }
public class ConcreteProductA : IProduct { }
public class ConcreteProductB : IProduct { }

public class ProductFactory
{
    public IProduct Create(string type) => type switch
    {
        "A" => new ConcreteProductA(),
        "B" => new ConcreteProductB(),
        _ => throw new ArgumentException("Unknown type")
    };
}
```
### 观察者模式

```
csharp
public interface IObserver
{
    void Update(string message);
}

public class Subject
{
    private readonly List<IObserver> _observers = new();
    
    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);
    
    public void Notify(string message)
    {
        foreach (var observer in _observers)
        {
            observer.Update(message);
        }
    }
}
```
### 策略模式

```
csharp
public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid ${amount} by credit card");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid ${amount} via PayPal");
    }
}

public class PaymentService
{
    private IPaymentStrategy _strategy;
    
    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void ProcessPayment(decimal amount)
    {
        _strategy.Pay(amount);
    }
}
```
### 依赖注入

```
csharp
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
}

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    
    public EfRepository(DbContext context)
    {
        _context = context;
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
```
---

## 🎯 C# 版本特性速查

### C# 8.0
- 可空引用类型
- 异步流（IAsyncEnumerable）
- 索引和范围（`^`, `..`）
- Switch 表达式
- 默认接口方法

### C# 9.0
- Records
- Init-only 属性
- Top-level statements
- Pattern matching enhancements
- Target-typed new expressions

### C# 10.0
- Record structs
- Global using directives
- File-scoped namespaces
- Improved structure patterns
- Constant interpolated strings

### C# 11.0
- Raw string literals
- List patterns
- Generic attributes
- Required members
- UTF-8 string literals

### C# 12.0
- Primary constructors
- Collection expressions
- Alias any type
- Inline arrays
- Experimental attribute

---

## 📖 学习资源

### 官方文档
- [C# Programming Guide](https://docs.microsoft.com/dotnet/csharp/)
- [C# Language Reference](https://docs.microsoft.com/dotnet/csharp/language-reference/)

### 书籍
- 《C# in Depth》by Jon Skeet
- 《Effective C#》by Bill Wagner
- 《CLR via C#》by Jeffrey Richter

### 在线学习
- Microsoft Learn - C# 路径
- Pluralsight - C# 课程
- Udemy - Complete C# Masterclass

---

**最后更新**：2026-04-24  
**适用版本**：C# 8.0 - C# 12.0
```


这份文档涵盖了 C# 语言的核心技术栈，从基础到高级特性都有详细说明。你可以将它保存为 `CSharp技术栈指南.md` 文件，作为你的 C# 学习参考手册！🚀