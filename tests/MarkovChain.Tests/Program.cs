using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using CommandLine;

namespace MarkovChain.Tests
{
    [TestFixture]
    public class Program
    {
        [Test]
        public void Options()
        {
            string[] args = new string[]{};
            ParserResult<Cli.Program.Options> result;
            result = Parser.Default.ParseArguments<Cli.Program.Options>(args);
            result.WithParsed<Cli.Program.Options>(options =>
                {
                    Assert.AreEqual(1, options.SentenceCount);
                    Assert.AreEqual(new string[]{ ".", "?", "!", "‽" }, options.StartingTokens);
                    Assert.IsFalse(options.Verbose);
                    Assert.AreEqual(new string[]{"-"}, options.SampleFiles);
                }
            ).WithNotParsed<Cli.Program.Options>(errors =>
                {
                    Assert.Fail(
                        $"parsing empty args resulted in errors: {String.Join(", ", errors)}"
                    );
                }
            );

            args = new string[]{ "1.txt", "2.txt", "-n", "42", "--start", "Test1", "Test2", "-v"};
            result = Parser.Default.ParseArguments<Cli.Program.Options>(args);
            result.WithParsed<Cli.Program.Options>(options =>
                {
                    Assert.AreEqual(42, options.SentenceCount);
                    Assert.AreEqual(new string[]{ "Test1", "Test2" }, options.StartingTokens);
                    Assert.IsTrue(options.Verbose);
                    Assert.AreEqual(new string[]{ "1.txt", "2.txt" }, options.SampleFiles);
                }
            ).WithNotParsed<Cli.Program.Options>(errors =>
                {
                    Assert.Fail(
                        $"parsing {{{String.Join(", ", args)}}} resulted in errors: {String.Join(", ", errors)}"
                    );
                }
            );
        }

        [Test]
        public void ParseFiles()
        {
            Assert.AreEqual(
                new string[]{"A", "b", "c", "d", "e", ".", "1", "2", "3", "4", "."},
                Cli.Program.ParseFiles(
                    Enumerable.Range(1, 2).Select(number =>
                        {
                            return $"{TestContext.CurrentContext.TestDirectory}/SampleFiles/{number}.txt";
                        }
                    ).GetEnumerator()
                ).ToArray()
            );
        }

        [Test]
        public void GenerateSentences()
        {
            Chain<string> chain = new Chain<string>();
            chain.AddTransitions(new string[]{"a", "b", "c", "d"}.Cast<string>().GetEnumerator());
            chain.AddTransitions(new string[]{"a", "b", "1", "2"}.Cast<string>().GetEnumerator());
            chain.AddTransitions(new string[]{"a", "b", "3", "4", "a", "b", "5", "6"}.Cast<string>().GetEnumerator());
            Assert.IsTrue(chain.TryGetState("a", out ChainState<string> state));
            IEnumerable<string> generator = Cli.Program.GenerateSentences(
                new HashSet<string>(){"a"},
                new List<ChainState<string>>(){state}
            );
            HashSet<string> sentences = new HashSet<string>(generator.Take(20));
            sentences.ExceptWith(new string[]{"b c d", "b 1 2", "b 3 4 a", "b 5 6"});
            Assert.AreEqual(0, sentences.Count);
        }
    }
}