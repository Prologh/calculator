using Calculator.DesktopApp.Models;
using System;
using System.Collections.Generic;
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
        private readonly string _emptyTextBoxValue;
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

            _currentValue = 0d;

            _currentOperationSelected = Operation.None;
            _lastOperationSelected = Operation.None;

            _displayDefault = Button0.Content.ToString();
            _emptyTextBoxValue = string.Empty;

            _operations = typeof(Operation)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(propInfo => propInfo.GetValue(null, null))
                .OfType<Operation>()
                .Where(o => o.IsAffectingResult)
                .ToList();
        }

        private void ButtonDigit_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
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
                    txtDisplay.Text = sourceContentControl.Content.ToString();
                    _lastOperationSelected = Operation.None;

                    return;
                }
            }

            if (txtDisplay.Text.Contains(_displayDefault) && txtDisplay.Text.Length == 1)
            {
                txtDisplay.Clear();
            }

            txtDisplay.Text += sourceContentControl.Content;
        }

        private void ButtonZero_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
            }

            if (Operation.Result == _lastOperationSelected)
            {
                txtDisplay.Text = sourceContentControl.Content.ToString();
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
                    || !txtDisplay.Text.Contains(sourceContentControl.Content.ToString()))
                {
                    txtDisplay.Text += sourceContentControl.Content;
                }
            }
        }

        private void ButtonComma_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
            }

            if (txtDisplay.Text.Contains(sourceContentControl.Content.ToString())
                || (0 == txtDisplay.Text.Length))
            {
                return;
            }

            txtDisplay.Text += sourceContentControl.Content.ToString();
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
                txtDisplay.Text = txtDisplay.Text.Remove(0, 1);
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
            if (!(sender is ContentControl sourceContentControl))
            {
                throw new ArgumentException(
                    $"{nameof(sender)} is not an instance of {nameof(ContentControl)}.",
                    nameof(sender));
            }

            var operationSign = sourceContentControl.Content.ToString();
            var operation = _operations.FirstOrDefault(o => o.Sign == operationSign);

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

        private void ButtonEquals_Click(object sender, RoutedEventArgs e)
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
                    ButtonClear_Click(sender, e);
                }
            }

            txtDisplay.Text = result.ToString();
            txtDisplayMemory.Clear();
            txtDisplayOperation.Clear();

            _lastOperationSelected = Operation.Result;
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
