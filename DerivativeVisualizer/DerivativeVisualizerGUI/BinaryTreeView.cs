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
    // TODO: Beállítani, hogy a maximális input méret 20 vagy 25 legyen, és ezzel az inputtal megnézni, hogy mekkora a legnagyobb lehetséges fa, és arra optimalizálni ezt az osztályt.
    // Legnagyobb input 20-ra: x^-1+x+x+x+x+x+x+x+x
    // TODO: A modell hívásoknak az App.xaml-ben kéne lenniük, mint a macilaciban. De ezt majd elég a végén megcsinálnod, most a funkcionalitás legyen meg.
    public class BinaryTreeView : Canvas
    {
        private const double NodeSize = 60;
        private const double HorizontalSpacing = 100;
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

        private void DrawTree()
        {
            if (TreeToPresent == null) return;

            Children.Clear();

            double minX = 0, maxX = 0;
            double treeWidth, treeHeight;
            CalculateTreeSize(TreeToPresent, 0, 0, out treeWidth, out treeHeight, ref minX, ref maxX);

            Width = Math.Max(treeWidth, MinCanvasWidth) + 50;
            Height = treeHeight;

            double centerX = (Width - treeWidth) / 2;

            DrawNode(TreeToPresent, centerX + treeWidth / 2, 20, treeWidth / 4);
        }

        private void DrawNode(ASTNode node, double x, double y, double offset)
        {
            if (node == null) return;

            double NodeSize = 70;

            Button nodeButton = new Button
            {
                Width = NodeSize,
                Height = NodeSize,
                Background = node.NeedsDifferentiation ? Brushes.Blue : Brushes.LightGray,
                Content = node.Value.ToString(),
                Foreground = Brushes.Black,
                IsEnabled = node.NeedsDifferentiation,
            };

            if (node.NeedsDifferentiation)
            {
                ToolTip tooltip = new ToolTip { Content = node.DiffRule };
                ToolTipService.SetShowOnDisabled(nodeButton, true);
                ToolTipService.SetInitialShowDelay(nodeButton, 0);
                ToolTipService.SetBetweenShowDelay(nodeButton, 0);
                nodeButton.ToolTip = tooltip;
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
            SetLeft(nodeButton, x - NodeSize / 2);
            SetTop(nodeButton, y);

            if (node.Left != null)
            {
                double newOffset = Math.Max(offset / 2, HorizontalSpacing);
                DrawLine(x, y + NodeSize, x - newOffset, y + NodeSize + VerticalSpacing);
                DrawNode(node.Left, x - newOffset, y + NodeSize + VerticalSpacing, newOffset);
            }

            if (node.Right != null)
            {
                double newOffset = Math.Max(offset / 2, HorizontalSpacing);
                DrawLine(x, y + NodeSize, x + newOffset, y + NodeSize + VerticalSpacing);
                DrawNode(node.Right, x + newOffset, y + NodeSize + VerticalSpacing, newOffset);
            }
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

        private void CalculateTreeSize(ASTNode node, double x, double y, out double width, out double height, ref double minX, ref double maxX)
        {
            if (node == null)
            {
                width = 0;
                height = 0;
                return;
            }

            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);

            double leftWidth = 0, rightWidth = 0;
            double leftHeight = 0, rightHeight = 0;

            CalculateTreeSize(node.Left, x - HorizontalSpacing, y + VerticalSpacing, out leftWidth, out leftHeight, ref minX, ref maxX);
            CalculateTreeSize(node.Right, x + HorizontalSpacing, y + VerticalSpacing, out rightWidth, out rightHeight, ref minX, ref maxX);

            width = maxX - minX + NodeSize;
            height = Math.Max(leftHeight, rightHeight) + NodeSize + VerticalSpacing + 10;
        }
    }
}
