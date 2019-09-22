using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebPageParsing
{
    //Публичный для тестов
    /// <summary>
    /// Обрабатывает HTML строки, отдает содержимое, как только закрывается открытый тег.
    /// </summary>
    public class HtmlHandler
    {
        Stack<string> tagStack = new Stack<string>();
        Stack<string> tagContentStack = new Stack<string>();
        static string[] tags = new[] { "a", "abbr", "acronym", "address", "applet", "article", "aside", "audio", "b", "basefont", "bdi", "bdo", "big", "blockquote", "body", "button", "canvas", "caption", "center", "cite", "code", "colgroup", "colgroup", "data", "datalist", "dd", "del", "details", "dfn", "dialog", "dir", "div", "dl", "dt", "em", "fieldset", "figcaption", "figure", "figure", "font", "footer", "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", "html", "i", "iframe", "ins", "kbd", "label", "legend", "fieldset", "li", "main", "map", "mark", "meter", "nav", "noframes", "noscript", "object", "ol", "optgroup", "option", "output", "p", "picture", "pre", "progress", "q", "rp", "rt", "ruby", "s", "samp", "script", "section", "select", "small", "span", "strike", "strong", "style", "sub", "summary", "sup", "svg", "table", "tbody", "td", "template", "textarea", "tfoot", "th", "thead", "time", "title", "tr", "tt", "u", "ul", "var", "video" };
        List<string> completeContent = new List<string>();


        List<Match> TagMatchesInFrag(string htmlFragment, out List<int> breakPoints)
        {
            breakPoints = new List<int>();
            //breakPoints.Add(0);
            List<Match> matchList = new List<Match>();
            foreach (var tag in tags)
            {
                Regex startTag = new Regex($@"<{tag}[>| ]");
                Regex endTag = new Regex($@"</{tag}>");
                foreach (Match reg in startTag.Matches(htmlFragment))
                {
                    breakPoints.Add(reg.Index);
                    matchList.Add(reg);
                }
                foreach (Match reg in endTag.Matches(htmlFragment))
                {
                    breakPoints.Add(reg.Index + reg.Length);
                    matchList.Add(reg);
                }
            }
            breakPoints.Add(htmlFragment.Length);
            breakPoints.Sort();
            return matchList;
        }

        void ZeroMatchesCondition(List<Match> matches, string htmlFrag)
        {
            if (matches.Count == 0 && tagContentStack.Count != 0)
            {
                string curTopStr = tagContentStack.Peek();
                curTopStr += htmlFrag;
            }
        }


        void StartEndTagMatchesHandling(List<Match> matches,string htmlFragment )
        {
            int curBrakeInd = 0;
            int indexOfCurMatch = 0;
            foreach (var match in matches)
            {
                var matchText = match.Value;
                var peekText = tagStack.Count != 0 ? tagStack.Peek() : "";
                var pureTagForEnd = matchText.Substring(2, matchText.Length - 3);
                var pureTagForStart = tagStack.Count != 0 ? peekText.Substring(1, peekText.Length - 2) : "";
                if (matchText[1] != '/')
                {
                    if (tagContentStack.Count != 0)
                    {
                        var topStackStr = tagContentStack.Peek();
                        topStackStr += htmlFragment.Substring(curBrakeInd, match.Index - curBrakeInd);
                        curBrakeInd = match.Index;
                    }
                    tagStack.Push(matchText);
                    if (indexOfCurMatch == matches.Count - 1)
                        tagContentStack.Push(htmlFragment.Substring(curBrakeInd));
                    else
                    {
                        tagContentStack.Push(htmlFragment.Substring(curBrakeInd, matches[indexOfCurMatch + 1].Index - curBrakeInd));
                        curBrakeInd = matches[indexOfCurMatch + 1].Index;
                    }
                }
                else if (tagStack.Count != 0 && pureTagForEnd == pureTagForStart)
                {
                    tagStack.Pop();
                    var firstPart = tagContentStack.Pop();
                    int closeTagEnd = match.Index + match.Length;
                    var secPart = htmlFragment.Substring(curBrakeInd, closeTagEnd - curBrakeInd);
                    completeContent.Add(firstPart + secPart);
                    curBrakeInd = closeTagEnd;
                }
                indexOfCurMatch++;
            }
            if (tagContentStack.Count != 0)
            {
                string curTopStr = tagContentStack.Peek();
                if (curBrakeInd != htmlFragment.Length)
                    curTopStr += htmlFragment.Substring(curBrakeInd);
            }
        }

        
        /// <summary>
        /// Обработка входящих строк и помещение готовых в специальный массив.
        /// </summary>
        /// <param name="htmlFragment">HTML строка для обработки.</param>
        public void HandleFragment(string htmlFragment)
        {
            List<Match> matches = TagMatchesInFrag(htmlFragment, out List<int> breaks);
            matches.Sort(new MatchComparerByIndex());            
            ZeroMatchesCondition(matches, htmlFragment);
            StartEndTagMatchesHandling(matches,htmlFragment);            
        }



        /// <summary>
        /// Возвращает массив готовых к обработке строк-тегов.
        /// </summary>
        /// <returns>Строки, готовые к обработке парсером.</returns>
        public string[] GetCompeleteContent()
        {
            string[] cont = new string[completeContent.Count];
            completeContent.CopyTo(cont);
            completeContent.Clear();
            return cont;
        }
    }
}
