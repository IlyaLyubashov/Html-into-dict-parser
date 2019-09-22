using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace WebPageParsing
{
    /// <summary>
    /// Реализует интерфейс IComparer для сравнения объектов Match по индексу
    /// </summary>
    class MatchComparerByIndex : IComparer<Match>
    {

        public int Compare(Match x, Match y)
        {
            if (x.Index > y.Index)
                return 1;
            else if (x.Index < y.Index)
                return -1;
            else
                return 0;
        }
    }
}
