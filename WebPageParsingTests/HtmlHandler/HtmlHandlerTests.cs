using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace WebPageParsing.Tests
{
    [TestClass()]
    public class HtmlHandlerTests
    {

        public void HandleTest(string str,string[] arr)
        {
            var handler = new HtmlHandler();
            handler.HandleFragment(str);
            var handled = handler.GetCompeleteContent();
            Assert.IsTrue(handled.SequenceEqual(arr));
            //AreArraysEqual(handled, arr);
        }


        [TestMethod()]
        public void HandleOneClosed()
        {
            HandleTest("<div>Hello</div>",new string[]{"<div>Hello</div>" });
        }


        [TestMethod()]
        public void HandleTwoClosed()
        {
            HandleTest("<div>1</div><div>2</div>", new string[] { "<div>1</div>",
            "<div>2</div>"});
        }


        [TestMethod()]
        public void HandleOneNotClosed()
        {
            HandleTest("<div>", new string[] { });
        }


        [TestMethod()]
        public void HandleInsideClosedOuterClosed()
        {
            HandleTest("<div>1<div>2</div></div>", new string[] { "<div>2</div>", "<div>1</div>" });
        }


        [TestMethod()]
        public void HandleInsideClosedOuterNotClosed()
        {
            HandleTest("<div>1<div>2</div>", new string[] { "<div>2</div>"});
        }


        [TestMethod()]
        public void HandleTwoNotClosed()
        {
            HandleTest("<div>1<div>2", new string[] {  });
        }

    }
}