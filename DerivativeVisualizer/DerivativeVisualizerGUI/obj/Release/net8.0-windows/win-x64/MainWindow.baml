   M S B A M L   `   `   `  ���� R  NDerivativeVisualizerGUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:DerivativeVisualizerGUIDerivativeVisualizerGUI  U/clr-namespace:DerivativeVisualizerGUI.Behaviors!DerivativeVisualizerGUI.Behaviors  (    "DerivativeVisualizerGUI.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     7local%clr-namespace:DerivativeVisualizerGUI     R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   A	behaviors/clr-namespace:DerivativeVisualizerGUI.Behaviors     � Title$>  8Deriválási szabályok alkalmazása és szemléltetése��$	��700��$	��900��$	��700��$	��900�� � DataContext '   !DerivativeVisualizerGUI.ViewModel  .���  � RowDefinitions �� $��* ��� $��* �.+�� $��0�� � ColumnDefinitions �� $	��405 ��� $��* �.+��� $��0��$
��Auto=�$
��Auto=�.���� $��0��$	��375��$��10q�.+���  �� Orientation$ 
Horizontal=�$��	0,10,0,10q�.+��� $ 
Horizontal=�$��Center=�.+��� $��f(x) = ��$��20(�$��Center=��� $	��235��$��30�� �� 	MaxLength$ 20��$��20(����� *	InputText+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��30��$��30��$��10,0,0,0q� �� CornerRadius$ 5������ *
InputValid+   BooleanToBrushConverter	 �� 	Converter#	 [  ._��� $��16(�$
��Bold"�
�������
 �� TextAlignment$
 Center=�$��Center=�$��Center=����� *
InputValid+  BooleanToCheckConverter#	 [ �� $��?��$��30��$��30��$��5,0,0,0q�y��� *
InputValid+   BooleanToVisibilityConverter#	 [  �� ConverterParameter$ False������ *ToggleErrorMessageCommand+�� $��Függvények��$��	0,10,0,10q�.��A�  A� Rows$ 4�� A� Columns$ 5��$��0,3,0,0q�.+��� $	��log��$	��log������ *FunctionButtonCommand+�� $��ln��$��ln������ *FunctionButtonCommand+�� $	��sin��$	��sin������ *FunctionButtonCommand+�� $	��cos��$	��cos������ *FunctionButtonCommand+�� $��tg��$��tg������ *FunctionButtonCommand+�� $	��ctg��$	��ctg������ *FunctionButtonCommand+�� $��arcsin��$��arcsin������ *FunctionButtonCommand+�� $��arccos��$��arccos������ *FunctionButtonCommand+�� $��arctg��$��arctg������ *FunctionButtonCommand+�� $��arcctg��$��arcctg������ *FunctionButtonCommand+�� $��sh��$��sh������ *FunctionButtonCommand+�� $��ch��$��ch������ *FunctionButtonCommand+�� $��th��$��th������ *FunctionButtonCommand+�� $	��cth��$	��cth������ *FunctionButtonCommand+�� $
��arsh��$
��arsh������ *FunctionButtonCommand+�� $
��arch��$
��arch������ *FunctionButtonCommand+�� $
��arth��$
��arth������ *FunctionButtonCommand+�� $��arcth��$��arcth������ *FunctionButtonCommand+�� 
���  ��$��5,0,0,0q�$��12(�$
��Wrap=�$
��None=����� *ErrorMessage+y��� *ShowErrorMessage+#	 [ �� $��5,0,0,0q�$��20(�$
��Wrap=�$
��None=����� *DerivativeText+y��� *ShowDerivativeText+#	 [ �� $��Egyszerűsítés��$��5,10,0,0q�y��� *ShowSimplifyButton+#	 [ ���� *SimplifyCommand+�� $��1��$
��Auto=�$
��Auto=�D   >DerivativeVisualizerGUI.Behaviors.HorizontalScrollOnShiftWheel   Enable$
 True��y��� *TreeToPresent+  NullToVisibilityConverter#	 [ .��,   &DerivativeVisualizerGUI.BinaryTreeView  
�������   TreeToPresent �� *TreeToPresent+ �� � $��1�� �� $	��395 ��� $��* �.+�� $��0��$��20q�$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$ 
Horizontal=�$��0,0,0,10q�y��� *TreeToPresent+#	 [ .+��� $��Intervallum: [��$��Center=�$��20(��� $��65��$��20(�$ 6��$��30������ *StartInterval+$ TwoWay=�$ PropertyChanged=��� $	�� , ��$��Center=�$��20(��� $��65��$��20(�$ 6��$��30������ *EndInterval+$ TwoWay=�$ PropertyChanged=��� $��]��$��Center=�$��20(��� $��1��$ 
Horizontal=�.+��� $��Függvényrajzolás��
����ح�$��0,0,10,0q�$��16(����� *PlotFunctionCommand+y��� *TreeToPresent+#	 [ �� $#��Deriváltfüggvény-rajzolás��
������$��16(����� *PlotDerivativeCommand+y��� *ShowPlotDerivativeButton+#	 [ �� $��2��$ 
Horizontal=�$��
0,10,10,10q�y��� *ShowDerivativeAtAPoint+#	 [ .+��� $ ��Pontbeli deriválás: a = ��$��Center=�$��20(��� $��60��$��20(�$ 5��$��30������ *DerivativeAtAPointText+�� $��Érintőrajzolás��$��3��
��� ���$��	0,0,10,10q�$��16(�$
��Auto������ *PlotTangentCommand+y��� *ShowDerivativeAtAPoint+#	 [ �� $��4��$��20(�$��30��$
��Wrap=�$
��None=����� *ValueOfFunctionAtAPointText+y��� *#!ShowValueOfDerivativeAtAPointText+#	 [ �� $��5��$��20(�$��30��$
��Wrap=�$
��None=����� *ValueOfDerivativeAtAPointText+y��� *#!ShowValueOfDerivativeAtAPointText+#	 [ �� $��6��$��20(�$��30��$
��Wrap=�$
��None=����� *EquationOfTangentText+y��� *#!ShowValueOfDerivativeAtAPointText+#	 [   OxyPlot.Wpf.PlotView  $��1��$��10q�  OxyPlot.Wpf.PlotViewBase   Model �� *	PlotModel+