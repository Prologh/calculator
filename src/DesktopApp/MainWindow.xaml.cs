using Calculator.DesktopApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _displayBlank = string.Empty;

        private OperationType _currentOperationSelected;
        private OperationType _lastOperationSelected;
        private double _currentValue;
        private string _displayDefault;

        /// <summary>
        /// Initializes new instance of <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _currentOperationSelected = OperationType.None;
            _lastOperationSelected = OperationType.None;
            _currentValue = 0d;
            _displayDefault = Button0.Content.ToString();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button sourceButton))
            {
                throw new ArgumentException($"{nameof(sender)} is not an instance of {nameof(Button)}.", nameof(sender));
            }

            if (OperationType.Result == _lastOperationSelected)
            {
                txtDisplay.Clear();
                _lastOperationSelected = OperationType.None;
            }
            else
            {
                if (_lastOperationSelected != OperationType.None)
                {
                    txtDisplay.Text = sourceButton.Content.ToString();
                    _lastOperationSelected = OperationType.None;
                    return;
                }
            }

            if (txtDisplay.Text.Contains(_displayDefault) && txtDisplay.Text.Length == 1)
            {
                txtDisplay.Clear();
            }

            txtDisplay.Text += sourceButton.Content;
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button sourceButton))
            {
                throw new ArgumentException($"{nameof(sender)} is not an instance of {nameof(Button)}.", nameof(sender));
            }

            if (OperationType.Result == _lastOperationSelected)
            {
                txtDisplay.Text = sourceButton.Content.ToString();
                _lastOperationSelected = OperationType.None;
                return;
            }
            else
            {
                if (_lastOperationSelected != OperationType.None)
                {
                    txtDisplay.Text = _displayDefault;
                    _lastOperationSelected = OperationType.None;
                }
                if (txtDisplay.Text.Length > 1
                    || !txtDisplay.Text.Contains(sourceButton.Content.ToString()))
                {
                    txtDisplay.Text += sourceButton.Content;
                }
            }
        }

        private void ButtonComma_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button sourceButton))
            {
                throw new ArgumentException($"{nameof(sender)} is not an instance of {nameof(Button)}.", nameof(sender));
            }

            if ((txtDisplay.Text.Contains(sourceButton.Content.ToString())) ||
                (0 == txtDisplay.Text.Length))
            {
                return;
            }
            txtDisplay.Text += sourceButton.Content.ToString();
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (OperationType.Result == _lastOperationSelected)
            {
                txtDisplay.Text = _displayDefault;
                _lastOperationSelected = OperationType.None;
                return;
            }
            else
            {
                if (_lastOperationSelected != OperationType.None)
                {
                    txtDisplay.Text = _displayDefault;
                    _lastOperationSelected = OperationType.None;
                    return;
                }
            }

            if (txtDisplay.Text.Length == 1 && !txtDisplay.Text.Contains(_displayDefault))
            {
                txtDisplay.Text = _displayDefault;
                return;
            }

            if (txtDisplay.Text.Length > 1)
            {
                string temp = string.Empty;
                char[] array = txtDisplay.Text.ToArray();
                for (int i = 1; i < txtDisplay.Text.Length; i++)
                    temp += array[i - 1];
                txtDisplay.Text = temp;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            txtDisplay.Text = _displayDefault;
            txtDisplayMemory.Clear();
            txtDisplayOperation.Clear();
            _lastOperationSelected = OperationType.None;
            _currentOperationSelected = OperationType.None;
            _currentValue = 0d;
        }

        private void ButtonOperation_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button sourceButton))
            {
                throw new ArgumentException($"{nameof(sender)} is not an instance of {nameof(Button)}.", nameof(sender));
            }

            switch (sourceButton.Content.ToString())
            {
                case "+":
                    _lastOperationSelected = OperationType.Addition;
                    _currentOperationSelected = OperationType.Addition;
                    break;
                case "-":
                    _lastOperationSelected = OperationType.Subtraction;
                    _currentOperationSelected = OperationType.Subtraction;
                    break;
                case "*":
                    _lastOperationSelected = OperationType.Multiplication;
                    _currentOperationSelected = OperationType.Multiplication;
                    break;
                case "/":
                    _lastOperationSelected = OperationType.Division;
                    _currentOperationSelected = OperationType.Division;
                    break;
            }

            txtDisplayOperation.Text = sourceButton.Content.ToString();
            if (txtDisplayMemory.Text.Equals(_displayBlank))
            {
                txtDisplayMemory.Text = txtDisplay.Text;
            }
        }

        private void ButtonEquals_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOperationSelected != OperationType.None)
            {
                double a = 0d, b = 0d, result = 0d;
                if (_lastOperationSelected == OperationType.Result)
                {
                    a = double.Parse(txtDisplay.Text);
                    b = _currentValue;
                }
                else
                {
                    a = double.Parse(txtDisplayMemory.Text);
                    b = double.Parse(txtDisplay.Text);
                    _currentValue = b;
                }

                if (_currentOperationSelected.IsAffectingResult)
                {
                    try
                    {
                        result = _currentOperationSelected.Operation(a, b);
                    }
                    catch (DivideByZeroException)
                    {
                        MessageBox.Show(
                            this,
                            "Nie da się dzielić przez zero!",
                            "BŁĄD!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        ButtonClear_Click(sender, e);
                    }
                }

                txtDisplay.Text = result.ToString();
                txtDisplayMemory.Clear();
                txtDisplayOperation.Clear();
                _lastOperationSelected = OperationType.Result;
            }
        }
    }
}
