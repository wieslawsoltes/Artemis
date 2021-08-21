﻿using Artemis.Core;

namespace Artemis.VisualScripting.Nodes
{
    [Node("To String", "Converts the input to a string.")]
    public class ConvertToStringNode : Node
    {
        #region Properties & Fields

        public InputPin<object> Input { get; }

        public OutputPin<string> String { get; }

        #endregion

        #region Constructors

        public ConvertToStringNode()
            : base("To String", "Converts the input to a string.")
        {
            Input = CreateInputPin<object>();
            String = CreateOutputPin<string>();
        }

        #endregion

        #region Methods

        public override void Evaluate()
        {
            String.Value = Input.Value?.ToString();
        }

        #endregion
    }

    [Node("To Integer", "Converts the input to an integer.")]
    public class ConvertToIntegerNode : Node
    {
        #region Properties & Fields

        public InputPin<object> Input { get; }

        public OutputPin<int> Integer { get; }

        #endregion

        #region Constructors

        public ConvertToIntegerNode()
            : base("To Integer", "Converts the input to an integer.")
        {
            Input = CreateInputPin<object>();
            Integer = CreateOutputPin<int>();
        }

        #endregion

        #region Methods

        public override void Evaluate()
        {
            Integer.Value = Input.Value switch
            {
                int input => input,
                double input => (int) input,
                float input => (int) input,
                _ => TryParse(Input.Value)
            };
        }

        private int TryParse(object input)
        {
            if (!int.TryParse(input?.ToString(), out int value))
                value = 0;

            return value;
        }

        #endregion
    }

    [Node("To Double", "Converts the input to a double.")]
    public class ConvertToDoubleNode : Node
    {
        #region Properties & Fields

        public InputPin<object> Input { get; }

        public OutputPin<double> Double { get; }

        #endregion

        #region Constructors

        public ConvertToDoubleNode()
            : base("To Double", "Converts the input to a double.")
        {
            Input = CreateInputPin<object>();
            Double = CreateOutputPin<double>();
        }

        #endregion

        #region Methods

        public override void Evaluate()
        {
            Double.Value = Input.Value switch
            {
                int input => input,
                double input => input,
                float input => input,
                _ => TryParse(Input.Value)
            };
        }

        private double TryParse(object input)
        {
            if (!double.TryParse(input?.ToString(), out double value))
                value = 0.0;

            return value;
        }

        #endregion
    }

    [Node("To Float", "Converts the input to a float.")]
    public class ConvertToFloatNode : Node
    {
        #region Properties & Fields

        public InputPin<object> Input { get; }

        public OutputPin<float> Float { get; }

        #endregion

        #region Constructors

        public ConvertToFloatNode()
            : base("To Float", "Converts the input to a float.")
        {
            Input = CreateInputPin<object>();
            Float = CreateOutputPin<float>();
        }

        #endregion

        #region Methods

        public override void Evaluate()
        {
            Float.Value = Input.Value switch
            {
                int input => input,
                double input => (float) input,
                float input => input,
                _ => TryParse(Input.Value)
            };
        }

        private float TryParse(object input)
        {
            if (!float.TryParse(input?.ToString(), out float value))
                value = 0.0f;

            return value;
        }

        #endregion
    }
}