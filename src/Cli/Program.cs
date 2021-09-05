using System;
using System.IO;
using System.Collections.Generic;
using CommandLine;

namespace MarkovChain.Cli
{
    class Program
    {
        public class Options
        {
            [Option('n', "sentences", Default = 1, HelpText = "How many sentences should be produced, n < 1 for unlimited.")]
            public int SentenceCount { get; set; }

            [Option('s', "start", Default = ".", HelpText = "Token which indicates the start of a new sentence.")]
            public string StartingToken { get; set; }

            [Option('v', "verbose", Default = false, HelpText = "Show tokens while parsing.")]
            public bool Verbose { get; set; }

            [Value(0, Default = new string[]{"-"}, HelpText = "Files to read sample text from, - represents stdin.")]
            public IEnumerable<string> SampleFiles { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options =>
                {
                    IEnumerator<char> enumerator;
                    IEnumerator<string> tokens;
                    Chain<string> chain = new Chain<string>();

                    foreach (string path in options.SampleFiles)
                    {
                        if (path.Equals("-"))
                        {
                            enumerator = new StreamReader(Console.OpenStandardInput()).ReadToEnd().GetEnumerator();
                        }
                        else
                        {
                            enumerator = File.ReadAllText(path).GetEnumerator();
                        }
                        using(tokens = new Tokenizer(new LineJoiningEnumerator(enumerator)))
                        {
                            if (options.Verbose)
                            {
                                tokens = Program.WriteTokens(tokens);
                            }
                            chain.AddTransitions(tokens);
                        }
                    }

                    Program.WriteSentences(chain, options);
                }
            );
        }

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

        static void WriteSentences(Chain<string> chain, Options options)
        {
            Random rng = new Random();
            List<string> buffer = new List<string>();
            IEnumerator<string> tokens;
            if (chain.TryGetState(options.StartingToken, out ChainState<string> initialState))
            {

                for (int i = 0; i < options.SentenceCount; i++)
                {
                    using (tokens = initialState.StartTransitions(rng))
                    {
                        tokens.MoveNext();  // skip inital Token
                        while (tokens.MoveNext())
                        {
                            buffer.Add(tokens.Current);
                            if (tokens.Current.Equals(options.StartingToken))
                            {
                                break;
                            }
                        }

                        Console.WriteLine(String.Join(' ', buffer));
                        buffer.Clear();
                    }
                }
            }
        }
    }
}
