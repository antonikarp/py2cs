using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;

namespace py2cs {
    public class EvalVisitor : LabeledExprBaseVisitor<int> {
        Dictionary<string, int> memory = new Dictionary<string, int>();
        public override int VisitAssign([NotNull] LabeledExprParser.AssignContext context)
        {
            string id = context.ID().GetText();
            int value = Visit(context.expr());
            memory[id] = value;
            return value;
        }
        public override int VisitPrintExpr([NotNull] LabeledExprParser.PrintExprContext context)
        {
            int value = Visit(context.expr());
            Console.WriteLine(value);
            return 0; // dummy value
        }
        public override int VisitInt([NotNull] LabeledExprParser.IntContext context)
        {
            return Int32.Parse(context.INT().GetText());
        }
        public override int VisitId([NotNull] LabeledExprParser.IdContext context)
        {
            string id = context.ID().GetText();
            if (memory.ContainsKey(id)) {
                return memory[id];
            }
            return 0;
        }
        public override int VisitMulDiv([NotNull] LabeledExprParser.MulDivContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == LabeledExprParser.MUL) {
                return left * right;
            } else{
                return left / right;
            }
        }
        public override int VisitAddSub([NotNull] LabeledExprParser.AddSubContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == LabeledExprParser.ADD) {
                return left + right;
            } else {
                return left - right;
            }
        }
        public override int VisitParens([NotNull] LabeledExprParser.ParensContext context)
        {
            return Visit(context.expr());
        }

    }

}