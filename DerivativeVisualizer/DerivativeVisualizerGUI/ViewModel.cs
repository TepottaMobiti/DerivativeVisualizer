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
    // TODO: Csinálj egy modellt ami összevonja a classok funckionalitását, és az invokáljon eventeket, stb.
    public class ViewModel : INotifyPropertyChanged
    {
        private string inputText;
        private Brush backgroundColor;
        private Tokenizer? tokenizer;
        private Parser? parser;

        public ViewModel()
        {
            inputText = string.Empty;
            backgroundColor = Brushes.White;
        }

        public string InputText
        {
            get => inputText;
            set
            {
                inputText = value;
                OnPropertyChanged(nameof(inputText));
                ValidateInput();
            }
        }

        public Brush BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnPropertyChanged(nameof(backgroundColor));
            }
        }

        // TODO
        // Inkább úgy legyen, hogy eventekkel játszol a modelben, és az event handlerben állítod a background colort. Az eventben küldd el az üzit is, legyen egy box ennek, ami a hibaüzenetet vagy
        // Jó inputot ír ki.
        private void ValidateInput()
        {
            tokenizer = new Tokenizer(InputText);
            bool success = false;

            try
            {
                parser = new Parser(tokenizer.Tokenize());
                parser.ParseExpression();
                success = true;
            } catch (Exception) { }

            BackgroundColor = success ? Brushes.LightGreen : Brushes.LightCoral;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
