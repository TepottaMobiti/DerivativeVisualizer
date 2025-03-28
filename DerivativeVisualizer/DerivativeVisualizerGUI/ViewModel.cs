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
    
        1. Hiba: Pl. köbgyöknél nem jó a negatív rész, mert a Math.Pow rosszul kezeli. Valami olyasmit kéne csinálni, hogy áttérni ott sqrt-re, ha detektálni tudjuk. DoneZO
        2. TODO: Átírni magyarra a parser / tokenizer üzeneteit és ne hiba szerűek legyenek, hanem segítő hangnem. Legyenek újfajta üzenetek is, ahol lehet, küldjünk vissza valamit. Pl. ha felismerjük, hogy fv,
                 mondjuk "sin", de nincs utána nyitó zárójel. DoneZO
        3. TODO: Megnézni a modell hol dobhat exceptiönt és mindenhol lekezelni, MessageBox-okkal.
        4. TODO: double-ök kerekítése 2 tizedesjegyre.
        5. TODO: Valahogy beállítani, hogy a function inputban csak 2 tizedesjegy lehessen. Ezt a Tokenizerben lehet elkapni a legegyszerűbben szerintem. DoneZO
        6. TODO: A falevelek néha összeérnek. Lehetne még javítani rajta valamit?
        7. Hiba: elfogadunk ilyesmit, hogy x/(1-1) és lederiválja, 1 lesz a derivált. Mit lehet ezzel csinálni? Az egyszerűsítés sem működik jól itt. Ezt nézd végig, hogy mi történik pontosan.

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

        private string inputText;
        private string errorMessage;
        private string startInterval;
        private string endInterval;
        private string derivativeText;
        private string derivativeAtAPointText;
        private string valueOfDerivativeAtAPointText;

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
                derivativeAtAPointText = string.Empty;
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
                double functionValueAtPoint = FunctionEvaluator.Evaluate(InputFunction!, point, step);
                if (!double.IsFinite(functionValueAtPoint))
                {
                    ErrorOccurred?.Invoke("A függvény nincs értelmezve a megadott pontban.");
                    return;
                }

                double derivativeValueAtPoint = FunctionEvaluator.Evaluate(SimplifiedTree!, point, step);
                if (!double.IsFinite(derivativeValueAtPoint))
                {
                    ErrorOccurred?.Invoke("A függvény nem deriválható a megadott pontban.");
                    return;
                }

                // Érintő létrehozása. y = f(a) + f'(a) * (x - a)
                functionValueAtPoint = Math.Round(functionValueAtPoint, 2);
                derivativeValueAtPoint = Math.Round(derivativeValueAtPoint, 2);

                ASTNode tangent = new ASTNode("+",
                                      new ASTNode(functionValueAtPoint.ToString()),
                                      new ASTNode("*",
                                          new ASTNode(derivativeValueAtPoint.ToString()),
                                          new ASTNode("-",
                                              new ASTNode("x"),
                                              new ASTNode(point.ToString()))));

                PlotSeries(tangent, xValues, step, OxyColors.Orange, tangent.ToString());

                ValueOfDerivativeAtAPointText = $"f'({point}) = {derivativeValueAtPoint}";
                ShowValueOfDerivativeAtAPointText = true;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e.Message);
            }

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
            int numPoints = 40 * (int)(end - start) + 1;
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
