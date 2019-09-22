using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebPageParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPageParsing.Tests
{
    [TestClass()]
    class WordCounterTests
    {
        void CountWordsTest(string str, Dictionary<string, int> d1)
        {
            var countedWords = WordCounter.CountWords(str);
            Assert.IsTrue(countedWords.OrderBy(i=> i.Value)
                .SequenceEqual(d1.OrderBy(i=>i.Value)));
        }


        [TestMethod()]
        public void SimpleTest()
        {
            var dict = new Dictionary<string, int> ();
            dict.Add("privet",1);
            CountWordsTest("Privet", dict);
        }


        [TestMethod()]
        public void SeparatorSpam()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("1", 1);
            dict.Add("2", 1);
            dict.Add("3", 1);
            dict.Add("6", 1);
            CountWordsTest("1.2..3....6", dict);
        }


        [TestMethod()]
        public void TwoSameWords()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("privet", 2);
            CountWordsTest("Privet.privet", dict);
        }
    }
}