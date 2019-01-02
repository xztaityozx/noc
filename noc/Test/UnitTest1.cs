using System;
using System.Linq;
using noc;
using Xunit;
using Xunit.Sdk;

namespace Test {
    public class UnitTest1 {
        [Fact]
        public void GetModeSmall() {
            var nabe = new Nabe();

            for (var c = 'a'; c <= 'z'; c++) {
                Assert.Equal(Nabe.Block.Mode.Small, nabe.GetMode($"{c}"));
            }
        }

        [Fact]
        public void GetModeBig() {
            var nabe = new Nabe();

            for (var c = 'A'; c <= 'Z'; c++) {
                Assert.Equal(Nabe.Block.Mode.Big, nabe.GetMode($"{c}"));
            }
        }

        [Fact]
        public void GetModeSymbol() {
            var nabe = new Nabe();

            foreach (var s in new[] {
                "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+",
                ",", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?", "@",
                "[", "\\", "]", "^", "_", "`", "{", "|", "}", "~", " "
            }) {

                Assert.Equal(Nabe.Block.Mode.Symbol, nabe.GetMode(s));
            }
        }

        [Fact]
        public void GetModeBase64() {
            var nabe = new Nabe();

            for (var c = 'あ'; c <= 'ん'; c++) {
                Assert.Equal(Nabe.Block.Mode.Base64, nabe.GetMode($"{c}"));
            }
        }
    }

    public class UnitTest2 {
        [Fact]
        public void Encode001() {
            const string src = "abc";
            Assert.Equal("辺辺󠄀辺󠄁辺󠄂", new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode002() {
            const string src = "ABC";
            Assert.Equal("邉邉󠄀邉󠄁邉󠄂", new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode003() {
            const string src = "123";
            Assert.Equal("邊邊󠄐邊󠄑邊󠄒", new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode004() {
            const string src = "あいう";
            Assert.Equal("渡邊邊󠄓邊󠄓邉邉󠄆邉󠄂邊邊󠄓邊󠄓邉邉󠄆邉󠄄邊邊󠄓邊󠄓邉邉󠄆邉󠄆渡", new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode005() {
            var src = string.Join("", Enumerable.Range(0, 26).Select(x => (char) ('a' + x)));
            Assert.Equal("辺辺󠄀辺󠄁辺󠄂邉󠄀邉󠄁邉󠄂邉󠄃邉󠄄邉󠄅邉󠄆邉󠄇邉󠄈邉󠄉邉󠄊邉󠄋邉󠄌邉󠄍邉󠄎邉󠄏邉󠄐邉󠄑邉󠄒邉󠄓邉󠄔邉󠄕邉󠄖",
                new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode006() {
            var src = string.Join("", Enumerable.Range(0, 26).Select(x => (char) ('A' + x)));
            Assert.Equal("邉邉󠄀邉󠄁邉󠄂邉󠄃邉󠄄邉󠄅邉󠄆邉󠄇邉󠄈邉󠄉邉󠄊邉󠄋邉󠄌邉󠄍邉󠄎邉󠄏邉󠄐邉󠄑邉󠄒邉󠄓邉󠄔邉󠄕邉󠄖邉󠄗邉󠄘邉󠄙",
                new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode007() {
            const string src = "0123456789";
            Assert.Equal("邊邊󠄏邊󠄐邊󠄑邊󠄒邊󠄓邊󠄔辺󠄀辺󠄁辺󠄂邉󠄀", new NabeEncoder().Encode(src));
        }

        [Fact]
        public void Encode008() {
            const string src = "こんにちは!これはnocコマンドのTestですよ！.";
            Assert.Equal(
                "渡邊邊󠄓邊󠄓邉邉󠄆邉󠄓邊邊󠄓邊󠄓邉邉󠄊邉󠄓邊邊󠄓邊󠄓邉邉󠄆辺邉󠄎邊邊󠄓邊󠄓邉邉󠄆辺邉󠄄邊邊󠄓邊󠄓邉邉󠄆辺邉󠄒渡邊邊󠄀渡邊邊󠄓邊󠄓邉邉󠄆邉󠄓邊邊󠄓邊󠄓邉邉󠄊邉󠄌邊邊󠄓邊󠄓邉邉󠄆辺邉󠄒渡辺邉󠄊邉󠄋辺󠄂渡邊邊󠄓邊󠄓邉邉󠄊辺邉󠄖邊邊󠄓邊󠄓邉邉󠄎辺邉󠄁邊邊󠄓邊󠄓邉邉󠄎辺邉󠄖邊邊󠄓邊󠄓邉邉󠄎邉󠄉邊邊󠄓邊󠄓邉邉󠄆辺邉󠄑渡邉邉󠄓辺邉󠄁邉󠄏邉󠄐渡邊邊󠄓邊󠄓邉邉󠄆辺邉󠄊邊邊󠄓邊󠄓邉邉󠄆邉󠄙邊邊󠄓邊󠄓邉邉󠄊邉󠄈邊辺󠄁辺󠄁辺邉󠄕邉邉󠄁渡邊邊󠄍",
                new NabeEncoder().Encode(src));

        }

        [Fact]
        public void Encode009() {
            const string src = "abcABC123あいう";
            Assert.Equal("辺辺󠄀辺󠄁辺󠄂邉邉󠄀邉󠄁邉󠄂邊邊󠄐邊󠄑邊󠄒渡邊邊󠄓邊󠄓邉邉󠄆邉󠄂邊邊󠄓邊󠄓邉邉󠄆邉󠄄邊邊󠄓邊󠄓邉邉󠄆邉󠄆渡",
                new NabeEncoder().Encode(src));
        }
    }

    public class UnitTest3 {
        [Fact]
        public void Decode001() {
            const string src = "辺辺󠄀辺󠄁辺󠄂";
            Assert.Equal("abc", new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode002() {
            const string src = "邉邉󠄀邉󠄁邉󠄂";
            Assert.Equal("ABC", new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode003() {
            const string src = "邊邊󠄐邊󠄑邊󠄒";
            Assert.Equal("123", new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode004() {
            const string src = "渡邊邊󠄓邊󠄓邉邉󠄆邉󠄂邊邊󠄓邊󠄓邉邉󠄆邉󠄄邊邊󠄓邊󠄓邉邉󠄆邉󠄆渡";
            Assert.Equal("あいう", new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode005() {
            const string src = "辺辺󠄀辺󠄁辺󠄂邉󠄀邉󠄁邉󠄂邉󠄃邉󠄄邉󠄅邉󠄆邉󠄇邉󠄈邉󠄉邉󠄊邉󠄋邉󠄌邉󠄍邉󠄎邉󠄏邉󠄐邉󠄑邉󠄒邉󠄓邉󠄔邉󠄕邉󠄖";
            Assert.True(Enumerable.Range(0, 26).Select(x => (char) ('a' + x))
                .Zip(new NabeDecoder().Decode(src), (a, b) => a == b).All(x => x));
        }

        [Fact]
        public void Decode006() {
            const string src = "邉邉󠄀邉󠄁邉󠄂邉󠄃邉󠄄邉󠄅邉󠄆邉󠄇邉󠄈邉󠄉邉󠄊邉󠄋邉󠄌邉󠄍邉󠄎邉󠄏邉󠄐邉󠄑邉󠄒邉󠄓邉󠄔邉󠄕邉󠄖邉󠄗邉󠄘邉󠄙";
            Assert.True(Enumerable.Range(0, 26).Select(x => (char)('A' + x))
                .Zip(new NabeDecoder().Decode(src), (a, b) => a == b).All(x => x));
        }

        [Fact]
        public void Decode007() {
            const string src = "邊邊󠄏邊󠄐邊󠄑邊󠄒邊󠄓邊󠄔辺󠄀辺󠄁辺󠄂邉󠄀";
            Assert.Equal("0123456789", new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode008() {
            const string src = "渡邊邊󠄓邊󠄓邉邉󠄆邉󠄓邊邊󠄓邊󠄓邉邉󠄊邉󠄓邊邊󠄓邊󠄓邉邉󠄆辺邉󠄎邊邊󠄓邊󠄓邉邉󠄆辺邉󠄄邊邊󠄓邊󠄓邉邉󠄆辺邉󠄒渡邊邊󠄀渡邊邊󠄓邊󠄓邉邉󠄆邉󠄓邊邊󠄓邊󠄓邉邉󠄊邉󠄌邊邊󠄓邊󠄓邉邉󠄆辺邉󠄒渡辺邉󠄊邉󠄋辺󠄂渡邊邊󠄓邊󠄓邉邉󠄊辺邉󠄖邊邊󠄓邊󠄓邉邉󠄎辺邉󠄁邊邊󠄓邊󠄓邉邉󠄎辺邉󠄖邊邊󠄓邊󠄓邉邉󠄎邉󠄉邊邊󠄓邊󠄓邉邉󠄆辺邉󠄑渡邉邉󠄓辺邉󠄁邉󠄏邉󠄐渡邊邊󠄓邊󠄓邉邉󠄆辺邉󠄊邊邊󠄓邊󠄓邉邉󠄆邉󠄙邊邊󠄓邊󠄓邉邉󠄊邉󠄈邊辺󠄁辺󠄁辺邉󠄕邉邉󠄁渡邊邊󠄍";
            Assert.Equal(
                "こんにちは!これはnocコマンドのTestですよ！.",
                new NabeDecoder().Decode(src));
        }

        [Fact]
        public void Decode009() {
            const string src = "辺辺󠄀辺󠄁辺󠄂邉邉󠄀邉󠄁邉󠄂邊邊󠄐邊󠄑邊󠄒渡邊邊󠄓邊󠄓邉邉󠄆邉󠄂邊邊󠄓邊󠄓邉邉󠄆邉󠄄邊邊󠄓邊󠄓邉邉󠄆邉󠄆渡";
            Assert.Equal("abcABC123あいう", new NabeDecoder().Decode(src));
        }
    }
}
