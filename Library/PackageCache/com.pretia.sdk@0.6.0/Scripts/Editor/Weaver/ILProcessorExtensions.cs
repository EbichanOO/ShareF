using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PretiaArCloud.Networking.Weaver
{
    internal static class ILProcessorExtensions
    {
        internal static Instruction CreateLoadIntegerInstruction(this ILProcessor iLProcessor, int value)
        {
            if (value > 8)
            {
                if (value < 128)
                {
                    return iLProcessor.Create(OpCodes.Ldc_I4_S, (sbyte)value);
                }
                else
                {
                    return iLProcessor.Create(OpCodes.Ldc_I4, value);
                }
            }
            else
            {
                OpCode opCode = OpCodes.Nop;
                switch (value)
                {
                    case 0:
                        opCode = OpCodes.Ldc_I4_0;
                        break;
                    case 1:
                        opCode = OpCodes.Ldc_I4_1;
                        break;
                    case 2:
                        opCode = OpCodes.Ldc_I4_2;
                        break;
                    case 3:
                        opCode = OpCodes.Ldc_I4_3;
                        break;
                    case 4:
                        opCode = OpCodes.Ldc_I4_4;
                        break;
                    case 5:
                        opCode = OpCodes.Ldc_I4_5;
                        break;
                    case 6:
                        opCode = OpCodes.Ldc_I4_6;
                        break;
                    case 7:
                        opCode = OpCodes.Ldc_I4_7;
                        break;
                    case 8:
                        opCode = OpCodes.Ldc_I4_8;
                        break;
                }

                return iLProcessor.Create(opCode);
            }
        }
    }
}