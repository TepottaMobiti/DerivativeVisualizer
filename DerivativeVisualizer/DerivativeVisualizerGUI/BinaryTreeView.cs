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
        private const double nodeSize = 60;
        private const double horizontalSpacing = 120;
        private const double verticalSpacing = 80;
        private const double minCanvasWidth = 400;

        public ASTNode TreeToPresent
        {
            get { return (ASTNode)GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register("TreeToPresent", typeof(ASTNode), typeof(BinaryTreeView), new PropertyMetadata(null, OnRootChanged));

        /// <summary>
        /// Called when the <see cref="TreeToPresent"/> property changes.
        /// Invalidates the view and redraws the binary tree based on the new root node.
        /// </summary>
        /// <param name="d">The dependency object where the property changed.</param>
        /// <param name="e">Details about the property change event.</param>
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

        /// <summary>
        /// Clears and redraws the entire binary tree structure on the canvas based on the current <see cref="TreeToPresent"/>.
        /// Calculates layout, updates canvas dimensions, and draws nodes and edges.
        /// </summary>
        private void DrawTree()
        {
            if (TreeToPresent == null) return;

            Children.Clear();
            nodePositionsX.Clear();
            currentX = 0;

            CalculateLayout(TreeToPresent);

            double maxX = nodePositionsX.Values.Max();
            double minX = nodePositionsX.Values.Min();
            double treeWidth = (maxX - minX + 1) * horizontalSpacing;
            double treeHeight = GetHeight(TreeToPresent) * (nodeSize + verticalSpacing) + 50;

            Width = Math.Max(treeWidth, minCanvasWidth) + 100;
            Height = treeHeight;

            DrawNode(TreeToPresent, Width / 2, 20, minX, maxX);
        }

        /// <summary>
        /// Recursively calculates horizontal positions for each node to determine their placement in the tree.
        /// Positions are stored in <c>nodePositionsX</c> and prevent node overlap by adjusting children positions if needed.
        /// </summary>
        /// <param name="node">The current node being processed.</param>
        /// <returns>The computed X position for the current node.</returns>
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

                // Avoid overlap
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
                pos = left;
            }
            else
            {
                pos = right;
            }

            nodePositionsX[node] = pos;
            return pos;
        }

        /// <summary>
        /// Draws a single node and its connecting lines to children on the canvas.
        /// Binds click events, applies styles, and sets tooltips for differentiable nodes.
        /// </summary>
        /// <param name="node">The AST node to draw.</param>
        /// <param name="centerX">The horizontal center reference point for drawing.</param>
        /// <param name="y">The vertical coordinate of the node.</param>
        /// <param name="minX">Minimum X layout value (used for normalization).</param>
        /// <param name="maxX">Maximum X layout value (used for normalization).</param>
        private void DrawNode(ASTNode node, double centerX, double y, double minX, double maxX)
        {
            if (node == null) return;

            double normalizedX = GetNormalizedX(node, minX, maxX);

            Button nodeButton = new Button
            {
                Width = nodeSize,
                Height = nodeSize,
                Background = node.ToBeDifferentiated ? Brushes.LightSkyBlue : Brushes.LightGray,
                Content = node.Value.ToString(),
                Foreground = Brushes.Black,
                IsEnabled = node.ToBeDifferentiated,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            if (node.ToBeDifferentiated)
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
            SetLeft(nodeButton, normalizedX - nodeSize / 2);
            SetTop(nodeButton, y);

            if (node.Left != null)
            {
                double childX = GetNormalizedX(node.Left, minX, maxX);
                DrawLine(normalizedX, y + nodeSize, childX, y + nodeSize + verticalSpacing);
                DrawNode(node.Left, centerX, y + nodeSize + verticalSpacing, minX, maxX);
            }

            if (node.Right != null)
            {
                double childX = GetNormalizedX(node.Right, minX, maxX);
                DrawLine(normalizedX, y + nodeSize, childX, y + nodeSize + verticalSpacing);
                DrawNode(node.Right, centerX, y + nodeSize + verticalSpacing, minX, maxX);
            }
        }

        /// <summary>
        /// Converts a raw horizontal node position into a pixel-based canvas coordinate,
        /// normalized based on the total tree width.
        /// </summary>
        /// <param name="node">The node whose position to normalize.</param>
        /// <param name="minX">Minimum raw X value across the tree.</param>
        /// <param name="maxX">Maximum raw X value across the tree.</param>
        /// <returns>The normalized X coordinate for canvas placement.</returns>
        private double GetNormalizedX(ASTNode node, double minX, double maxX)
        {
            if (!nodePositionsX.TryGetValue(node, out double rawX)) return Width / 2;
            if (Math.Abs(maxX - minX) < 0.0001) return Width / 2;

            return ((rawX - minX) / (maxX - minX)) * (Width - 100) + 50;
        }

        /// <summary>
        /// Recursively calculates the height of the tree from the given node.
        /// Height is defined as the number of layers from root to the deepest leaf.
        /// </summary>
        /// <param name="node">The root node of the subtree.</param>
        /// <returns>The height of the subtree rooted at the given node.</returns>
        private int GetHeight(ASTNode node)
        {
            if (node == null) return 0;
            return 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        /// <summary>
        /// Draws a line between two points on the canvas to visually connect tree nodes.
        /// Skips drawing if any coordinate is invalid.
        /// </summary>
        /// <param name="x1">The starting X coordinate.</param>
        /// <param name="y1">The starting Y coordinate.</param>
        /// <param name="x2">The ending X coordinate.</param>
        /// <param name="y2">The ending Y coordinate.</param>
        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            if (double.IsNaN(x1) || double.IsNaN(y1) || double.IsNaN(x2) || double.IsNaN(y2))
                return;

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
