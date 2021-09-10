using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CommandLine;

namespace MarkovChain.Cli
{
    /// <summary>Class implementing the Cli app</summary>
    public static class Program
    {
        /// <summary>Class representing parsed Cli options.</summary>
        /// <remarks><see href="https://github.com/commandlineparser/commandline">CommandLine</see> is used for parsing.</remarks>
        public class Options
        {
            [Option('n', "sentences", Default = 1, HelpText = "How many sentences should be produced, n < 1 for unlimited.")]
            public int SentenceCount { get; set; }

            [Option('s', "start", Default = new string[]{ ".", "?", "!", "‽" }, HelpText = "Tokens which indicate the start of a new sentence.")]
            public IEnumerable<string> StartingTokens { get; set; }

            [Option('v', "verbose", Default = false, HelpText = "Show tokens while parsing.")]
            public bool Verbose { get; set; }

            [Value(0, Default = new string[]{"-"}, HelpText = "Files to read sample text from, - represents stdin.")]
            public IEnumerable<string> SampleFiles { get; set; }
        }

        /// <summary>Entry point of the Cli app</summary>
        /// <param name="args">Cli arguments</params>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options =>
                {
                    Chain<string> chain = new Chain<string>();

                    using(IEnumerator<string> tokens = Program.ParseFiles(options.SampleFiles.GetEnumerator()).GetEnumerator())
                    {
                            if (options.Verbose)
                            {
                                chain.AddTransitions(Program.WriteTokens(tokens));
                            }
                            else
                            {
                                chain.AddTransitions(tokens);
                            }
                    }

                    List<ChainState<string>> initialStates = new List<ChainState<string>>();
                    HashSet<string> initialTokens = new HashSet<string>();
                    foreach (string token in options.StartingTokens)
                    {
                        if (chain.TryGetState(token, out ChainState<string> state) && initialTokens.Add(token))
                        {
                            initialStates.Add(state);
                        }
                    }
                    foreach (string sentence in Program.GenerateSentences(initialTokens, initialStates).Take(options.SentenceCount))
                    {
                        Console.WriteLine(sentence);
                    }
                }
            );
        }

        /// <summary>Generator which parses the files passed to <see cref="Program.Main(string[])"/>.</summary>
        /// <remarks>The file path "-" is translated into stdin.</remarks>
        /// <param name="paths">File paths to be parsed into tokens</param>
        /// <returns>An <see cref="IEnumerable{string}"/> yielding the resulting tokens</returns>
        public static IEnumerable<string> ParseFiles(IEnumerator<string> paths)
        {
            IEnumerator<char> stream;
            IEnumerator<string> tokens;
            while (paths.MoveNext())
            {
                if (paths.Current.Equals("-"))
                {
                    stream = new StreamReader(Console.OpenStandardInput()).ReadToEnd().GetEnumerator();
                }
                else
                {
                    stream = File.ReadAllText(paths.Current).GetEnumerator();
                }
                using(tokens = new Tokenizer(new LineJoiningEnumerator(stream)))
                {
                    while (tokens.MoveNext())
                    {
                        yield return tokens.Current;
                    }
                }
            }
        }

        /// <summary>Generator which wraps an <see cref="IEnumerator{string}"/> and prints each token to <see cref="Console"/></summary>
        /// <param name="enumerator"><see cref="IEnumerator{string}"/> to wrap</param>
        /// <returns>An <see cref="IEnumerator{string}"/> wrapping <paramref name="enumerator"/></returns>
        static IEnumerator<string> WriteTokens(IEnumerator<string> enumerator)
        {
            Console.WriteLine("Tokens {");
            while (enumerator.MoveNext())
            {
                string token = enumerator.Current;
                Console.WriteLine($"\t{token}");
                yield return token;
            }
            Console.WriteLine("}");
        }

        /// <summary>Generate sentences from a <see cref="Chain{string}"/>.</summary>
        /// <remarks>The quality of the sentences depends on the samples provided and the rng.</remarks>
        /// <param name="initialTokens">Tokens which mark the start of a new sentence</param>
        /// <param name="initialStates">States of the tokens in <paramref name="initialTokens"/></param>
        /// <returns>An <see cref="IEnumerable{string}"/> which yields an unlimited amount of sentences</returns>
        public static IEnumerable<string> GenerateSentences(ICollection<string> initialTokens, IList<ChainState<string>> initialStates)
        {
            Random rng = new Random();
            List<string> buffer = new List<string>();

            if (initialStates.Count != 0)
            {
                while (true)
                {
                    using (IEnumerator<string> tokens = initialStates[rng.Next(initialStates.Count)].StartTransitions(rng))
                    {
                        tokens.MoveNext();  // skip inital Token
                        while (tokens.MoveNext())
                        {
                            buffer.Add(tokens.Current);
                            if (initialTokens.Contains(tokens.Current))
                            {
                                break;
                            }
                        }
                    }
                    yield return String.Join(' ', buffer);
                    buffer.Clear();
                }
            }
            else
            {
                throw new ArgumentException($"no starting Token ({String.Join(", ", initialTokens)}) does exist in the chain");
            }
        }
    }
}
