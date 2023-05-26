namespace Infor.HammPdfReading.UnitTest
{
    [TestClass]
    public class ConvertingExpressionTest
    {
        string _text;
        
        [TestMethod]
        public void TestMethod1()
        {
            var context = new Context<Detail>
            {
                Text = _text,
                Index = 0
            };

            var expression = new MainExpression();
            expression.Watch(context);
            expression.Write(context);

            Assert.AreEqual(
                new Detail()
                {
                    Item = 1,
                    PartNo = 860670,
                    ValidFor = (1, 9999),
                    Quantity = 1,
                    Unit = Unit.PC
                },
                context.Result);
        }

        ConvertingExpressionTest()
        {
            using (StreamReader reader = new StreamReader(new Uri("res/row.txt").AbsolutePath))
                _text = reader.ReadToEnd();
        }
    }
}