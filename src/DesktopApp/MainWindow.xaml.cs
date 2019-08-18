using Calculator.DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEnumerable<Key> _digitsKeys;
        private readonly string _emptyTextBoxValue;
        private readonly IEnumerable<Operation> _operations;
        private readonly IEnumerable<Key> _operationsKeys;
        private readonly IEnumerable<string> _operationsSigns;

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

            _currentValue = 0d;

            _currentOperationSelected = Operation.None;
            _lastOperationSelected = Operation.None;

            _displayDefault = Button0.Content.ToString();
            _emptyTextBoxValue = string.Empty;

            _operations = typeof(Operation)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(propInfo => propInfo.GetValue(null, null))
                .OfType<Operation>()
                .Where(o => o.IsAffectingResult);
            _operationsKeys = _operations.Select(o => o.Key);
            _operationsSigns = _operations.Select(o => o.Sign);

            _digitsKeys = new List<Key>
            {
                Key.D0,
                Key.D1,
                Key.D2,
                Key.D3,
                Key.D4,
                Key.D5,
                Key.D6,
                Key.D7,
                Key.D8,
                Key.D9,
            };
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_operationsKeys.Contains(e.Key))
            {
                var operation = _operations.FirstOrDefault(o => o.Key == e.Key);

                if (operation != null)
                {
                    SetOperation(operation);
                }

                return;
            }

            if (_digitsKeys.Contains(e.Key))
            {
                AppendDigit(e.Key.ToString().TrimStart('D'));

                return;
            }

            switch (e.Key)
            {
                case Key.Back:
                    TrimLastDigit();
                    break;
                case Key.Enter:
                    ButtonEquals_Click(this, e);
                    break;
                case Key.C:
                    ButtonClear_Click(this, e);
                    break;
            }
        }

        private void ButtonDigit_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
            }

            var digit = sourceContentControl.Content.ToString();
            AppendDigit(digit);
        }

        private void ButtonZero_Click(object sender, RoutedEventArgs e)
        {
            AppendZero();
        }

        private void ButtonComma_Click(object sender, RoutedEventArgs e)
        {
            AppendComma();
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            TrimLastDigit();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ResetCalculator();
        }

        private void ButtonOperation_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
            }

            var operationSign = sourceContentControl.Content.ToString();
            var operation = _operations.FirstOrDefault(o => o.Sign == operationSign);

            if (operation != null)
            {
                SetOperation(operation);
            }
        }

        private void ButtonEquals_Click(object sender, RoutedEventArgs e)
        {
            CalculateOperation();
        }

        private void AppendComma()
        {
            if (!txtDisplay.Text.Contains(",") && (txtDisplay.Text.Length > 0))
            {
                txtDisplay.Text += ",";
            }
        }

        private void AppendDigit(string digit)
        {
            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Clear();
                _lastOperationSelected = Operation.None;
            }
            else
            {
                if (_lastOperationSelected != Operation.None)
                {
                    txtDisplay.Text = digit;
                    _lastOperationSelected = Operation.None;

                    return;
                }
            }

            if (txtDisplay.Text.Contains(_displayDefault) && txtDisplay.Text.Length == 1)
            {
                txtDisplay.Clear();
            }

            txtDisplay.Text += digit;
        }

        private void AppendZero()
        {
            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Text = "0";
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
                if (txtDisplay.Text.Length > 1 || !txtDisplay.Text.Contains("0"))
                {
                    txtDisplay.Text += "0";
                }
            }
        }

        private void CalculateOperation()
        {
            if (_currentOperationSelected == Operation.None)
            {
                return;
            }

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
                        owner: this,
                        messageBoxText: "Dividing by zero is not allowed.",
                        caption: "Error!",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    ResetCalculator();
                }
            }

            txtDisplay.Text = result.ToString();
            txtDisplayMemory.Clear();
            txtDisplayOperation.Clear();

            _lastOperationSelected = Operation.Result;
        }

        private void ResetCalculator()
        {
            txtDisplay.Text = _displayDefault;
            txtDisplayMemory.Clear();
            txtDisplayOperation.Clear();
            _lastOperationSelected = Operation.None;
            _currentOperationSelected = Operation.None;
            _currentValue = 0d;
        }

        private void SetOperation(Operation operation)
        {
            if (operation == null)
            {
                return;
            }

            _lastOperationSelected = operation;
            _currentOperationSelected = operation;
            txtDisplayOperation.Text = operation.Sign;

            if (txtDisplayMemory.Text.Equals(_emptyTextBoxValue))
            {
                txtDisplayMemory.Text = txtDisplay.Text;
            }
        }

        private void TrimLastDigit()
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
                var length = txtDisplay.Text.Length;
                txtDisplay.Text = txtDisplay.Text.Remove(length - 1);
            }
        }
    }
}
