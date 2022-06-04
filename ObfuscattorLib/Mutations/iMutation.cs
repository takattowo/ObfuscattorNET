using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace ObfuscattorLib.Mutations
{
    public interface iMutation
    {
        void Process(MethodDef method, ref int index);
        void Prepare(TypeDef type);
        bool Supported(Instruction instr);
    }
}
