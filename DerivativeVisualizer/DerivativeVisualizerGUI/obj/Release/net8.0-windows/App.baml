   M S B A M L   `   `   `  ���� R  NDerivativeVisualizerGUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:DerivativeVisualizerGUIDerivativeVisualizerGUI  !    DerivativeVisualizerGUI.App   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7local%clr-namespace:DerivativeVisualizerGUI       �� 	Resources  5   /DerivativeVisualizerGUI.BooleanToBrushConverter   BooleanToBrushConverter5   /DerivativeVisualizerGUI.BooleanToCheckConverter  BooleanToCheckConverter7   1DerivativeVisualizerGUI.NullToVisibilityConverter  NullToVisibilityConverter:   4DerivativeVisualizerGUI.BooleanToVisibilityConverter   BooleanToVisibilityConverter  MyFocusVisual �� Property �� Value �� 
TargetType �� StrokeDashArray � ActualWidth � ActualHeight �� CornerRadius  CircleButton �� 
TargetType	 � Offset
 � Color A�Name ��Name �� Triggers P� Property P� Value �� 
TargetName �� 
StartPoint �� EndPoint T� Y T� X��%�  &	        &	      &	 
     &	      &	      &	 �     7          �� .���  � �� �� " ��.R�� $��8q�.+�A� $��r1��
���   �$	 2 2R�$��1���� $��border��#��z #��z $��1q�$ 2���� " ��.���  �@�OverridesDefaultStyle$
 True����  � ��$ 2q���  �@�FocusVisualStyle8   ��  � �� �	#FF6DB4EF��  � �� �� " ��.R�� .+�A� #��z��A� ��
� .��� $	 0Q�$
 	#00000000��� $
	 0.88Q�$
 	#00000000��� $	 1Q�$
 	#80000000��A� $ highlightCircle��$��10q����� .��� $	 0Q�$
 	#50FFFFFF��� $		 0.5Q�$
 	#00FFFFFF��� $	 1Q�$
 	#50FFFFFF���� $ content��$��Center=�$��Center=� P�  � ��$
 True��. ��� $ highlightCircle�� � �� �� $ 0.3,0&�$ 0.7,1&�.��� $	 0Q�$
 	#50FFFFFF��� $		 0.5Q�$
 	#00FFFFFF��� $	 1Q�$
 	#50FFFFFF���� $ content�� � z� T� $	 0.5Q�$	 0.5Q�