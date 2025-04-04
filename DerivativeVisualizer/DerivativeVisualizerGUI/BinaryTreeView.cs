using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DerivativeVisualizerGUI
{
    public class BinaryTreeView : Canvas
    {
        private const double NodeSize = 60;
        private const double HorizontalSpacing = 120;
        private const double VerticalSpacing = 80;
        private const double MinCanvasWidth = 400;

        public ASTNode TreeToPresent
        {
            get { return (ASTNode)GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register("TreeToPresent", typeof(ASTNode), typeof(BinaryTreeView), new PropertyMetadata(null, OnRootChanged));

        private static void OnRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BinaryTreeView view)
            {
                view.InvalidateVisual();
                view.DrawTree();
            }
        }

        private Dictionary<ASTNode, double> nodePositionsX = new Dictionary<ASTNode, double>();
        private double currentX = 0;

        private void DrawTree()
        {
            if (TreeToPresent == null) return;

            Children.Clear();
            nodePositionsX.Clear();
            currentX = 0;

            // First pass: layout calculation
            CalculateLayout(TreeToPresent);

            // Calculate canvas size
            double maxX = nodePositionsX.Values.Max();
            double minX = nodePositionsX.Values.Min();
            double treeWidth = (maxX - minX + 1) * HorizontalSpacing;
            double treeHeight = GetHeight(TreeToPresent) * (NodeSize + VerticalSpacing) + 50;

            Width = Math.Max(treeWidth, MinCanvasWidth) + 100;
            Height = treeHeight;

            DrawNode(TreeToPresent, Width / 2, 20, minX, maxX);
        }

        private double CalculateLayout(ASTNode node)
        {
            if (node == null) return 0;

            double left = CalculateLayout(node.Left);
            double right = CalculateLayout(node.Right);

            double pos;
            if (node.Left == null && node.Right == null)
            {
                pos = currentX++;
            }
            else if (node.Left != null && node.Right != null)
            {
                double leftPos = nodePositionsX[node.Left];
                double rightPos = nodePositionsX[node.Right];

                // Ensure minimum spacing between left and right child
                if (rightPos - leftPos < 1.0)
                {
                    double mid = (leftPos + rightPos) / 2;
                    nodePositionsX[node.Left] = mid - 0.5;
                    nodePositionsX[node.Right] = mid + 0.5;
                    leftPos = nodePositionsX[node.Left];
                    rightPos = nodePositionsX[node.Right];
                }

                pos = (leftPos + rightPos) / 2;
            }
            else if (node.Left != null)
            {
                pos = nodePositionsX[node.Left] + 0.5;
            }
            else // only right
            {
                pos = nodePositionsX[node.Right!] - 0.5;
            }

            nodePositionsX[node] = pos;
            return pos;
        }

        private void DrawNode(ASTNode node, double centerX, double y, double minX, double maxX)
        {
            if (node == null) return;

            double normalizedX;
            if (Math.Abs(maxX - minX) < 0.0001)
            {
                // Only one node, center it
                normalizedX = Width / 2;
            }
            else
            {
                normalizedX = ((nodePositionsX[node] - minX) / (maxX - minX)) * (Width - 100) + 50;
            }

            Button nodeButton = new Button
            {
                Width = NodeSize,
                Height = NodeSize,
                Background = node.NeedsDifferentiation ? Brushes.LightSkyBlue : Brushes.LightGray,
                Content = node.Value.ToString(),
                Foreground = Brushes.Black,
                IsEnabled = node.NeedsDifferentiation,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            if (node.NeedsDifferentiation)
            {
                ToolTip tooltip = new ToolTip { Content = node.DiffRule };
                ToolTipService.SetShowOnDisabled(nodeButton, true);
                ToolTipService.SetInitialShowDelay(nodeButton, 0);
                nodeButton.ToolTip = tooltip;
                nodeButton.Foreground = Brushes.Navy;
            }

            if (Application.Current.Resources.Contains("CircleButton"))
            {
                nodeButton.Style = (Style)Application.Current.Resources["CircleButton"];
            }

            var binding = new Binding($"DataContext.NodeClickCommands[{node.Locator}]")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1),
                Mode = BindingMode.OneWay
            };
            nodeButton.SetBinding(Button.CommandProperty, binding);

            Children.Add(nodeButton);
            SetLeft(nodeButton, normalizedX - NodeSize / 2);
            SetTop(nodeButton, y);

            if (node.Left != null)
            {
                double childX = ((nodePositionsX[node.Left] - minX) / (maxX - minX)) * (Width - 100) + 50;
                DrawLine(normalizedX, y + NodeSize, childX, y + NodeSize + VerticalSpacing);
                DrawNode(node.Left, centerX, y + NodeSize + VerticalSpacing, minX, maxX);
            }

            if (node.Right != null)
            {
                double childX = ((nodePositionsX[node.Right] - minX) / (maxX - minX)) * (Width - 100) + 50;
                DrawLine(normalizedX, y + NodeSize, childX, y + NodeSize + VerticalSpacing);
                DrawNode(node.Right, centerX, y + NodeSize + VerticalSpacing, minX, maxX);
            }
        }

        private int GetHeight(ASTNode node)
        {
            if (node == null) return 0;
            return 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Children.Add(line);
        }
    }
}
