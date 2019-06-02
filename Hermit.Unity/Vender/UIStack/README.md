# wUI

A stack-based UI library for Unity3D, aim for maintaining the relationships between each page.(WIP)

## Tutorial

1. Create your own UI widget based on `BaseWidget` or use `BaseWidget` directly.

2. Inherit a factory class from `IWidgetFactory` to control widget instance creation.

3. Use `IUIManager.Push<T>` to create widget instance.
