using System;
using System.Runtime.Serialization;

namespace SerwisMaster
{
    [Serializable]
    internal class ProbaDodaniaRodzicaElementuDoDzieckaException : Exception
    {
        public ProbaDodaniaRodzicaElementuDoDzieckaException()
        {
        }

        public ProbaDodaniaRodzicaElementuDoDzieckaException(string message) : base(message)
        {
        }

    }
}