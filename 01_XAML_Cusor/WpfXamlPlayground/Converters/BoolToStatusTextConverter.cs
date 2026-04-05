using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfXamlPlayground.Converters;

/// 1.BoolToStatusTextConverter
/// 作用：将bool值转换为中文状态文本的转换器
/// 用处：状态转换
/// 1.定义转换器类
  //2.再XAMK中声明
  //3.绑定中引用转换器

//参数：
//参数 类型  含义 当前代码中的用法
//value object                  绑定源的值（原始数据）	接收 IsDone 的 bool 值
//targetType Type           绑定目标属性的类型 目标为 TextBlock.Text，类型是 string
//parameter   object        转换器参数（可选）	当前未使用，可用于传递额外配置
//culture CultureInfo        文化信息（区域设置）	用于本地化，如日期/数字格式

//XAML 传递：
//- ConverterParameter='参数值'   →  object parameter
//- ConverterCulture='zh-CN'     →  CultureInfo culture
//-自动获取                     →  Type targetType

//C# 接收：
//public object Convert(object value, Type targetType, object parameter, CultureInfo culture)

//口诀：参数手动传，文化可指定，类型自动获
public class BoolToStatusTextConverter : IValueConverter
{
    /// <summary>
    /// 定义转换方法
    /// 参数：值，目标类型，参数，区域
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool done)
        {
            return done ? "已完成" : "进行中";
        }

        return "未知状态";
    }
    /// <summary>
    /// 作用：反向转换
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string text && text == "已完成";
    }
}
//┌─────────────────────────────────────────────────────────────────┐
//│  双向绑定完整流程                                                │
//│                                                                 │
//│  ViewModel(bool)                                               │
//│  ┌─────────────┐                                               │
//│  │ IsDone      │                                               │
//│  │ true/false  │                                               │
//│  └─────────────┘                                               │
//│         ↑↓                                                      │
//│         │ Convert()      显示数据时                             │
//│         │ (bool→string)                                         │
//│         ↓                                                       │
//│  UI (string)                                                    │
//│  ┌─────────────┐                                               │
//│  │ ComboBox    │                                               │
//│  │ "已完成"/"进行中" │                                           │
//│  └─────────────┘                                               │
//│         ↑                                                       │
//│         │ ConvertBack()   用户修改时                            │
//│         │ (string→bool)                                         │
//│         │                                                       │
//└─────────────────────────────────────────────────────────────────┘