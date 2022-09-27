using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Paolo Magnani 4i 09/24/2022

// Piccolo problema di logica di base, mi sa che ora come ora sto riciclando tutta la matrice per il numero di metodi che ho, ma penso che la soluzione migliore sia
// quella di ciclare solo una volta e poi ogni volta controllare per ogni possibilita'

namespace Cruciverba
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string _content;
        List<List<Button>> _buttons;
        public Dictionary<PossibleDirections, (Func<int, int> Line, Func<int, int> Column)> _actions;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _actions = new Dictionary<PossibleDirections, (Func<int, int> , Func<int, int>)>();

            _actions.Add(PossibleDirections.LeftToRight, (Increment, null));
            _actions.Add(PossibleDirections.RightToLeft, (Decrement, null));

            _actions.Add(PossibleDirections.TopToBottom, (null, Increment));
            _actions.Add(PossibleDirections.BottomToTop, (null, Decrement));

            _actions.Add(PossibleDirections.TopLeftToBottomRight, (Increment, Increment));
            _actions.Add(PossibleDirections.BottomRightToTopLeft, (Decrement, Decrement));

            _actions.Add(PossibleDirections.TopRightToBottomLeft, (Increment, Decrement));
            _actions.Add(PossibleDirections.BotomLeftToTopRight, (Decrement, Increment));
        }


        public int Increment(int number) => ++number;

        public int Decrement(int numer) => --numer;

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = GetOpenFileDialog();

            var result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == false)
                return;

            if (!IsFormatCorrect(dialog.FileName))
                return; //Todo far uscire un messaggiox

            SetContent(dialog.FileName);

            //Todo CreateGridOfButtons(_content);
        }

        bool IsFormatCorrect(string text)
        {
            return true;
        }

        private void SetContent(string path)
            => _content = File.ReadAllText(path);

        private OpenFileDialog GetOpenFileDialog()
        {
            return  new OpenFileDialog()
            {
                FileName = "Document",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
        }


        private void btnSpawnButtons_Click(object sender, RoutedEventArgs e)
        {
            _buttons = new List<List<Button>>();

            if (_content == null)
                return;

            CreateGridOfButtons(_content);
        }

        private void CreateGridOfButtons(string text)
        {
            text = text.Trim();

            for (int i = 0; text.Length != 0; i++)
            {
                // start method

                var lenght = GetLenghtOfLine(text);

                var textLine = text.Substring(0, lenght); // il -1 toglie il \r

                // var che serve per togliere i cazzi
                var sos = (lenght != text.Length) ? 2 : 0;

                text = text.Substring(lenght + sos, text.Length - lenght - sos); // il + 1 toglie il \n

                var line = CreateNewLineOfButtons(textLine, i);

                _buttons.Add(line);
            }
        }

        int GetLenghtOfLine(string text)
        {
            var s = text.IndexOf('\n');

            if (s == -1)
                return text.Length;
            
            return s - 1;
        }

        private List<Button> CreateNewLineOfButtons(string text, int height)
        {
            var line = new List<Button>();

            for (int i = 0; i < text.Length; i++)
            {
                var button = CrateButton(i, height, content: text[i]);
                line.Add(button);
            }

            return line;
        }

        public Button CrateButton(int top, int left, int width = 29, int height = 29, char content = ' ')
        {
            var button = new Button()
            {
                Width = width,
                Height = height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(top * 30, left * 30, 0, 0),
                Content = content
            };

            grdButtons.Children.Add(button);

            return button;
        }
        
        public (PossibleDirections dir, int x, int y) SearchWord(string word)
        {
            for(int i = 0; i < _buttons.Count; i++)
            {
                var risp = SearchWordInLineDirections(i, word);

                if (risp.Direction != PossibleDirections.NotFound)
                    return risp;
            }
    
            return (PossibleDirections.NotFound, - 1, -1);
        }

        public (PossibleDirections Direction, int x, int y) SearchWordInLineDirections(int line, string word)
        {
            for (int i = 0; i < _buttons[line].Count; i++)
            {
                // Todo fare un altro dictionary per capire se c'e' abbastanza spazio
                // anzi , forse ancora meglio controllare se incrementa o decrementa per poi capire cosa bisogna fare.
                // Ricoedarsi che nel dictionary tra le possibili opzioni possiamo trovare anche il null, quindi gestirlo

                var sos = ControlWordEveryDirections(line, i, word);

                if (sos != PossibleDirections.NotFound)
                    return (sos, line, i);
            }

            return (PossibleDirections.NotFound, -1, -1);
        }

        public PossibleDirections ControlWordEveryDirections(int line, int column, string word)
        {
            foreach(var item in _actions)
            {
                if (!IsWordFitInSpaceDirection(line, column, word.Length, item.Key))
                    continue;

                var sos = ControlWordDirection(line, column, word, item.Key);

                if (sos)
                    return item.Key;
            }

            return PossibleDirections.NotFound;
        }

        public bool ControlWordDirection(int line, int column, string word, PossibleDirections direction)
        {
            foreach (var item in word)
            {
                var charOfButton = char.Parse(_buttons[line][column].Content + "");

                if (!IsCharOfWordSameAsButtonContent(item, charOfButton))
                    return false;

                // A sto punto posso anche non usare la variabile di lavoro, ma usare direttamente dictionary
                var actions = _actions[direction];

                // Per farlo come diceva il prof dovrei usare solo un metodo e
                // quan non fare questo ma chiamare il valore del dictionary
                if(actions.Column != null)
                    column = actions.Column(column);
                
                if (actions.Line != null)
                    line = actions.Line(line);
            }

            return true;
        }

        public bool IsCharOfWordSameAsButtonContent(char buttonContent, char wordChar)
        {
            return (buttonContent == wordChar) ? true : false;
        }

        public bool IsWordFitInSpaceDirection(int column, int line, int worldLength, PossibleDirections dir)
        {
            var risp = IsThereEnoughtSpace(_actions[dir].Column, column);

            if (!risp)
                return false;

            risp = IsThereEnoughtSpace(_actions[dir].Line, line);

            return risp;
        }

        // Questo funziona solo se nelle funzioni di incremento non viene usato un * o /
        private bool IsThereEnoughtSpace(Func<int, int> func, int nPos) 
        {
            if (func == null)
                return true;

            var sos = func(0);
            //var space = (sos * (nPos + 1)) + _buttons.Count;
            var space = (sos * (nPos + 1));

            if (space < 0 || space > _buttons.Count)
                return false;

            return true;
        }

        private void btnSeachWord_Click(object sender, RoutedEventArgs e)
        {
            var word = GetWord(txtSeachWord).ToUpper();

            var result = SearchWord(word);

            if (result.dir == PossibleDirections.NotFound)
            {
                MessageBox.Show("la parola non e' stata trovata.");
                return;
            }

            PrintWord(result.x, result.y, word.Length, result.dir);
        }

        public void PrintWord(int line, int column, int lengthWord, PossibleDirections dir)
        {
            for (int i = 0; i < lengthWord; i++)
            {
                _buttons[line][column].Background = new SolidColorBrush(Colors.Yellow);

                var sos = _actions[dir];

                if (sos.Column != null)
                    column = sos.Column(column);
                
                if (sos.Line != null)
                    line = sos.Line(line);
            }
        }

        public string GetWord(TextBox textBox)
        {
            return textBox.Text;
        }
    }
}
