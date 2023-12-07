namespace AdventOfCode2022.Solutions
{
    internal struct Instruction
    {
        internal readonly string Verb;
        internal readonly int Arg1;
        
        internal Instruction(string verb, int arg1 = 0)
        {
            Verb = verb;
            Arg1 = arg1;
        }
    }
}