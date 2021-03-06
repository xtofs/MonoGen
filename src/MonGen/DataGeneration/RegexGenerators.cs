﻿using System;
using System.Linq;
using MonGen.RegexParsers;

namespace MonGen.DataGeneration
{
    public static class RegexGenerators
    {
        public static IGenerator<string> Generator(this RegexParsers.Regex alternatives)
        {
            var n = alternatives.Count;
            return from i in Generators.Range(0, n)
                   from g in Generator(alternatives[i])
                   select g;
        }

        public static IGenerator<string> Generator(Sequence sequence)
        {
            return sequence.Select(atom => Generator(atom)).Pivot().Select(items => string.Concat(items));
        }

        public static IGenerator<string> Generator(Atom atom)
        {
            var m = atom.Multiplicity;
            var expr = Generator(atom.Expression);
            return from lst in expr.Sequence(m.MinOccurs, m.MaxOccurs ?? 20)
                   select string.Concat(lst);
        }

        public static IGenerator<string> Generator(ISimpleExpression expression)
        {
            var charset = expression as Charset;
            if (charset != null)
            {
                return from i in Generators.Range(0, charset.Count)
                       select charset[i].ToString();
            }
            var literal = expression as Literal;
            if (literal != null)
            {
                return Generators.Constant(literal.Value);
            }
            var group = expression as Group;
            if (group != null)
            {
                return group.Root.Generator();
            }            
            throw new InvalidCastException($"{nameof(expression)} is not one of Charset, Literal, Group");
            //switch (expression)
            //{
            //    case Charset charset:
            //        return from i in Generators.Range(0, charset.Count)
            //               select charset[i].ToString();
            //    case Literal literal:
            //        return Generators.Constant(literal.Value);
            //    case Group group:
            //        return group.Root.Generator();
            //    default:
            //        throw new InvalidCastException($"{nameof(expression)} is not one of Charset, Literal, Group");
            //}
        }
    }
}