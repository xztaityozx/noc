using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NabeObfuscationCode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var nabe = new Nabe();

            if (args.Length != 0 && (args[0] == "-d" || args[0] == "--decode"))
            {
                var src = args.Length == 1 ? GetStringFromStdIn() : string.Join(" ", args.Skip(1));
                Console.WriteLine(nabe.Decode(src).Trim());
            }
            else
            {
                var src = args.Length == 0 ? GetStringFromStdIn() : string.Join(" ", args);
                Console.WriteLine(nabe.Encode(src).Trim());
            }

        }

        private static string GetStringFromStdIn()
        {
            var sb = new StringBuilder();
            string line;
            Console.InputEncoding = Encoding.UTF8;
            while ((line = Console.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }



    public class Nabe
    {
        public class Block
        {
            public enum Mode
            {
                Small,
                Big,
                Symbol,
                Any
            }

            public static readonly Dictionary<string, Mode> ModeDictionary = new Dictionary<string, Mode> {
                {"辺", Mode.Small},
                {"邉", Mode.Big},
                {"邊", Mode.Symbol},
            };

            public static readonly Dictionary<Mode, string> InverseDictionary = new Dictionary<Mode, string> {
                {Mode.Small, "辺"},
                {Mode.Big, "邉"},
                {Mode.Symbol, "邊"},
                {Mode.Any, " "}
            };

            public static bool IsDelimiter(string str) => new[] { "辺", "邊", "邉" }.Contains(str);
        }

        public readonly Dictionary<Block.Mode, Dictionary<string, string>> ConvertTable =
            new Dictionary<Block.Mode, Dictionary<string, string>> {
                {
                    Block.Mode.Small, new Dictionary<string, string> {
                        {"辺󠄀", "a"},
                        {"辺󠄁", "b"},
                        {"辺󠄂", "c"},
                        {"邉󠄀", "d"},
                        {"邉󠄁", "e"},
                        {"邉󠄂", "f"},
                        {"邉󠄃", "g"},
                        {"邉󠄄", "h"},
                        {"邉󠄅", "i"},
                        {"邉󠄆", "j"},
                        {"邉󠄇", "k"},
                        {"邉󠄈", "l"},
                        {"邉󠄉", "m"},
                        {"邉󠄊", "n"},
                        {"邉󠄋", "o"},
                        {"邉󠄌", "p"},
                        {"邉󠄍", "q"},
                        {"邉󠄎", "r"},
                        {"邉󠄏", "s"},
                        {"邉󠄐", "t"},
                        {"邉󠄑", "u"},
                        {"邉󠄒", "v"},
                        {"邉󠄓", "w"},
                        {"邉󠄔", "x"},
                        {"邉󠄕", "y"},
                        {"邉󠄖", "z"}
                    }
                }, {
                    Block.Mode.Big, new Dictionary<string, string> {
                        {"邉󠄀", "A"},
                        {"邉󠄁", "B"},
                        {"邉󠄂", "C"},
                        {"邉󠄃", "D"},
                        {"邉󠄄", "E"},
                        {"邉󠄅", "F"},
                        {"邉󠄆", "G"},
                        {"邉󠄇", "H"},
                        {"邉󠄈", "I"},
                        {"邉󠄉", "J"},
                        {"邉󠄊", "K"},
                        {"邉󠄋", "L"},
                        {"邉󠄌", "M"},
                        {"邉󠄍", "N"},
                        {"邉󠄎", "O"},
                        {"邉󠄏", "P"},
                        {"邉󠄐", "Q"},
                        {"邉󠄑", "R"},
                        {"邉󠄒", "S"},
                        {"邉󠄓", "T"},
                        {"邉󠄔", "U"},
                        {"邉󠄕", "V"},
                        {"邉󠄖", "W"},
                        {"邉󠄗", "X"},
                        {"邉󠄘", "Y"},
                        {"邉󠄙", "Z"}
                    }
                }, {
                    Block.Mode.Symbol, new Dictionary<string, string> {
                        {"邊󠄀", "!"},
                        {"邊󠄁", "\""},
                        {"邊󠄂", "#"},
                        {"邊󠄃", "$"},
                        {"邊󠄄", "%"},
                        {"邊󠄅", "&"},
                        {"邊󠄆", "'"},
                        {"邊󠄇", "("},
                        {"邊󠄈", ")"},
                        {"邊󠄉", "*"},
                        {"邊󠄊", "+"},
                        {"邊󠄋", ","},
                        {"邊󠄌", "-"},
                        {"邊󠄍", "."},
                        {"邊󠄎", "/"},
                        {"邊󠄏", "0"},
                        {"邊󠄐", "1"},
                        {"邊󠄑", "2"},
                        {"邊󠄒", "3"},
                        {"邊󠄓", "4"},
                        {"邊󠄔", "5"},
                        {"辺󠄀", "6"},
                        {"辺󠄁", "7"},
                        {"辺󠄂", "8"},
                        {"邉󠄀", "9"},
                        {"邉󠄁", ":"},
                        {"邉󠄂", ";"},
                        {"邉󠄃", "<"},
                        {"邉󠄄", "="},
                        {"邉󠄅", ">"},
                        {"邉󠄆", "?"},
                        {"邉󠄇", "@"},
                        {"邉󠄈", "["},
                        {"邉󠄉", "\\"},
                        {"邉󠄊", "]"},
                        {"邉󠄋", "^"},
                        {"邉󠄌", "_"},
                        {"邉󠄍", "`"},
                        {"邉󠄎", "{"},
                        {"邉󠄏", "|"},
                        {"邉󠄐", "}"},
                        {"邉󠄑", "~"},
                        {"邉󠄒", " "},
                    }
                }
            };

        private readonly string[] targets = {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
            "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
            "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+",
            ",", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?", "@",
            "[", "\\", "]", "^", "_", "`", "{", "|", "}", "~", " "
        };

        public string Decode(string str)
        {
            var rt = "";
            var currentMode = Block.Mode.Any;
            for (var item = StringInfo.GetTextElementEnumerator(str); item.MoveNext();)
            {

                if (Block.IsDelimiter(item.GetTextElement()))
                {
                    currentMode = Block.ModeDictionary[item.GetTextElement()];
                }
                else
                {
                    if (currentMode == Block.Mode.Any) rt += item.GetTextElement();
                    else if (ConvertTable[currentMode].ContainsKey(item.GetTextElement())) rt += ConvertTable[currentMode][item.GetTextElement()];
                    else rt += item.GetTextElement();
                }
            }

            return rt;
        }

        public string Encode(string str)
        {
            var rt = "";

            var currentMode = Block.Mode.Any;

            var inverseTable = new Dictionary<Block.Mode, Dictionary<string, string>>();
            foreach (var table in ConvertTable)
            {
                var key = table.Key;
                var invCollection = new Dictionary<string, string>();
                foreach (var item in table.Value)
                {
                    invCollection.Add(item.Value, item.Key);
                }

                inverseTable.Add(key, invCollection);
            }

            foreach (var item in str.Select(x => x.ToString()))
            {
                if (targets.Contains(item))
                {
                    var idx = 0;
                    while (item == targets[idx]) idx++;
                    var nextMode = idx <= 25 ? Block.Mode.Small : idx <= 51 ? Block.Mode.Big : Block.Mode.Symbol;

                    if (nextMode != currentMode) rt += Block.InverseDictionary[nextMode];
                    currentMode = nextMode;

                    rt += inverseTable[currentMode][item];
                }
                else
                {
                    rt += item;
                }
            }

            return rt;
        }

    }

}
