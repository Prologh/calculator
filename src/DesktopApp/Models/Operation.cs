using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calculator.DesktopApp.Models
{
    /// <summary>
    /// Indicates the type of basic mathematical operation.
    /// </summary>
    [DebuggerDisplay("{Sign,nq} {Name,nq}")]
    public class Operation : IEquatable<Operation>
    {
        /// <summary>
        /// Gets the addition operation.
        /// </summary>
        public static Operation Addition => new Operation("+", nameof(Addition), (a, b) => a + b);

        /// <summary>
        /// Gets the division operation.
        /// <para/>
        /// Note: underlying function for division will throw a
        /// <see cref="DivideByZeroException"/> when zero is passed
        /// as a divider value.
        /// </summary>
        public static Operation Division => new Operation("/", nameof(Division), (a, b) =>
            b != 0d
                ? a / b
                : throw new DivideByZeroException($"Cannot divide {a} by zero.")
        );

        /// <summary>
        /// Gets the multiplication operation.
        /// </summary>
        public static Operation Multiplication => new Operation("*", nameof(Multiplication), (a, b) => a * b);

        /// <summary>
        /// Gets the none operation. The underlying function will not perform
        /// any calculations and will always return first argument as a result.
        /// </summary>
        public static Operation None => new Operation(string.Empty, nameof(None), NoCalculationFunction);

        /// <summary>
        /// Gets the result operation. The underlying function will not perform
        /// any calculations and will always return first argument as a result.
        /// </summary>
        public static Operation Result => new Operation("=", nameof(Result), NoCalculationFunction);

        /// <summary>
        /// Gets the substraction operation.
        /// </summary>
        public static Operation Subtraction => new Operation("-", nameof(Subtraction), (a, b) => a - b);

        private static Func<double, double, double> NoCalculationFunction => (a, b) => a;

        private Operation(string sign, string name, Func<double, double, double> function)
        {
            Sign = sign;
            Name = name;
            Function = function;
        }

        /// <summary>
        /// Gets the operation function.
        /// </summary>
        public Func<double, double, double> Function { get; }

        /// <summary>
        /// Gets a <see cref="bool"/> value indicating whether underlying
        /// function of current operation is affecting returned result.
        /// </summary>
        public bool IsAffectingResult => Function != NoCalculationFunction;

        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the string representatio of operation, usually a single sign e.g. "+".
        /// </summary>
        public string Sign { get; }

        /// <summary>
        /// Checks if current instance of <see cref="Operation"/> is equal
        /// to other <see cref="object"/> instance.
        /// </summary>
        /// <param name="other">
        /// Other <see cref="object"/> instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if provided other <see cref="object"/>
        /// instance is equal to the current one; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Operation);
        }

        /// <summary>
        /// Checks if current instance of <see cref="Operation"/> is equal
        /// to other <see cref="Operation"/> instance.
        /// </summary>
        /// <param name="other">
        /// Other <see cref="Operation"/> instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if provided other <see cref="object"/>
        /// instance is equal to the current one; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(Operation other)
        {
            return other != null &&
                   EqualityComparer<Func<double, double, double>>.Default.Equals(Function, other.Function) &&
                   IsAffectingResult == other.IsAffectingResult &&
                   Name == other.Name &&
                   Sign == other.Sign;
        }

        /// <summary>
        /// Gets the hashcode for this instance.
        /// </summary>
        /// <returns>
        /// Integer hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -1578661628;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<double, double, double>>.Default.GetHashCode(Function);
            hashCode = hashCode * -1521134295 + IsAffectingResult.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Sign);

            return hashCode;
        }

        public static bool operator ==(Operation operation1, Operation operation2)
        {
            return EqualityComparer<Operation>.Default.Equals(operation1, operation2);
        }

        public static bool operator !=(Operation operation1, Operation operation2)
        {
            return !(operation1 == operation2);
        }
    }
}
