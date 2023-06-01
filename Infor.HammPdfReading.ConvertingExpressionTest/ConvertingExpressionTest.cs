namespace Infor.HammPdfReading.UnitTest
{
    [TestClass]
    public class ConvertingExpressionTest
    {
        string GetRowText() => Properties.Resources.row;
        string GetRowsText() => Properties.Resources.rows;
        string GetDesignationText() => Properties.Resources.designation;

        [TestMethod]
        public void VerticalMethod()
        {
            var text = GetRowText();

            var context = new Context<Detail>
            {
                Text = text,
                Index = 0
            };

            var expression = new MainExpression();
            expression.Interpret(context);

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

        [TestMethod]
        public void ConvertingMethod()
        {
            var text = GetRowText();

            var context = new Context<Detail>
            {
                Text = text,
                Index = 0
            };

            var expression = new TestConvertingExpression();
            expression.Interpret(context);

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

        [TestMethod]
        public void RowsMethod()
        {
            var text = GetRowsText();

            var context = new Context<List<Detail>>
            {
                Text = text,
                Index = 0,
                Result = new List<Detail>()
            };

            var expression = new MainListVerticalExpression();
            expression.Interpret(context);

            var expected =
                new Detail[]
                {
                    new Detail()
                    {
                        Item = 1,
                        PartNo = 860670,
                        ValidFor = (1, 9999),
                        Quantity = 1,
                        Unit = Unit.PC
                    },
                    new Detail()
                    {
                        Item = 2,
                        PartNo = 1210106,
                        ValidFor = (1, 9999),
                        Quantity = 4,
                        Unit = Unit.PC
                    }
                };

            for (int i = 0; i < 2; i++)
                Assert.AreEqual(expected[i], context.Result[i]);
        }

        [TestMethod]
        public void HorizontalMethod()
        {
            var text = GetRowsText();

            var context = new Context<List<Detail>>
            {
                Text = text,
                Index = 0,
                Result = new List<Detail>()
            };

            var expression = new MainListHorizontalExpression();
            expression.Interpret(context);

            var expected =
                new Detail[]
                {
                    new Detail()
                    {
                        Item = 1,
                        PartNo = 860670,
                        ValidFor = (1, 9999),
                        Quantity = 1,
                        Unit = Unit.PC
                    },
                    new Detail()
                    {
                        Item = 2,
                        PartNo = 1210106,
                        ValidFor = (1, 9999),
                        Quantity = 4,
                        Unit = Unit.PC
                    }
                };

            for (int i = 0; i < 2; i++)
                Assert.AreEqual(expected[i], context.Result[i]);
        }

        [TestMethod]
        public void DesignationMethod()
        {
            var text = GetDesignationText();

            var context = new Context<Designations>
            {
                Text = text,
                Index = 0,
                Result = new Designations()
            };

            var expression = new DesignationBodyExpression();
            expression.Interpret(context);

            Assert.AreEqual(" –€ÿ¿", context.Result.DesignationRussian);
        }

        [TestMethod]
        public void DesignationsMethod()
        {
            var text = GetRowsText();

            var context = new Context<List<Designations>>
            {
                Text = text,
                Index = 0,
                Result = new List<Designations>()
            };

            var expression = new DesignationListExpression();
            expression.Interpret(context);

            var expected =
                new string[]
                {
                    " –€ÿ¿",
                    "œŒƒ À¿ƒÕ¿ﬂ ÿ¿…¡¿"
                };

            for (int i = 0; i < 2; i++)
            {
                context.Result[i].DesignationRussian = context.Result[i].DesignationRussian.Trim();
                Assert.AreEqual(expected[i], context.Result[i].DesignationRussian);
            }
        }

        [TestMethod]
        public void DetailsMethod()
        {
            var text = GetRowsText();

            var context = new Context<List<Detail>>
            {
                Text = text,
                Index = 0,
                Result = new List<Detail>()
            };

            var expression = new DetailTableExprssion();
            expression.Interpret(context);

            var expected =
                new Detail[]
                {
                    new Detail()
                    {
                        Item = 1,
                        PartNo = 860670,
                        ValidFor = (1, 9999),
                        Quantity = 1,
                        Unit = Unit.PC,
                        Designation = " –€ÿ¿"
                    },
                    new Detail()
                    {
                        Item = 2,
                        PartNo = 1210106,
                        ValidFor = (1, 9999),
                        Quantity = 4,
                        Unit = Unit.PC,
                        Designation = "œŒƒ À¿ƒÕ¿ﬂ ÿ¿…¡¿"
                    }
                };

            var trimmedResult = new List<Detail>();
            foreach (var detail in context.Result)
                trimmedResult.Add(new Detail()
                {
                    Item = detail.Item,
                    PartNo = detail.PartNo,
                    ValidFor = detail.ValidFor,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Designation = detail.Designation.Trim()
                });

            for (int i = 0; i < 2; i++)
                Assert.AreEqual(expected[i], trimmedResult[i]);
        }
    }
}