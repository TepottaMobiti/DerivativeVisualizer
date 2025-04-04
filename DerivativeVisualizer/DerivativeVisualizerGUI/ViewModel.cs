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
    /* Priority list
        
        ÚJ: 0^0-ra is csinálj egy hasonlót, mint a 0-val való osztásra, az nagyon jó.
        
        0. Draggable Point: Amikor a pontbeli deriválás lehetősége megnyílik, inicializálódjon egy pont az intervallum közepén, ami draggelhető. Legyen egy input mező, ami egy x koordinátát vár, és oda ugrik a draggable
        point, ha helyes a megadott érték. Ne legyen gomb, nem kell. Esetleg ki/be kapcsoló gombja lehet a gombnak, hogy feleslegesen ne jelenjen meg az érintő. Ja és ahova húzzuk a pontot, oda jelenjen meg az érintő, 
        nyilván. Ezt foglald össze egy szép és érthető, tiszta promptba és pakkpakk. DoneZO
        
        Logika: deriválás egyszerűsítve van, függvény ábrázolva van -> megjelenik a pontbeli deriválás -> beír egy értéket, rákattint a gombra -> Tűnjön el a gomb, az értékmegadás mező
        maradjon, meg a f'(a) = b szöveg is maradjon, és lehessen húzogatni a gombot, meg ha megadunk egy új értéket akkor ugorjon oda a pont.
        
        1. Hiba: Pl. köbgyöknél nem jó a negatív rész, mert a Math.Pow rosszul kezeli. Valami olyasmit kéne csinálni, hogy áttérni ott sqrt-re, ha detektálni tudjuk. DoneZO
        2. TODO: Átírni magyarra a parser / tokenizer üzeneteit és ne hiba szerűek legyenek, hanem segítő hangnem. Legyenek újfajta üzenetek is, ahol lehet, küldjünk vissza valamit. Pl. ha felismerjük, hogy fv,
                 mondjuk "sin", de nincs utána nyitó zárójel. DoneZO
        3. TODO: Megnézni a modell hol dobhat exceptiönt és mindenhol lekezelni, MessageBox-okkal.
        4. TODO: double-ök kerekítése 2 tizedesjegyre.
        5. TODO: Valahogy beállítani, hogy a function inputban csak 2 tizedesjegy lehessen. Ezt a Tokenizerben lehet elkapni a legegyszerűbben szerintem. DoneZO
        6. TODO: A falevelek néha összeérnek. Lehetne még javítani rajta valamit?
        7. Hiba: elfogadunk ilyesmit, hogy x/(1-1) és lederiválja, 1 lesz a derivált. Mit lehet ezzel csinálni? Az egyszerűsítés sem működik jól itt. Ezt nézd végig, hogy mi történik pontosan. DoneZO
        8. Csak akkor jelenjen meg a fa, meg minden ami a fához kapcs (f'(x) felirat), ha elfogadtuk az inputot. Tehát ha az inputText változik, akkor a megjelenítő cuccaikat false-ra kell állítani.

     */

    // UTOLSÓ LÉPÉSEK
    // TODO: Átnevezni mindent konzisztensen, pl. show / visible a megjelenítendő dolgok, illetve egyfajta közölést használni, pl. hány tabot nyomsz, hol hagysz ki sorokat, stb.
    // TODO: Átfogó CHECK: Minden exceptiont dobó függvényt try-catchben hívunk a modellből? És mindegyik kiír valami messageboxot?
    // TODO: Majd a végén kérd meg a chadet hogy egyszerűsítsen a kódon, pl. a két plot function hasonló.
    // Generáltass majd a chatGPT-vel különböző buta user storykat, és nézd meg, hogy valamelyikre rosszul reagál-e az app. Megkérdezheted tőle azt is, hogy lát-e valami problémát. Minden class-t
    // ellenőriztess le vele.
    // Kérd meg a ChatGPT-t, hogy minden sort kommenteljen, hogy mi történik ott. Ahol kell a magyarázat, ott hagyd meg. Ezt is minden classra játszd el. Pl. hogy melyik field mit csinál, nem trivi.
    // TODO: Minden elnevezést egységesíts: UI elemek nevei tükrözzék, hogy milyen UI elemek pl Button név vége Button legyen.
    // TODO: Átnevezések, hogy mindennek találó neve legyen.
    // TODO: Minden fieldet átvizsgálni és dokumentálni, hogy mikor következik be változás rajta.
    // Átnézni, hogy nincs-e felesleges ellenőrizni való, ami azért felesleges, mert az alkalmazás biztosítja, hogy nem kell ellenőrizni, pl. láthatóság miatt.
    // Vagy inkább legyenek mindenhol felesleges ellenőrzések?

    // Minden függvényt ellenőrizz, hogy dobhat-e exceptiont.

    // Fractions-t, OxyPlot-ot mindenképpen dokumentáld majd le.

    // Parser megértése: többféle függvényre nézd meg, hogy hogy működik.

    public class ViewModel : ViewModelBase
    {
        #region Private Fields

        private ScatterSeries? draggablePoint;
        private LineSeries? tangentLine;
        private bool isDragging = false;

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
        private bool simplifyButtonVisible;
        private bool showDerivativeText;
        private bool showPlotDerivativeButton;
        private bool functionPlotted;
        private bool derivativePlotted;
        private bool showValueOfDerivativeAtAPointText;

        private DerivativeVisualizerModel.Model model;
        private ASTNode? treeToPresent;
        private PlotModel? plotModel;

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
                SimplifyButtonVisible = false;
                PlotModel = null;
                ShowPlotDerivativeButton = false;
                functionPlotted = false;
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                derivativePlotted = false;
                OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
                ShowValueOfDerivativeAtAPointText = false;
                DerivativeAtAPointText = string.Empty;
                draggablePoint = null;
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
                draggablePoint = null;
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
                draggablePoint = null;
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

        public bool SimplifyButtonVisible
        {
            get => simplifyButtonVisible;
            set
            {
                simplifyButtonVisible = value;
                OnPropertyChanged(nameof(SimplifyButtonVisible));
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
                InitializeCommands(TreeToPresent);
            }
        }

        public PlotModel? PlotModel
        {
            get => plotModel;
            set { plotModel = value; OnPropertyChanged(nameof(PlotModel)); }
        }

        private ASTNode? InputFunction { get; set; }

        public ASTNode? SimplifiedTree { get; private set; }

        public Dictionary<int, DelegateCommand> NodeClickCommands { get; } = new();

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

        private void Model_OnInputProcessed(ASTNode? tree, string msg)
        {
            InputValid = inputText.Length == 0 ? null : tree is not null;

            if (InputValid == true)
            {
                ShowErrorMessage = false;
            }

            if (InputValid == false)
            {
                ErrorMessage = msg;
            }
        }

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

        private void Model_OnDifferentiationFinished(ASTNode? tree)
        {
            SimplifyButtonVisible = true;
            SimplifiedTree = tree;
        }

        #endregion

        #region Delegate Functions

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

        private void SimplifyTree(object? parameter)
        {
            TreeToPresent = SimplifiedTree;
            SimplifyButtonVisible = false;
            DerivativeText = "f'(x) = " + SimplifiedTree?.ToString() ?? "";
            ShowDerivativeText = true;
            ShowPlotDerivativeButton = true;
            OnPropertyChanged(nameof(ShowDerivativeAtAPoint));
        }

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
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }
        }

        private void PlotTangent(object? parameter)
        {
            bool pointParsed = double.TryParse(DerivativeAtAPointText, NumberStyles.Float, CultureInfo.InvariantCulture, out double point);

            if (!pointParsed)
            {
                ErrorOccurred?.Invoke("A pont megadásához írjon be egy számot.");
                return;
            }

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

            double r = (endInterval - startInterval) / 8; // Az érintő hosszának a fele

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

                if (draggablePoint == null)
                {
                    draggablePoint = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 5 };
                    PlotModel!.Series.Add(draggablePoint);

                    // Hook up dragging support
                    PlotModel.MouseDown += OnMouseDown;
                    PlotModel.MouseMove += OnMouseMove;
                    PlotModel.MouseUp += OnMouseUp;
                }

                // Create tangent line if needed
                if (tangentLine == null)
                {
                    tangentLine = new LineSeries
                    {
                        Color = OxyColors.Orange,
                        StrokeThickness = 3
                    };
                    PlotModel!.Series.Add(tangentLine);
                }

                // Move point and update tangent
                UpdateDraggableAndTangent(point, functionValueAtPoint, derivativeValueAtPoint);

                PlotModel!.InvalidatePlot(true);

                ValueOfFunctionAtAPointText = $"f({point}) = {Math.Round(functionValueAtPoint, 2)}";

                ValueOfDerivativeAtAPointText = $"f'({point}) = {derivativeValueAtPoint}";

                EquationOfTangentText = tangentLine.Title;

                ShowValueOfDerivativeAtAPointText = true;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }

        }

        private void UpdateDraggableAndTangent(double x, double y, double slope)
        {
            var (startInterval, endInterval) = CheckInterval();
            double r = (endInterval - startInterval) / 8;

            // Move draggable point
            draggablePoint!.Points.Clear();
            draggablePoint.Points.Add(new ScatterPoint(x, y));

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
            tangentLine.Title = $"y = {slope}x + {intercept}";
        }

        private void OnMouseDown(object? sender, OxyMouseDownEventArgs e)
        {
            var pos = e.Position;
            var x = PlotModel!.DefaultXAxis.InverseTransform(pos.X);
            var y = PlotModel.DefaultYAxis.InverseTransform(pos.Y);

            var point = draggablePoint?.Points.FirstOrDefault();
            if (point == null) return;

            // Check if user clicked close to the point
            if (Math.Abs(x - point.X) < 0.5 && Math.Abs(y - point.Y) < 0.5)
            {
                isDragging = true;
                e.Handled = true;
            }
        }

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

            UpdateDraggableAndTangent(x, y, slope);

            DerivativeAtAPointText = x.ToString();

            ValueOfFunctionAtAPointText = $"f({x}) = {y}";
            ValueOfDerivativeAtAPointText = $"f'({x}) = {slope}";
            EquationOfTangentText = tangentLine?.Title ?? "";
            ShowValueOfDerivativeAtAPointText = true;

            PlotModel.InvalidatePlot(false);
        }

        private void OnMouseUp(object? sender, OxyMouseEventArgs e)
        {
            isDragging = false;
        }


        #endregion

        #region Helper Functions

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

        private (double, double) CheckInterval()
        {
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
                    IsZoomEnabled = true
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
                    IsZoomEnabled = true
                });
            }

            return PlotModel;
        }

        private void InitializeCommands(ASTNode? node)
        {
            if (node == null) return;

            NodeClickCommands[node.Locator] = new DelegateCommand(param => OnNodeClick(node));

            InitializeCommands(node.Left);
            InitializeCommands(node.Right);
        }

        private (double[] xValues, double step) GenerateXValuesAndStep(double start, double end)
        {
            int numPoints = 40 * (int)(end - start) + 1; // Ne ilyen legyen
            double step = (end - start) / (numPoints - 1);
            double[] xValues = Enumerable.Range(0, numPoints).Select(i => start + step * i).ToArray();
            return (xValues, step);
        }

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