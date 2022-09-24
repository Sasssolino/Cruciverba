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
    }

    public Tuple<int ,int> Franco(string word)
    {
        for(int i = 0; i < _buttons.Count; i++)
        {

        }

        return (-1, -1);
    }
}
