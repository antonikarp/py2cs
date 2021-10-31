using System;
using Antlr4.Runtime.Misc;

namespace py2cs
{
    public class ShortToUnicodeString : ArrayInitBaseListener {
        public override void EnterInit([NotNull] ArrayInitParser.InitContext context)
        {
            Console.Write('"');
        }
        public override void ExitInit([NotNull] ArrayInitParser.InitContext context)
        {
            Console.Write('"');
        }
        public override void EnterValue([NotNull] ArrayInitParser.ValueContext context)
        {
            int value = Int32.Parse(context.INT().GetText());
            Console.Write("\\u");
            Console.Write(value.ToString("0000"));
        }
    }


}