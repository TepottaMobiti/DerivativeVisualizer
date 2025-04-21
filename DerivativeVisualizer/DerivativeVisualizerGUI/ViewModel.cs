using DerivativeVisualizerModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DerivativeVisualizerGUI
{
    public class ViewModel : ViewModelBase
    {
        #region Private Fields

        private string inputText;
        private string errorMessage;
        private string startInterval;
        private string endInterval;
        private string derivativeText;
        private string derivativeAtAPointText;
        private string valueOfDerivativeAtAPointText;
        private string valueOfFunctionAtAPointText;
        private string equationOfTangentText;

        private bool? inputValid;
        private bool showErrorMessage;
        private bool showSimplifyButton;
        private bool showDerivativeText;
        private bool showPlotDerivativeButton;
        private bool showValueOfDerivativeAtAPointText;
        private bool functionPlotted;
        private bool derivativePlotted;
        private bool isDragging = false;

        private DerivativeVisualizerModel.Model model;
        private ASTNode? treeToPresent;
        private PlotModel? plotModel;
        private ScatterSeries? draggablePointOnFunction;
        private ScatterSeries? draggablePointOnDerivative;
        private LineSeries? tangentLine;

        private double point;

        #endregion

        #region Properties

        public string InputText
        {
            get => inputText;
            set
            {
                inputText = value;
                OnPropertyChanged(nameof(InputText));
                model.ProcessInput(InputText);
                if (inputText.Length == 0)
                {
                    TreeToPresent = null;
                }
                ShowSimplifyButton = false;
                PlotModel = null;
                ShowPlotDerivativeButton = false;
                functionPlotted = false;
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                derivativePlotted = false;
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                ShowValueOfDerivativeAtAPointText = false;
                DerivativeAtAPointText = string.Empty;
                draggablePointOnFunction = null;
                draggablePointOnDerivative = null;
                tangentLine = null;
                isDragging = false;
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string StartInterval
        {
            get => startInterval;
            set
            {
                startInterval = value;
                PlotModel = null;
                functionPlotted = false;
                derivativePlotted = false;
                OnPropertyChanged(nameof(StartInterval));
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                ShowValueOfDerivativeAtAPointText = false;
                DerivativeAtAPointText = string.Empty;
                draggablePointOnFunction = null;
                draggablePointOnDerivative = null;
                tangentLine = null;
                isDragging = false;
            }
        }

        public string EndInterval
        {
            get => endInterval;
            set
            {
                endInterval = value;
                PlotModel = null;
                functionPlotted = false;
                derivativePlotted = false;
                OnPropertyChanged(nameof(EndInterval));
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                ShowValueOfDerivativeAtAPointText = false;
                DerivativeAtAPointText = string.Empty;
                draggablePointOnFunction = null;
                draggablePointOnDerivative = null;
                tangentLine = null;
                isDragging = false;
            }
        }

        public string DerivativeText
        {
            get => derivativeText;
            set
            {
                derivativeText = value;
                OnPropertyChanged(nameof(DerivativeText));
            }
        }

        public string DerivativeAtAPointText
        {
            get => derivativeAtAPointText;
            set
            {
                derivativeAtAPointText = value;
                OnPropertyChanged(nameof(DerivativeAtAPointText));
            }
        }

        public string ValueOfDerivativeAtAPointText
        {
            get => valueOfDerivativeAtAPointText;
            set
            {
                valueOfDerivativeAtAPointText = value;
                OnPropertyChanged(nameof(ValueOfDerivativeAtAPointText));
            }
        }

        public string ValueOfFunctionAtAPointText
        {
            get => valueOfFunctionAtAPointText;
            set
            {
                valueOfFunctionAtAPointText = value;
                OnPropertyChanged(nameof(ValueOfFunctionAtAPointText));
            }
        }

        public string EquationOfTangentText
        {
            get => equationOfTangentText;
            set
            {
                equationOfTangentText = value;
                OnPropertyChanged(nameof(EquationOfTangentText));
            }
        }

        public bool? InputValid
        {
            get => inputValid;
            set
            {
                inputValid = value;
                OnPropertyChanged(nameof(InputValid));
            }
        }

        public bool ShowErrorMessage
        {
            get => showErrorMessage;
            set
            {
                showErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool ShowSimplifyButton
        {
            get => showSimplifyButton;
            set
            {
                showSimplifyButton = value;
                OnPropertyChanged(nameof(ShowSimplifyButton));
            }
        }

        public bool ShowDerivativeText
        {
            get => showDerivativeText;
            set
            {
                showDerivativeText = value;
                OnPropertyChanged(nameof(ShowDerivativeText));
            }
        }

        public bool ShowPlotDerivativeButton
        {
            get => showPlotDerivativeButton;
            set
            {
                showPlotDerivativeButton = value;
                OnPropertyChanged(nameof(ShowPlotDerivativeButton));
            }
        }

        public bool ShowValueOfDerivativeAtAPointText
        {
            get => showValueOfDerivativeAtAPointText;
            set
            {
                showValueOfDerivativeAtAPointText = value;
                OnPropertyChanged(nameof(ShowValueOfDerivativeAtAPointText));
            }
        }

        public bool ShowDerivativeAtAPoint
        {
            get => functionPlotted && ShowPlotDerivativeButton;
        }

        public ASTNode? TreeToPresent
        {
            get => treeToPresent;
            set
            {
                treeToPresent = value;
                OnPropertyChanged(nameof(TreeToPresent));
                InitializeNodeCommands(TreeToPresent);
            }
        }

        public PlotModel? PlotModel
        {
            get => plotModel;
            set { plotModel = value; OnPropertyChanged(nameof(PlotModel)); }
        }

        private ASTNode? InputFunction { get; set; }

        private ASTNode? SimplifiedTree { get; set; }

        public Dictionary<int, ICommand> NodeClickCommands { get; } = new();

        #endregion

        #region Commands

        public ICommand ToggleErrorMessageCommand { get; }

        public ICommand FunctionButtonCommand { get; }

        public ICommand SimplifyCommand { get; }

        public ICommand PlotFunctionCommand { get; }

        public ICommand PlotDerivativeCommand { get; }

        public ICommand PlotTangentCommand { get; }

        #endregion

        #region Constructor
        public ViewModel()
        {
            inputText = string.Empty;
            errorMessage = string.Empty;
            derivativeText = string.Empty;
            derivativeAtAPointText = string.Empty;
            valueOfDerivativeAtAPointText = string.Empty;
            valueOfFunctionAtAPointText = string.Empty;
            equationOfTangentText = string.Empty;
            startInterval = "-10";
            endInterval = "10";
            model = new DerivativeVisualizerModel.Model();
            model.InputProcessed += Model_OnInputProcessed;
            model.TreeReady += Model_OnTreeReady;
            model.TreeUpdated += Model_OnTreeUpdated;
            model.DifferentiationFinished += Model_OnDifferentiationFinished;
            treeToPresent = null;
            ToggleErrorMessageCommand = new DelegateCommand(_ => ShowErrorMessage = !ShowErrorMessage);
            FunctionButtonCommand = new DelegateCommand(AddFunctionText);
            SimplifyCommand = new DelegateCommand(SimplifyTree);
            PlotFunctionCommand = new DelegateCommand(PlotFunction);
            PlotDerivativeCommand = new DelegateCommand(PlotDerivative);
            PlotTangentCommand = new DelegateCommand(PlotTangent);
        }

        #endregion

        #region Events

        public event Action<string>? ErrorOccurred;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates InputValid and displays error messages based on whether the input expression was successfully parsed.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="msg"></param>
        private void Model_OnInputProcessed(string msg)
        {
            InputValid = inputText.Length == 0 ? null : msg.Length == 0;

            if (InputValid == true)
            {
                ShowErrorMessage = false;
            }

            if (InputValid == false)
            {
                ErrorMessage = msg;
            }
        }

        /// <summary>
        /// Stores the initial parsed function, updates the tree to present, and shows its derivative.
        /// </summary>
        /// <param name="tree"></param>
        private void Model_OnTreeReady(ASTNode? tree)
        {
            if (tree is null)
            {
                return;
            }
            InputFunction = tree;
            TreeToPresent = tree;
            DerivativeText = "f'(x) = " + TreeToPresent?.ToString() ?? "";
            ShowDerivativeText = true;
        }

        /// <summary>
        /// Updates the UI with the new derivative tree and makes the derivative text visible.
        /// </summary>
        /// <param name="tree"></param>
        private void Model_OnTreeUpdated(ASTNode? tree)
        {
            if (tree is null)
            {
                return;
            }
            TreeToPresent = tree;
            DerivativeText = "f'(x) = " + TreeToPresent?.ToString() ?? "";
            ShowDerivativeText = true;
        }

        /// <summary>
        /// Stores the final simplified derivative and makes the simplify button visible.
        /// </summary>
        /// <param name="tree"></param>
        private void Model_OnDifferentiationFinished(ASTNode? tree)
        {
            ShowSimplifyButton = true;
            SimplifiedTree = tree;
        }

        #endregion

        #region OnClick Handlers

        /// <summary>
        /// Requests a differentiation step for the clicked node in the syntax tree.
        /// </summary>
        /// <param name="node"></param>
        private void OnNodeClick(ASTNode node)
        {
            try
            {
                model.DifferentiateByLocator(node.Locator);
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Adds the selected function syntax (e.g. sin(), log(,)) to the input field, respecting length constraints.
        /// </summary>
        /// <param name="parameter"></param>
        private void AddFunctionText(object? parameter)
        {
            if (parameter is string buttonText)
            {
                if (buttonText == "log" && InputText.Length + buttonText.Length + 3 <= 20) // + 3: "(,)"
                {
                    InputText += buttonText + "(,)";
                }
                else if (InputText.Length + buttonText.Length + 2 <= 20) // + 2: "()"
                {
                    InputText += buttonText + "()";
                }
            }
        }

        /// <summary>
        /// Sets the displayed tree to the simplified version, updates the derivative text, and enables plotting the derivative.
        /// </summary>
        /// <param name="parameter"></param>
        private void SimplifyTree(object? parameter)
        {
            TreeToPresent = SimplifiedTree;
            ShowSimplifyButton = false;
            DerivativeText = "f'(x) = " + SimplifiedTree?.ToString() ?? "";
            ShowDerivativeText = true;
            ShowPlotDerivativeButton = true;
            OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
        }

        /// <summary>
        /// Plots the original function over the given interval.
        /// </summary>
        /// <param name="parameter"></param>
        private void PlotFunction(object? parameter)
        {
            if (functionPlotted)
            {
                return;
            }
            var (startInterval, endInterval) = CheckInterval();
            if (double.IsNaN(startInterval) || double.IsNaN(endInterval)) return;

            var (xValues, step) = GenerateXValuesAndStep(startInterval, endInterval);

            try
            {
                PlotSeries(InputFunction!, xValues, step, OxyColors.Blue, InputText);
                functionPlotted = true;
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Plots the simplified derivative function and, if possible, adds a draggable point showing its slope.
        /// </summary>
        /// <param name="parameter"></param>
        private void PlotDerivative(object? parameter)
        {
            if (derivativePlotted)
            {
                return;
            }
            var (startInterval, endInterval) = CheckInterval();
            if (double.IsNaN(startInterval) || double.IsNaN(endInterval)) return;

            var (xValues, step) = GenerateXValuesAndStep(startInterval, endInterval);

            try
            {
                PlotSeries(SimplifiedTree!, xValues, step, OxyColors.Green, SimplifiedTree!.ToString());
                derivativePlotted = true;
                if (functionPlotted && draggablePointOnFunction is not null)
                {
                    draggablePointOnDerivative?.Points.Clear();
                    double slope = FunctionEvaluator.Evaluate(SimplifiedTree, point, 0.01);
                    draggablePointOnDerivative?.Points.Add(new ScatterPoint(point, slope));
                    PlotModel!.InvalidatePlot(true);

                }
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Calculates and draws the tangent line to the original function at a user-specified point, with validation and slope display.
        /// </summary>
        /// <param name="parameter"></param>
        private void PlotTangent(object? parameter)
        {
            bool pointParsed = double.TryParse(DerivativeAtAPointText, NumberStyles.Float, CultureInfo.InvariantCulture, out double point);

            if (!pointParsed)
            {
                ErrorOccurred?.Invoke("A pont megadásához írjon be egy számot.");
                return;
            }

            this.point = point;

            if (DerivativeAtAPointText.Contains("."))
            {
                var parts = DerivativeAtAPointText.Split('.');
                if (parts.Length == 2 && parts[1].Length > 2)
                {
                    ErrorOccurred?.Invoke("Legfeljebb 2 tizedesjegy megadása engedélyezett.");
                    return;
                }
            }

            var (startInterval, endInterval) = CheckInterval();
            if (double.IsNaN(startInterval) || double.IsNaN(endInterval)) return;

            if (point < startInterval || point > endInterval)
            {
                ErrorOccurred?.Invoke("A pontnak a megadott intervallumon belül kell lennie.");
                return;
            }

            double r = (endInterval - startInterval) / 8;

            double intervalStart = point - r;
            double intervalEnd = point + r;

            var (xValues, step) = GenerateXValuesAndStep(intervalStart, intervalEnd);

            try
            {
                double functionValueAtPoint = FunctionEvaluator.Evaluate(InputFunction!, point, 1e-10);
                if (!double.IsFinite(functionValueAtPoint))
                {
                    ErrorOccurred?.Invoke("A függvény nincs értelmezve a megadott pontban.");
                    return;
                }

                double derivativeValueAtPoint = FunctionEvaluator.Evaluate(SimplifiedTree!, point, 1e-10);
                if (!double.IsFinite(derivativeValueAtPoint))
                {
                    ErrorOccurred?.Invoke("A függvény nem deriválható a megadott pontban.");
                    return;
                }

                if (draggablePointOnFunction == null)
                {
                    draggablePointOnFunction = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 5 };
                    PlotModel!.Series.Add(draggablePointOnFunction);
                    draggablePointOnDerivative = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 5 };
                    PlotModel.Series.Add(draggablePointOnDerivative);

                    PlotModel.MouseDown += OnMouseDown;
                    PlotModel.MouseMove += OnMouseMove;
                    PlotModel.MouseUp += OnMouseUp;
                }

                if (tangentLine == null)
                {
                    tangentLine = new LineSeries
                    {
                        Color = OxyColors.Orange,
                        StrokeThickness = 3
                    };
                    PlotModel!.Series.Add(tangentLine);
                }

                UpdateDraggableAndTangent(point, functionValueAtPoint, derivativeValueAtPoint);

                PlotModel!.InvalidatePlot(true);

                ValueOfFunctionAtAPointText = $"f({point}) = {Math.Round(functionValueAtPoint, 2)}";

                ValueOfDerivativeAtAPointText = $"f'({point}) = {Math.Round(derivativeValueAtPoint,2)}";

                EquationOfTangentText = tangentLine.Title;

                ShowValueOfDerivativeAtAPointText = true;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Starts dragging if the mouse is clicked near the draggable point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object? sender, OxyMouseDownEventArgs e)
        {
            var pos = e.Position;
            var x = PlotModel!.DefaultXAxis.InverseTransform(pos.X);
            var y = PlotModel.DefaultYAxis.InverseTransform(pos.Y);

            var point = draggablePointOnFunction?.Points.FirstOrDefault();
            if (point == null) return;

            // Check if user clicked close to the point
            if (Math.Abs(x - point.X) < 0.5 && Math.Abs(y - point.Y) < 0.5)
            {
                isDragging = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Updates the draggable point and tangent line position as the mouse moves during dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object? sender, OxyMouseEventArgs e)
        {
            if (!isDragging) return;

            var pos = e.Position;
            double x = PlotModel!.DefaultXAxis.InverseTransform(pos.X);

            var (startInterval, endInterval) = CheckInterval();
            x = Math.Max(startInterval, Math.Min(endInterval, x)); // Clamp x

            double y = FunctionEvaluator.Evaluate(InputFunction!, x, 0.01);
            double slope = FunctionEvaluator.Evaluate(SimplifiedTree!, x, 0.01);

            x = Math.Round(x, 2);
            y = Math.Round(y, 2);
            slope = Math.Round(slope, 2);
            point = x;

            UpdateDraggableAndTangent(x, y, slope);

            DerivativeAtAPointText = x.ToString();

            ValueOfFunctionAtAPointText = $"f({x}) = {y}";
            ValueOfDerivativeAtAPointText = $"f'({x}) = {slope}";
            EquationOfTangentText = tangentLine?.Title ?? "";
            ShowValueOfDerivativeAtAPointText = true;

            PlotModel.InvalidatePlot(false);
        }

        /// <summary>
        /// Stops the dragging interaction when the mouse button is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object? sender, OxyMouseEventArgs e)
        {
            isDragging = false;
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Validates and parses the user-specified interval for plotting, returning (NaN, NaN) if invalid.
        /// </summary>
        /// <returns></returns>
        private (double, double) CheckInterval()
        {
            if (StartInterval.Contains("."))
            {
                var parts = StartInterval.Split('.');
                if (parts.Length == 2 && parts[1].Length > 2)
                {
                    ErrorOccurred?.Invoke("Legfeljebb 2 tizedesjegy megadása engedélyezett.");
                    return(double.NaN, double.NaN);
                }
            }
            if (EndInterval.Contains("."))
            {
                var parts = EndInterval.Split('.');
                if (parts.Length == 2 && parts[1].Length > 2)
                {
                    ErrorOccurred?.Invoke("Legfeljebb 2 tizedesjegy megadása engedélyezett.");
                    return (double.NaN, double.NaN);
                }
            }

            bool startParsed = double.TryParse(StartInterval, NumberStyles.Float, CultureInfo.InvariantCulture, out double startInterval);
            bool endParsed = double.TryParse(EndInterval, NumberStyles.Float, CultureInfo.InvariantCulture, out double endInterval);
            if (InputFunction is null)
            {
                ErrorOccurred?.Invoke("Adjon meg egy függvényt az ábrázoláshoz.");
                return (double.NaN, double.NaN);
            }
            if (!startParsed || !endParsed)
            {
                ErrorOccurred?.Invoke("Az intervallum megadásához írjon be 2 számot.");
                return (double.NaN, double.NaN);
            }
            if (startInterval >= endInterval)
            {
                ErrorOccurred?.Invoke("Az intervallum kezdete nagyobb vagy egyenlő mint a vége.");
                return (double.NaN, double.NaN);
            }
            if (Math.Abs(startInterval) > 50 || Math.Abs(endInterval) > 50)
            {
                ErrorOccurred?.Invoke("Az intervallum nem részintervalluma a [-50, 50] intervallumnak.");
                return (double.NaN, double.NaN);
            }

            return (startInterval, endInterval);
        }

        /// <summary>
        /// Initializes and returns the PlotModel with configured axes if it hasn’t been created yet.
        /// </summary>
        /// <returns></returns>
        private PlotModel EnsurePlotModel()
        {
            if (PlotModel == null)
            {
                PlotModel = new PlotModel { };

                PlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    PositionAtZeroCrossing = true,
                    AxislineStyle = LineStyle.Solid,
                    AxislineThickness = 1,
                    AxislineColor = OxyColors.Black,
                    MajorGridlineStyle = LineStyle.None,
                    MinorGridlineStyle = LineStyle.None,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
                });

                PlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionAtZeroCrossing = true,
                    AxislineStyle = LineStyle.Solid,
                    AxislineThickness = 1,
                    AxislineColor = OxyColors.Black,
                    MajorGridlineStyle = LineStyle.None,
                    MinorGridlineStyle = LineStyle.None,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
                });

                //PlotModel.PlotType = PlotType.Cartesian;
            }

            return PlotModel;
        }

        /// <summary>
        /// Recursively assigns click commands to syntax tree nodes for triggering differentiation.
        /// </summary>
        /// <param name="node"></param>
        private void InitializeNodeCommands(ASTNode? node)
        {
            if (node == null) return;

            NodeClickCommands[node.Locator] = new DelegateCommand(param => OnNodeClick(node));

            InitializeNodeCommands(node.Left);
            InitializeNodeCommands(node.Right);
        }

        /// <summary>
        /// Generates evenly spaced X values and step size for plotting within a given interval.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private (double[] xValues, double step) GenerateXValuesAndStep(double start, double end)
        {
            int numPoints = Math.Max(40 * (int)(end - start) + 1, 201);
            double step = (end - start) / (numPoints - 1);
            double[] xValues = Enumerable.Range(0, numPoints).Select(i => start + step * i).ToArray();
            return (xValues, step);
        }

        /// <summary>
        /// Updates the position of a draggable point on a function graph and calculates the corresponding tangent line at that point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="slope"></param>
        private void UpdateDraggableAndTangent(double x, double y, double slope)
        {
            var (startInterval, endInterval) = CheckInterval();
            double r = (endInterval - startInterval) / 8;

            // Move draggable point
            draggablePointOnFunction!.Points.Clear();
            draggablePointOnFunction.Points.Add(new ScatterPoint(x, y));

            // Calculate tangent line
            double x1 = x - r;
            double x2 = x + r;
            double y1 = slope * (x1 - x) + y;
            double y2 = slope * (x2 - x) + y;

            tangentLine!.Points.Clear();
            tangentLine.Points.Add(new DataPoint(x1, y1));
            tangentLine.Points.Add(new DataPoint(x2, y2));

            // Set the equation of the tangent line as the title
            double intercept = y - slope * x;
            intercept = Math.Round(intercept, 2);
            slope = Math.Round(slope, 2);

            if (derivativePlotted)
            {
                draggablePointOnDerivative?.Points.Clear();
                draggablePointOnDerivative?.Points.Add(new ScatterPoint(x,slope));
            }

            tangentLine.Title = $"y = {slope}x + {intercept}";
        }

        /// <summary>
        /// Plots a mathematical expression by evaluating it over the given X values and adds the series to the plot model.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="xValues"></param>
        /// <param name="step"></param>
        /// <param name="color"></param>
        /// <param name="title"></param>
        private void PlotSeries(ASTNode expression, double[] xValues, double step, OxyColor color, string title)
        {
            var model = EnsurePlotModel();
            LineSeries currentSeries = new LineSeries
            {
                Color = color,
                Title = title
            };

            foreach (var x in xValues)
            {
                double y = FunctionEvaluator.Evaluate(expression, x, step);
                if (!double.IsFinite(y))
                {
                    if (currentSeries.Points.Count > 0)
                    {
                        model.Series.Add(currentSeries);
                        currentSeries = new LineSeries { Color = color };
                    }
                }
                else
                {
                    currentSeries.Points.Add(new DataPoint(x, y));
                }
            }

            if (currentSeries.Points.Count > 0)
                model.Series.Add(currentSeries);

            PlotModel = model;
            PlotModel.InvalidatePlot(true);
        }
        #endregion
    }
}