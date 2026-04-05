# WPF XAML 综合练习项目

这个项目用于集中练习常见的 XAML 核心能力，入口为 `MainWindow.xaml`。

## 已覆盖知识点

- 资源系统：`Application.Resources`、`Window.Resources`、`StaticResource`、`DynamicResource`
- 样式系统：隐式样式、显示样式、`BasedOn`（可自行扩展）
- 模板系统：`ControlTemplate`、`DataTemplate`、`TemplateBinding`
- 绑定系统：`ElementName`、`RelativeSource`、`TwoWay`、`UpdateSourceTrigger`
- 命令系统：`ICommand` + `RelayCommand`
- 触发器：`Trigger`、`EventTrigger`、`BeginStoryboard`
- 动画：`DoubleAnimation`
- 依赖属性：`Controls/RatingBadge.cs` 中的 `Score`、`BadgeBrush`
- 附加属性：`Helpers/HoverHelper.cs` 中的 `HighlightOnHover`
- 集合视图：`CollectionViewSource` 分组 + `GroupStyle`

## 运行

```bash
dotnet run
```

## 建议练习路径

1. 先修改 `App.xaml` 中主题色，观察 `DynamicResource` 的实时变化。
2. 修改 `TaskTemplate` 和 `PrimaryButtonStyle`，理解模板与样式边界。
3. 给 `RatingBadge` 增加新依赖属性（比如 `CornerRadius`），并在模板中使用。
4. 给 `HoverHelper` 增加附加属性参数，控制悬停边框颜色。
