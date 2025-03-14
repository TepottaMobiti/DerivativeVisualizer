using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DerivativeVisualizerGUI
{
    // Hiba: Ha kinyitjuk a ?-et pl. "sin(x"-nél, és utána bezárjuk a zárójelet "sin(x)" akkor a kérdőjel eltűnik de az error message nem.
    // TODO: Átírni magyarra a parser / tokenizer üzeneteit és ne hiba szerűek legyenek, hanem segítő hangnem. Legyenek újfajta üzenetek is, ahol lehet, küldjünk vissza valamit. Pl. ha felismerjük, hogy fv,
    // mondjuk "sin", de nincs utána nyitó zárójel.
    // Hiba: pl. elfogad ilyen inputot: "x5". Miért?
    public class ViewModel : ViewModelBase
    {
        private string inputText;
        private string errorMessage;
        private bool showErrorMessage;
        private Model model;
        private ASTNode? treeToPresent;

        public ASTNode? TreeToPresent
        {
            get => treeToPresent;
            set
            {
                treeToPresent = value;
                OnPropertyChanged(nameof(TreeToPresent));
            }
        }

        public ViewModel()
        {
            inputText = string.Empty;
            errorMessage = string.Empty;
            model = new Model();
            model.InputProcessed += Model_OnInputProcessed;
            model.TreeUpdated += Model_OnTreeUpdated;
            treeToPresent = null;
            ToggleErrorMessageCommand = new DelegateCommand(_ => ShowErrorMessage = !ShowErrorMessage);
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
