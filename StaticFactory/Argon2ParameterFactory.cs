using NSec.Cryptography; 
namespace Lorecraft_API.StaticFactory
{
    public class Argon2ParameterFactory
    {
        private const int Iterations = 4;
        private const int DegreeOfParallelism = 1;
        private const int MemorySize = 65536;
        public Argon2Parameters CreateParameters() => new(){
            MemorySize = MemorySize,
            NumberOfPasses = Iterations,
            DegreeOfParallelism = DegreeOfParallelism
        };

    }
}