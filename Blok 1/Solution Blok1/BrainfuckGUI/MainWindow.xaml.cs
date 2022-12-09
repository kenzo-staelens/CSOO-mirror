using System;
using System.Collections.Generic;
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

        public MainWindow() {
            
            
            InitializeComponent();
            Input = () => {
                InputBox dialogBox = new InputBox();
                string dialogResult = dialogBox.ShowDialog();
                return (dialogResult == null ? '\n' : dialogResult[0]);
            };
            Output = write => { OutputBox.Text = OutputBox.Text + write; Console.WriteLine(write); };
            Tick = () => { };
            interpreter = new BfInterpreter(Input, Output);
            interpreter.Tick = Tick;
        }

        private void Run(object sender, RoutedEventArgs e) {
            CodeBox.IsReadOnly = true;
            MemViewBtn.IsEnabled = true;
            interpreter.LoadProgram(CodeBox.Text);
            RunningCode.Text = interpreter.Program.Serialize();
            OutputBox.Text = "";
            RunBtn.Content = "pause";
            interpreter.Interpret();
            CodeBox.IsReadOnly = false;
            MemViewBtn.IsEnabled = false;
        }
        private void Clear(object sender, RoutedEventArgs e) { }
        private void Save(object sender, RoutedEventArgs e) { }
        private void Load(object sender, RoutedEventArgs e) { }
        private void Step(object sender, RoutedEventArgs e) {//+++++++++++[->++++++<]-,.
            interpreter.Step();
        }
        private void Memview(object sender, RoutedEventArgs e) { }
        
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
        string errormessage = "Invalid answer";//error messagebox content
        string errortitle = "Error";//error messagebox heading title
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
            if (input.Text == defaulttext || input.Text == "")
                MessageBox.Show(errormessage, errortitle);
            else {
                Box.Close();
            }
            clicked = false;
        }

        public string ShowDialog() {
            Box.ShowDialog();
            return input.Text;
        }
    }
}
