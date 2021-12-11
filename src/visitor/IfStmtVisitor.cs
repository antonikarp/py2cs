﻿using System;
using Antlr4.Runtime.Misc;

// This is a visitor to be used to compute an if statement
public class IfStmtVisitor : Python3ParserBaseVisitor<IfStmt>
{
    public IfStmt result;

    public override IfStmt VisitIf_stmt([NotNull] Python3Parser.If_stmtContext context)
    {
        result = new IfStmt();
        int n = context.ChildCount;
        int i = 0;
        while (i < context.ChildCount)
        {
            if (context.GetChild(i).ToString() == "if")
            {
                ++i;
                OrTestVisitor conditionVisitor = new OrTestVisitor();
                context.GetChild(i).Accept(conditionVisitor);
                string line = "if (" + conditionVisitor.result.ToString() + ")";
                IndentedLine newLine = new IndentedLine(line, 0);
                result.lines.Add(newLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
            }
            else if (context.GetChild(i).ToString() == "elif")
            {
                ++i;
                OrTestVisitor conditionVisitor = new OrTestVisitor();
                context.GetChild(i).Accept(conditionVisitor);
                string line = "else if (" + conditionVisitor.result.ToString() + ")";
                IndentedLine newLine = new IndentedLine(line, 0);
                result.lines.Add(newLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
            }
            else if (context.GetChild(i).ToString() == "else")
            {
                string line = "else";
                IndentedLine newLine = new IndentedLine(line, 0);
                result.lines.Add(newLine);
                IndentedLine openingBraceLine = new IndentedLine("{", 1);
                result.lines.Add(openingBraceLine);
            }
            else if (context.GetChild(i).ToString() == ":")
            {
                ++i;
                SuiteVisitor newVisitor = new SuiteVisitor();
                context.GetChild(i).Accept(newVisitor);
                int m = newVisitor.result.lines.Count;
                for (int j = 0; j < m - 1; ++j)
                {
                    result.lines.Add(newVisitor.result.lines[j]);
                }
                // Indent back after the last line
                IndentedLine lastLine = new IndentedLine(newVisitor.result.lines[m - 1].line, -1);
                result.lines.Add(lastLine);
                // End the block with a closing brace.
                IndentedLine closingBraceLine = new IndentedLine("}", 0);
                result.lines.Add(closingBraceLine);
            }
            ++i;
        }
        return result;
    }




    /*public override IfStmt VisitIf_stmt([NotNull] Python3Parser.If_stmtContext context)
    {
        result = new IfStmt();
        OrTestVisitor conditionVisitor = new OrTestVisitor();
        context.GetChild(1).Accept(conditionVisitor);
        string line = "if (" + conditionVisitor.result.ToString() + ")";
        IndentedLine conditionLine = new IndentedLine(line, 0);
        result.lines.Add(conditionLine);
        string line2 = "{";
        IndentedLine openingBraceLine = new IndentedLine(line2, 1);
        result.lines.Add(openingBraceLine);
        int i = 2;
        while (i < context.ChildCount)
        {
            if (con
            if (context.GetChild(i).ToString() == ":")
            {
                ++i;
                SuiteVisitor newVisitor = new SuiteVisitor();
                context.GetChild(i).Accept(newVisitor);
                int n = newVisitor.result.lines.Count;
                for (int j = 0; j < n - 1; ++j)
                {
                    result.lines.Add(newVisitor.result.lines[j]);
                }
                // Indent back after the last line
                IndentedLine lastLine = new IndentedLine(newVisitor.result.lines[n - 1].line, -1);
                result.lines.Add(lastLine);
            }
            ++i;

        }

        string line3 = "}";
        IndentedLine closingBraceLine = new IndentedLine(line3, 0);
        result.lines.Add(closingBraceLine);
        return result;
    }*/

}