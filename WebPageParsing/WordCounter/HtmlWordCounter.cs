using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace WebPageParsing
{
    /// <summary>
    /// Позволяет получить словарь встречаемости слов на HTML странице.
    /// </summary>
    class HtmlWordCounter: WordCounter
    {
        IHtmlDocument html;
        string[] ignored_elements = new string[] { "SCRIPT", "STYLE" };

        public HtmlWordCounter(IHtmlDocument html)
        {
            this.html = html;
        }


        public HtmlWordCounter() { }

        bool ProperParent(INode parent)
        {
            foreach (var elName in ignored_elements)
            {
                if (elName == parent.NodeName)
                {
                    return false;
                }
            }
            return true;
        }
        
        void DFS(INode node)
        {
            foreach (var child in node.ChildNodes)
            {
                if(child.HasChildNodes)
                    DFS(child);
                else if (child.NodeType == NodeType.Text && ProperParent(child.Parent))
                {
                    var textContent = child.TextContent.Trim();
                    if (textContent != "")
                    {
                        AppendWordsToCounter(textContent);
                    }
                }
            }
        }


        /// <summary>
        /// Обход всех вершин графа узлов переданного HTML элемента и его потомков, взятие текста из текстовых узлов нужных HTML элементов.
        /// </summary>
        /// <param name="node">Родительский узел, с которого начинается обход</param>
        public void ProcessHtml(INode node)
        {
            DFS(node);               
        }

        /// <summary>
        /// Обход всех вершин графа узлов переданного в конструктор класса HTML элемента и его потомков.
        /// </summary>
        public void ProcessHtml()
        {
           
            if (html == null) throw new ArgumentNullException("There is no html in counter!");
            DFS(html);
        }
    }
}
