using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace noc {
    internal class Program {
        private static void Main(string[] args) {

            if (args.Length != 0 && (args[0] == "-v" || args[0] == "--version")) {
                Console.WriteLine(
                    "noc - Nave Obfuscation Converter v1.3\nAuthor: xztaityozx\nRepo: https://github.com/xztaityozx/noc");
                return;
            }


            if (args.Length != 0 && (args[0] == "-h" || args[0] == "--help")) {
                Console.WriteLine(
                    "Usage: noc [option] [string]\n[string]をワタナベ難読化シェル芸を使って難読化します\n[string]を指定しない場合，StdInから読み取られます");
                Console.WriteLine("[option]\n  -d --decode\t デコードします");
                Console.WriteLine("  -h --help\t ヘルプを出力して終了します");
                Console.WriteLine("  -v --version\t バージョン情報を出力して終了します");
                return;
            }



            if (args.Length != 0 && (args[0] == "-d" || args[0] == "--decode")) {
                var decoder = new NabeDecoder();

                if (args.Length == 1) {
                    string line;
                    while ((line = Console.ReadLine()) != null) {
                        Console.WriteLine(decoder.Decode(line.Trim()));
                    }
                }
                else {
                    Console.WriteLine(decoder.Decode(string.Join(" ", args.Skip(1))));
                }
            }
            else {
                var encoder = new NabeEncoder();
                if (args.Length == 0) {
                    string line;
                    while ((line = Console.ReadLine()) != null) {
                        Console.WriteLine(encoder.Encode(line.Trim()));
                    }
                }
                else {
                    Console.WriteLine(encoder.Encode(string.Join(" ", args)));
                }
            }

        }
    }

    public class NabeEncoder : Nabe {


        private List<Block> EncodeSplit(string str) {
            var rt = new List<Block>();

            var currentBlock = new Block();
            for (var item = StringInfo.GetTextElementEnumerator(str); item.MoveNext();) {
                var nextMode = GetMode(item.GetTextElement());
                if (currentBlock.BlockMode == Block.Mode.Any) {
                    currentBlock.BlockMode = nextMode;
                    currentBlock.Message += item.GetTextElement();
                }
                else if (currentBlock.BlockMode == nextMode) {
                    currentBlock.Message += item.GetTextElement();
                }
                else {
                    rt.Add(currentBlock);
                    currentBlock = new Block {BlockMode = nextMode, Message = item.GetTextElement()};
                }
            }

            rt.Add(currentBlock);

            return rt;
        }

        private Dictionary<Block.Mode, Dictionary<string, string>> inverseTable;

        private void BuildInverseTable() {
            inverseTable = new Dictionary<Block.Mode, Dictionary<string, string>>();
            foreach (var table in ConvertTable) {
                var key = table.Key;
                var invCollection = new Dictionary<string, string>();
                foreach (var item in table.Value) {
                    invCollection.Add(item.Value, item.Key);
                }

                inverseTable.Add(key, invCollection);
            }
        }


        public string Encode(string str) {

            BuildInverseTable();

            var list = new List<Block>();

            Func<Block, string> Filter = block =>  {
                var filtered = Block.InverseDictionary[block.BlockMode];
                for (var item = StringInfo.GetTextElementEnumerator(block.Message); item.MoveNext();) {
                    filtered += inverseTable[block.BlockMode][item.GetTextElement()];
                }

                return filtered;
            };

            foreach (var b in EncodeSplit(str)) {
                if (b.BlockMode == Block.Mode.Base64) {
                    var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(b.Message));
                    list.Add(new Block {BlockMode = Block.Mode.Base64, Message = ""});
                    list.AddRange(EncodeSplit(b64));
                    list.Add(new Block {BlockMode = Block.Mode.Base64, Message = ""});
                }
                else {
                    list.Add(b);
                }
            }

            //list.ForEach(Console.Error.WriteLine);

            return string.Join("", list.Select(Filter));
        }
    }

    public class NabeDecoder : Nabe {

        private List<Block> DecodeSplit(string str) {
            var rt = new List<Block>();

            var block = new Block {BlockMode = Block.Mode.Any, Message = ""};

            for (var item = StringInfo.GetTextElementEnumerator(str); item.MoveNext();) {
                var element = item.GetTextElement();
                if (block.BlockMode == Block.Mode.Any || Block.IsDelimiter(element)) {
                    if (block.BlockMode != Block.Mode.Any) rt.Add(block);
                    block = new Block {
                        BlockMode = element == Block.InverseDictionary[Block.Mode.Base64] ? Block.Mode.Base64 :
                            element == Block.InverseDictionary[Block.Mode.Big] ? Block.Mode.Big :
                            element == Block.InverseDictionary[Block.Mode.Small] ? Block.Mode.Small :
                            Block.Mode.Symbol
                    };
                }
                else {
                    block.Message += element;
                }
            }

            rt.Add(block);

            return rt;
        }

        private string Filter(Block block) {
            var rt = "";

            if(block.BlockMode == Block.Mode.Base64) return "";

            for (var item = StringInfo.GetTextElementEnumerator(block.Message); item.MoveNext();) {
                rt += ConvertTable[block.BlockMode][item.GetTextElement()];
            }

            return rt;
        }

        public string Decode(string str) {
            var rt = "";

            var src = DecodeSplit(str);


            var idx = 0;
            for (; idx < src.Count;) {
                var block = src[idx];
                if (block.BlockMode == Block.Mode.Base64) {
                    idx++;
                    var b64 = Filter(src[idx]);
                    while (src[idx++].BlockMode != Block.Mode.Base64) {
                        b64 += Filter(src[idx]);
                    }

                    rt += Encoding.UTF8.GetString(Convert.FromBase64String(b64));
                    idx--;
                }
                else {
                    rt += Filter(block);
                }

                idx++;
            }

            return rt;
        }

    }

    public class Nabe {
        public class Block {

            public Block() {
                BlockMode = Mode.Any;
                Message = "";
            }

            protected bool Equals(Block other) {
                return string.Equals(Message, other.Message) && BlockMode == other.BlockMode;
            }

            public override string ToString() {
                return $"Mode: {BlockMode}, Message: {Message}";
            }

            public enum Mode {
                Small,
                Big,
                Symbol,
                Base64,
                Any
            }


            public static readonly Dictionary<Mode, string> InverseDictionary = new Dictionary<Mode, string> {
                {Mode.Small, "辺"},
                {Mode.Big, "邉"},
                {Mode.Symbol, "邊"},
                {Mode.Base64, "部"},
            };

            public static bool IsDelimiter(string str) => new[] {"辺", "邊", "邉", "部"}.Contains(str);
            public string Message { get; set; }
            public Mode BlockMode { get; set; }
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


        public Block.Mode GetMode(string str) {
            var idx = 0;
            while (idx<targets.Length && targets[idx] != str) if(idx<targets.Length) idx++;
                else {
                    idx = -1;
                    break;
                }
            return 0 <= idx && idx <= 25 ? Block.Mode.Small :
                26 <= idx && idx <= 51 ? Block.Mode.Big :
                52 <= idx && idx < targets.Length ? Block.Mode.Symbol : Block.Mode.Base64;
        }


    }

}
