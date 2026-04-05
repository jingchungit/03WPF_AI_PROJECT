namespace WpfXamlPlayground.Models;

public class TaskItem //任务数据模型,用于表示任务所属的分类,是实现 列表分组（Grouping） 的关键字段
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public bool IsDone { get; set; }
    public int Priority { get; set; }
}
