   M S B A M L   `   `   `  ����R  NDerivativeVisualizerGUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:DerivativeVisualizerGUIDerivativeVisualizerGUI  5      !    DerivativeVisualizerGUI.App   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      7local%clr-namespace:DerivativeVisualizerGUI     5        �� 	Resources  5      5   /DerivativeVisualizerGUI.BooleanToBrushConverter   BooleanToBrushConverter5   /DerivativeVisualizerGUI.BooleanToCheckConverter  BooleanToCheckConverter7   1DerivativeVisualizerGUI.NullToVisibilityConverter  NullToVisibilityConverter:   4DerivativeVisualizerGUI.BooleanToVisibilityConverter   BooleanToVisibilityConverter  MyFocusVisual �� Property �� Value �� 
TargetType �� StrokeDashArray � ActualWidth � ActualHeight �� CornerRadius  CircleButton �� 
TargetType	 � Offset
 � Color A�Name ��Name �� Triggers P� Property P� Value �� 
TargetName �� 
StartPoint �� EndPoint T� Y T� X��%  &	        &	      &	 &     &	 9     &	 L     &	 �    7    5   
   6(     5   
   6(     5   
   6*     5	   
   6-   �� 5
   
   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � $��8q�6   .+�5      A� $��r1��5      
���   �$	 2 2R�5      $��1��5      �� 5      $��border��5      #��z 5      #��z 5      $��1q�5      $ 2��5      5      5      5      5      �� 5   
   " ��6%   .�5       ��  �@�OverridesDefaultStyle$
 True��66   �� 5!       � ��$ 2q�6'   �� 5"       �@�FocusVisualStyle8   61   �� 5#       � �� �	#FF6DB4EF6+   �� 5$       � �� 5%      �� 5&      " ��6&   .R�5'      � .+�5(      A� #��z��6&   A� 5)      ��5*   "   
� 5+   &   .��5,   *   � $	 0Q�67   $
 	#00000000��6B   � 5-   *   $
	 0.88Q�67   $
 	#00000000��6E   � 5.   *   $	 1Q�67   $
 	#80000000��6B   5/   '   51      A� 52      $ highlightCircle��62   $��10q�6&   ��53   "   �� 54   &   .��55   *   � $	 0Q�67   $
 	#50FFFFFF��6B   � 56   *   $		 0.5Q�67   $
 	#00FFFFFF��6D   � 57   *   $	 1Q�67   $
 	#50FFFFFF��6B   58   '   5:      �� 5;      $ content��6/   $��Center=�6@   $��Center=�6]   5<       5=      P� 5>       � ��$
 True��6;   . �5?   "   �� $ highlightCircle��6)    � �� 5@   &   �� 5A   *   $ 0.3,0&�6>   $ 0.7,1&�6Q   .��5B   .   � $	 0Q�6;   $
 	#50FFFFFF��6F   � 5C   .   $		 0.5Q�6;   $
 	#00FFFFFF��6H   � 5D   .   $	 1Q�6;   $
 	#50FFFFFF��6F   5E   +   5G   #   �� 5H   "   $ content��6)    � z� 5I   &   T� 5J   *   $	 0.5Q�6=   $	 0.5Q�6E   5L   #   5M      5O      5Q      5R      5S      5T      