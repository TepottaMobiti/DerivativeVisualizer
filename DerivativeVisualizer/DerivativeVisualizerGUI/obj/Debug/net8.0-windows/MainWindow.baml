   M S B A M L   `   `   `  ����R  NDerivativeVisualizerGUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:DerivativeVisualizerGUIDerivativeVisualizerGUI  5   	   (    "DerivativeVisualizerGUI.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:DerivativeVisualizerGUI     5   	   R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5   	     � Title$  
MainWindow��5	   	   $	��450��6   $	��800��6)    � WindowState$ 	Maximized=�5
   	    � DataContext 5      '   !DerivativeVisualizerGUI.ViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      �� $��0��6   $	��375��6)   $��10q�65   .+�5       ��  �� Orientation$ 
Horizontal=�6   $��	0,10,0,10q�66   .+�5!      �� $ 
Horizontal=�6!   $��Center=�6:   .+�5"      �� $��f(x) = ��6    $��20(�62   $��Center=�6@   �� 5#      $	��235��5$      $��30��6    �� 	MaxLength$ 20��5%      $��20(�5&      ��5#   "   �� *	InputText+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5&      5'      �� 5*      $��30��6   $��30��6(   $��10,0,0,0q�64   	 �� CornerRadius$	 5��5,      ��5+      �� *
InputValid+   BooleanToBrushConverter
 �� 	Converter#
 [  ._�5-      �� $��16(�5.      $
��Bold"�6&   
������� �� TextAlignment$ Center=�5/   +   $��Center=�50      $��Center=�63   ��5-   $   �� *
InputValid+  BooleanToCheckConverter#
 [ 50   3   51      �� 54      $��?��6   $��30��6)   $��30��64   $��5,0,0,0q�6@   y�55      �� *
InputValid+   BooleanToVisibilityConverter#
 [  �� ConverterParameter$ False����56      �� *ToggleErrorMessageCommand+57      �� 5:      $��Function Selector��6   $��	0,10,0,10q�66   .��5;      A�  A� Rows$ 4��6"    A� Columns$ 5��6+   .+�5<      �� $	��log��6!   $	��log��6Y   ��6/   �� *FunctionButtonCommand+6Y   �� 5=      $��ln��6!   $��ln��6X   ��6.   �� *FunctionButtonCommand+6X   �� 5>      $	��sin��6!   $	��sin��6Y   ��6/   �� *FunctionButtonCommand+6Y   �� 5?      $	��cos��6!   $	��cos��6Y   ��6/   �� *FunctionButtonCommand+6Y   �� 5@      $��tg��6!   $��tg��6X   ��6.   �� *FunctionButtonCommand+6X   �� 5B      $	��ctg��6!   $	��ctg��6Y   ��6/   �� *FunctionButtonCommand+6Y   �� 5C      $��arcsin��6!   $��arcsin��6\   ��62   �� *FunctionButtonCommand+6\   �� 5D      $��arccos��6!   $��arccos��6\   ��62   �� *FunctionButtonCommand+6\   �� 5E      $��arctg��6!   $��arctg��6[   ��61   �� *FunctionButtonCommand+6[   �� 5F      $��arcctg��6!   $��arcctg��6\   ��62   �� *FunctionButtonCommand+6\   �� 5H      $��sh��6!   $��sh��6X   ��6.   �� *FunctionButtonCommand+6X   �� 5I      $��ch��6!   $��ch��6X   ��6.   �� *FunctionButtonCommand+6X   �� 5J      $��th��6!   $��th��6X   ��6.   �� *FunctionButtonCommand+6X   �� 5K      $	��cth��6!   $	��cth��6Y   ��6/   �� *FunctionButtonCommand+6Y   �� 5L      $
��arsh��6!   $
��arsh��6Z   ��60   �� *FunctionButtonCommand+6Z   �� 5N      $
��arch��6!   $
��arch��6Z   ��60   �� *FunctionButtonCommand+6Z   �� 5O      $
��arth��6!   $
��arth��6Z   ��60   �� *FunctionButtonCommand+6Z   �� 5P      $��arcth��6!   $��arcth��6[   ��61   �� *FunctionButtonCommand+6[   5Q      5R      �� 5U      
���  ��$��5,0,0,0q�5V      ��5U      �� *ErrorMessage+y�5W      �� *ShowErrorMessage+#
 [ �� 5Z      $��Simplify��6   $��5,10,0,0q�5[      y�5\      �� *SimplifyButtonVisible+#
 [ ��5]      �� *SimplifyCommand+�� 5_      $��5,0,0,0q�5`      $��20(�5b      ��5_      �� *DerivativeText+y�5a      �� *ShowDerivativeText+#
 [ 5b      5c      �� 5f      $��1��6   $
��Auto=�5g      $
��Auto=�5h      y�5i      �� *TreeToPresent+  NullToVisibilityConverter#
 [ .��5j      ,   &DerivativeVisualizerGUI.BinaryTreeView  
�������   TreeToPresent 6'   �� *TreeToPresent+ 6b   �� 5k      5l      � 5n   
   $��1��6    5o      �� 5p      $
��Auto �6#   �� 5q      $��* �6#   .+�5t      �� $��0��6   $ 
Horizontal=�6)   $��10q�6B   $
��Left=�6N   $	��Top=�6i   .+�5u      �� $��	Interval:��6   $��Center=�6-   $��14(�6H   �� 5v      $��50��6   $��5q�6D   $��14(�6O   ��6%   �� *IntervalStart+6O   �� 5w      $
�� to ��6   $��Center=�6(   $��14(�6C   �� 5x      $��50��6   $��5q�6B   $��14(�6M   ��6%   �� *IntervalEnd+6M   �� 5y      $��Plot Function��6   
����ح�$��5,0,0,0q�6p   ��61   �� *PlotFunctionCommand+6p   5z        OxyPlot.Wpf.PlotView  5|      $��1��6   $��10q�6G     OxyPlot.Wpf.PlotViewBase   Model 6+   �� *	PlotModel+6G   5}      5      5�      