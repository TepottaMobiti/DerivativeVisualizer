using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace DerivativeVisualizerGUI.Behaviors
{
    // Entire class is AI generated.
    public static class HorizontalScrollOnShiftWheel
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(HorizontalScrollOnShiftWheel),
                new UIPropertyMetadata(false, OnEnableChanged));

        /// <summary>
        /// Gets the value of the attached <c>Enable</c> property, which indicates whether shift-wheel horizontal scrolling is enabled.
        /// </summary>
        /// <param name="obj">The dependency object from which to read the property.</param>
        /// <returns><c>true</c> if horizontal scrolling on Shift+MouseWheel is enabled; otherwise, <c>false</c>.</returns>
        public static bool GetEnable(DependencyObject obj) => (bool)obj.GetValue(EnableProperty);

        /// <summary>
        /// Sets the value of the attached <c>Enable</c> property to enable or disable shift-wheel horizontal scrolling on a ScrollViewer.
        /// </summary>
        /// <param name="obj">The dependency object on which to set the property.</param>
        /// <param name="value"><c>true</c> to enable; <c>false</c> to disable horizontal scrolling with Shift+MouseWheel.</param>
        public static void SetEnable(DependencyObject obj, bool value) => obj.SetValue(EnableProperty, value);

        /// <summary>
        /// Handles changes to the <c>Enable</c> attached property.
        /// Attaches or detaches the PreviewMouseWheel event handler based on the new value.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                if ((bool)e.NewValue)
                    scrollViewer.PreviewMouseWheel += ScrollViewerPreviewMouseWheel;
                else
                    scrollViewer.PreviewMouseWheel -= ScrollViewerPreviewMouseWheel;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event and scrolls horizontally if the Shift key is held down.
        /// Prevents default vertical scrolling behavior when triggered.
        /// </summary>
        /// <param name="sender">The ScrollViewer that received the event.</param>
        /// <param name="e">The mouse wheel event data.</param>
        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
    }
}
