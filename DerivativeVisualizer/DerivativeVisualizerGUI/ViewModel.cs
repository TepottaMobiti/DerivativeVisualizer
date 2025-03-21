using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DerivativeVisualizerGUI
{
    // Hiba: Ha kinyitjuk a ?-et pl. "sin(x"-nél, és utána bezárjuk a zárójelet "sin(x)" akkor a kérdőjel eltűnik de az error message nem.
    // TODO: Átírni magyarra a parser / tokenizer üzeneteit és ne hiba szerűek legyenek, hanem segítő hangnem. Legyenek újfajta üzenetek is, ahol lehet, küldjünk vissza valamit. Pl. ha felismerjük, hogy fv,
    // mondjuk "sin", de nincs utána nyitó zárójel.
    // Hiba: Ha beírod, hogy pl. "3..", akkor a fa megjelenik, pedig nem kéne neki. Csak akkor kéne fának megjelennie, ha nincs error message. Csak akkor jelenjen meg a fa, ha pipánk van.
    // TODO: Az error üzeneteket tördelni, hogy kiférjenek. Favágó, de shit hap.
    // Hiba: Laggol az alkalmazás hosszú használat esetén, lehet explicit törölni kéne a memóriából a fákat amikor már új inputot írunk?

    // TODO: A Megjelenítendő dolgokat, pl. MessageBox az App.xaml-ben kell majd csinálni. Az MVVM architektúrának nem biztos, hogy ezek itt mind megfelelnek, majd nézd át, hogy minek hol a helye. Ld. MaciLaci.
    // TODO: Átnevezni mindent konzisztensen, pl. show / visible a megjelenítendő dolgok, illetve egyfajta közölést használni, pl. hány tabot nyomsz, hol hagysz ki sorokat, stb.
    // TODO: Labelek, textboxok törjenek meg ha nem férnek ki. Pl. x^sin(x)-nél az f'(x) sor nem fér ki.
    // TODO: Legyen az egész Viewban minden érték abszolút, tehát ne legyen auto a dolgok magassága pl. Nem baj, ha nem néz ki jól, csak full screenben nézzen ki jól.
    public class ViewModel : ViewModelBase
    {
        private string inputText;
        private string errorMessage;
        private bool showErrorMessage;
        private bool simplifyButtonVisible;
        private Model model;
        private ASTNode? treeToPresent;
        private string derivativeText;
        private bool showDerivativeText;

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

        public ASTNode? SimplifiedTree { get; private set; }

        public Dictionary<int, DelegateCommand> NodeClickCommands { get; } = new();

        public bool SimplifyButtonVisible
        {
            get => simplifyButtonVisible;
            set
            {
                simplifyButtonVisible = value;
                OnPropertyChanged(nameof(SimplifyButtonVisible));
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
        
        public bool ShowDerivativeText
        {
            get => showDerivativeText;
            set
            {
                showDerivativeText = value;
                OnPropertyChanged(nameof(ShowDerivativeText));
            }
        }

        private void InitializeCommands(ASTNode? node)
        {
            if (node == null) return;

            NodeClickCommands[node.Locator] = new DelegateCommand(param => OnNodeClick(node));

            InitializeCommands(node.Left);
            InitializeCommands(node.Right);
        }

        // Ez pl. biztos, hogy az App.xaml.cs-ben kell lennie.
        private void OnNodeClick(ASTNode node)
        {
            model.DifferentiateByLocator(node.Locator);
        }

        public ICommand FunctionButtonCommand { get; }
        public ICommand SimplifyCommand { get; }

        public ViewModel()
        {
            inputText = string.Empty;
            errorMessage = string.Empty;
            derivativeText = string.Empty;
            model = new Model();
            model.InputProcessed += Model_OnInputProcessed;
            model.TreeUpdated += Model_OnTreeUpdated;
            model.DifferentiationFinished += Model_OnDifferentiationFinished;
            treeToPresent = null;
            ToggleErrorMessageCommand = new DelegateCommand(_ => ShowErrorMessage = !ShowErrorMessage);
            FunctionButtonCommand = new DelegateCommand(AddFunctionText);
            SimplifyCommand = new DelegateCommand(SimplifyTree);
        }

        private void SimplifyTree(object? parameter)
        {
            TreeToPresent = SimplifiedTree;
            SimplifyButtonVisible = false;
            DerivativeText = "f'(x) = "+SimplifiedTree?.ToString() ?? "";
            ShowDerivativeText = true;
        }

        // Jelenjen meg egy egyszerűsítés gomb, amire ha rákattintunk, eltűnik a gomb, és a TreeToPresent egyszerűsítve lesz.
        private void Model_OnDifferentiationFinished(ASTNode? tree)
        {
            SimplifyButtonVisible = true;
            SimplifiedTree = tree;
        }

        // Ez is valszeg az App.xaml-ben kell, hogy legyen. A commandok megvalósítása.
        private void AddFunctionText(object? parameter)
        {
            if (parameter is string buttonText)
            {
                if (buttonText == "log")
                {
                    InputText += buttonText + "(,)";
                }
                else
                {
                    InputText += buttonText + "()";
                }
            }
        }

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
                ShowDerivativeText = false;
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

        public bool ShowErrorMessage
        {
            get => showErrorMessage;
            set
            {
                showErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ToggleErrorMessageCommand { get; }



        private bool? inputValid;
        public bool? InputValid
        {
            get => inputValid;
            set
            {
                inputValid = value;
                OnPropertyChanged(nameof(InputValid));
            }
        }

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
        }
    }
}
