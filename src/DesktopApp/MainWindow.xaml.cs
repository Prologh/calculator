using Calculator.DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _displayBlank = string.Empty;
        private readonly IReadOnlyList<Operation> _operations;

        private Operation _currentOperationSelected;
        private Operation _lastOperationSelected;
        private double _currentValue;
        private string _displayDefault;

        /// <summary>
        /// Initializes new instance of <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _currentOperationSelected = Operation.None;
            _lastOperationSelected = Operation.None;
            _currentValue = 0d;
            _displayDefault = Button0.Content.ToString();

            var properties = typeof(Operation)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(propInfo => propInfo.GetValue(null, null))
                .OfType<Operation>()
                .ToList();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button sourceButton))
            {
                throw new ArgumentException($"{nameof(sender)} is not an instance of {nameof(Button)}.", nameof(sender));
            }

            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Clear();
                _lastOperationSelected = Operation.None;
            }
            else
            {
                if (_lastOperationSelected != Operation.None)
                {
                    txtDisplay.Text = sourceButton.Content.ToString();
                    _lastOperationSelected = Operation.None;

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

            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Text = sourceButton.Content.ToString();
                _lastOperationSelected = Operation.None;

                return;
            }
            else
            {
                if (_lastOperationSelected != Operation.None)
                {
                    txtDisplay.Text = _displayDefault;
                    _lastOperationSelected = Operation.None;
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
            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Text = _displayDefault;
                _lastOperationSelected = Operation.None;

                return;
            }
            else
            {
                if (_lastOperationSelected != Operation.None)
                {
                    txtDisplay.Text = _displayDefault;
                    _lastOperationSelected = Operation.None;

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
                var tempString = string.Empty;
                var charArray = txtDisplay.Text.ToArray();
                for (int i = 1; i < txtDisplay.Text.Length; i++)
                {
                    tempString += charArray[i - 1];
                }

                txtDisplay.Text = tempString;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            txtDisplay.Text = _displayDefault;
            txtDisplayMemory.Clear();
            txtDisplayOperation.Clear();
            _lastOperationSelected = Operation.None;
            _currentOperationSelected = Operation.None;
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
                    _lastOperationSelected = Operation.Addition;
                    _currentOperationSelected = Operation.Addition;
                    break;
                case "-":
                    _lastOperationSelected = Operation.Subtraction;
                    _currentOperationSelected = Operation.Subtraction;
                    break;
                case "*":
                    _lastOperationSelected = Operation.Multiplication;
                    _currentOperationSelected = Operation.Multiplication;
                    break;
                case "/":
                    _lastOperationSelected = Operation.Division;
                    _currentOperationSelected = Operation.Division;
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
            if (_currentOperationSelected != Operation.None)
            {
                double a = 0d, b = 0d, result = 0d;
                if (_lastOperationSelected == Operation.Result)
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
                        result = _currentOperationSelected.Function(a, b);
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
                _lastOperationSelected = Operation.Result;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                ButtonBackspace_Click(this, e);
            }
        }
    }
}
