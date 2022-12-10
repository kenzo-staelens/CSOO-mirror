using System;
using System.Collections.Generic;
using System.Configuration;
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

using Logica;
using Microsoft.Win32;

namespace BrainfuckGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <see cref="https://learn.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-wpf?view=vs-2022"/>
    public partial class MainWindow : Window {
        private Func<char> Input;
        private Action<string> Output;
        private Action Tick;
        BfInterpreter interpreter;
        public string TextInput {get; set;}
        public int Delaytime { get; private set; }

        public MainWindow() {
            InitializeComponent();
            Delaytime = 0;
            Input = () => {
                InputBox dialogBox = new InputBox();
                string dialogResult = dialogBox.ShowDialog();
                return (dialogResult == null || dialogResult.Length==0) ? '\n' : dialogResult[0];
            };
            Output = write => { OutputBox.Text = OutputBox.Text + write; };
            Tick = () => {
                string serialized = interpreter.Program.Serialize();
                int pointer = interpreter.ProgramPointer;
                string t = serialized.Substring(0, pointer) + "\"" +
                serialized.Substring(pointer, 1) + "\""  +
                serialized.Substring(pointer+ 1, serialized.Length - pointer - 1);
                RunningCode.Text = t;
                Task.Delay(Delaytime);
            };
            interpreter = new BfInterpreter(Input, Output);
            interpreter.Tick = Tick;

            ToHideLabel.Visibility = Visibility.Collapsed;
            Delay.Visibility = Visibility.Collapsed;//implementatie werkt nog niet helemaal

        }

        private void Run(object sender, RoutedEventArgs e) {
            try {
                Delaytime = Int32.Parse(Delay.Text);
                Tick = () => {
                    string serialized = interpreter.Program.Serialize();
                    int pointer = interpreter.ProgramPointer;
                    string t = serialized.Substring(0, pointer) + "\"" +
                    serialized.Substring(pointer, 1) + "\"" +
                    serialized.Substring(pointer + 1, serialized.Length - pointer - 1);
                    RunningCode.Text = t;
                    Task.Delay(Delaytime);
                };
                interpreter.Tick = Tick;
            }
            catch {
                MessageBox.Show("Error while parsing "+Delay.Text + " to number", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Reset(sender, e);
            CodeBox.IsReadOnly = true;
            MemViewBtn.IsEnabled = true;
            interpreter.Interpret();
            CodeBox.IsReadOnly = false;
        }
        private void Clear(object sender, RoutedEventArgs e) {
            CodeBox.Text = "";
        }
        private void Save(object sender, RoutedEventArgs e) {
            var dlg = new SaveFileDialog();
            dlg.Filter = "brainfuck files (*.bf)|*.bf|text files (*.txt)|*.txt";
            bool OK = (bool)dlg.ShowDialog();
            if (!OK) return;
            interpreter.Save(dlg.FileName, CodeBox.Text);
        }
        private void Load(object sender, RoutedEventArgs e) {
            var dlg = new OpenFileDialog();
            dlg.Filter = "brainfuck files (*.bf)|*.bf|text files (*.txt)|*.txt";
            bool OK = (bool)dlg.ShowDialog();
            if(!OK ) return;
            string filename = dlg.FileName;
            CodeBox.Text = interpreter.LoadRaw(dlg.FileName);
            Reset(sender, e);
        }

        private void Step(object sender, RoutedEventArgs e) {
            try {
                interpreter.Step();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message + "\ntry clicking the reset button", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
        private void Memview(object sender, RoutedEventArgs e) {
            OutputBox.Text = "";
            foreach(byte b in interpreter.MemoryView) {
                OutputBox.Text += b.ToString() + " | ";
            }
        }

        private void Reset(object sender, RoutedEventArgs e) {
            interpreter.LoadProgram(CodeBox.Text);
            RunningCode.Text = interpreter.Program.Serialize();
            OutputBox.Text = "";
            MemViewBtn.IsEnabled = false;
            interpreter.reset();
        }
        
    }

    /// <summary>
    /// implementatie voor een dialog box
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/8103743/wpf-c-sharp-inputbox"/>
    public class InputBox {
        Window Box = new Window();//window for the inputbox
        StackPanel sp1 = new StackPanel();// items container
        string title = "InputBox";//title as heading
        string defaulttext = "Input...";//default textbox content
        string okbuttontext = "OK";//Ok button content
        bool clicked = false;
        TextBox input = new TextBox();
        Button ok = new Button();
        bool inputreset = false;

        public InputBox() {
            windowdef();
        }

        private void windowdef()// window building - check only for window size
        {
            Box.Height = 130;// Box Height
            Box.Width = 300;// Box Width
            Box.Title = title;
            Box.Content = sp1;
            Box.Closing += Box_Closing;

            TextBlock content = new TextBlock();
            content.Text = "Input gevraagd:";
            content.Padding = new Thickness(5);
            sp1.Children.Add(content);
    
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.Text = defaulttext;
            input.Width = 250;
            input.Height = 20;
            input.Margin = new Thickness(5, 0, 0, 0);
            input.MouseEnter += input_MouseDown;
            sp1.Children.Add(input);

            ok.Width = 80;
            ok.HorizontalAlignment = HorizontalAlignment.Left;
            ok.Click += ok_Click;
            ok.Content = okbuttontext;
            ok.Margin = new Thickness(175,10,0,0);
            sp1.Children.Add(ok);

        }

        void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!clicked)
                e.Cancel = true;
        }

        private void input_MouseDown(object sender, MouseEventArgs e) {
            if ((sender as TextBox).Text == defaulttext && inputreset == false) {
                (sender as TextBox).Text = null;
                inputreset = true;
            }
        }

        void ok_Click(object sender, RoutedEventArgs e) {
            clicked = true;
            if (input.Text == defaulttext || input.Text == "") input.Text = "";
            Box.Close();
            clicked = false;
        }

        public string ShowDialog() {
            Box.ShowDialog();
            return input.Text;
        }
    }
}
