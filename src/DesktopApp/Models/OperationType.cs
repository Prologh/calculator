using System;

namespace Calculator.DesktopApp.Models
{
    /// <summary>
    /// Indicates the type of mathematical operation.
    /// </summary>
    public class OperationType
    {
        public static readonly OperationType None = new OperationType(sign: string.Empty, operation: (a, b) => a);

        public static readonly OperationType Addition = new OperationType(sign: "+", operation: (a, b) => a + b);

        public static readonly OperationType Subtraction = new OperationType(sign: "-", operation: (a, b) => a - b);

        public static readonly OperationType Multiplication = new OperationType(sign: "*", operation: (a, b) => a * b);

        public static readonly OperationType Division = new OperationType(sign: "/", operation: (a, b) =>
            {
                if (b != 0d)
                {
                    return a / b;
                }
                else
                {
                    throw new DivideByZeroException($"Division of {a} by zero.");
                }
            }
        );

        public static readonly OperationType Result = new OperationType(sign: "=", operation: (a, b) => a);

        private OperationType(string sign, Func<double, double, double> operation)
        {
            Sign = sign;
            Operation = operation;
        }

        public string Sign { get; }

        public Func<double, double, double> Operation { get; }

        public bool IsAffectingResult
        {
            get
            {
                return this != None && this != Result;
            }
        }
    }
}
