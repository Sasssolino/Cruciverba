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
            _actions.Add(PossibleDirections.RightToLeft, (Increment, null));
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

        public (bool Found, int x, int y, PossibleDirections dir) SearchWord(string word)
        {
            for(int i = 0; i < _buttons.Count; i++)
            {

                // Todo Metodsss
                foreach(var item in _actions)
                {
                    var sos = SearchWordInDirection(i, word, item.Key);

                    if (sos.Found)
                        return sos;
                }
            }
    
            return (false, -1, -1, PossibleDirections.LeftToRight);
        }

        public (bool Found, int x, int y, PossibleDirections dir) SearchWordInLineDirection(int line, string word, PossibleDirections dir)
        {
            for (int i = 0; i < _buttons[line].Count; i++)
            {
                // Todo fare un altro dictionary per capire se c'e' abbastanza spazio
                if (!IsWordFitInSpaceDirection(i, line, word.Length, dir))
                    continue;

                var sos = ControlHorizontalWord(line, i, word);

                if (sos)
                    return (true, line, i, dir);
            }

            return (false, -1, -1, dir);
        }

        public bool ControlWordDirection(PossibleDirections dir, int line, int column, string word)
        {
            foreach (var item in word)
            {
                var charOfButton = char.Parse(_buttons[line][column].Content + "");

                if (!IsCharOfWordSameAsButtonContent(item, charOfButton))
                    return false;

                column = _actions[dir].Column(column);

                var sos = _actions[dir].Line;

                if (sos != null)
                    line = sos(line);
            }

            return true;
        }

        public bool ControlHorizontalWord(int line, int column, string word)
        {
            foreach (var item in word)
            {
                var charOfButton = char.Parse(_buttons[line][column].Content + "");

                if (!IsCharOfWordSameAsButtonContent(item, charOfButton))
                    return false;

                column++;
            }

            return true;
        }

        public bool IsCharOfWordSameAsButtonContent(char buttonContent, char wordChar)
        {
            return (buttonContent == wordChar) ? true : false;
        }

        public bool IsWordFitInSpace(int spaceGap, int worldLength)
        {
            if (worldLength > spaceGap)
                return false;

            return true;
        }
        public bool IsWordFitInSpaceDirection(int column, int line, int worldLength, PossibleDirections dir)
        {
            // Todo, Da finire
            if (_actions[dir].Column )

            if (worldLength > spaceGap)
                return false;

            return true;
        }

        private void btnSeachWord_Click(object sender, RoutedEventArgs e)
        {
            var word = GetWord(txtSeachWord).ToUpper();

            var result = SearchWord(word);

            if (!result.Found)
            {
                MessageBox.Show("la parola non e' stata trovata.");
                return;
            }

            PrintWord(result.x, result.y, word.Length);
        }

        public void PrintWord(int line, int column, int lengthWord)
        {
            for (int i = 0; i < lengthWord; i++)
                _buttons[line][column + i].Background = new SolidColorBrush(Colors.Yellow);
        }

        public string GetWord(TextBox textBox)
        {
            return textBox.Text;
        }

    }
}
