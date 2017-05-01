﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using Microsoft.TemplateEngine.Abstractions;
using Microsoft.Templates.Core.Composition;

using Xunit;

namespace Microsoft.Templates.Core.Test.Composition
{
    public class CompositionQueryTest
    {
        [Fact]
        public void ParseIvalidQueries()
        {
            var query1 = "wts.framework = framework & wts.type != Page&$name == Map";

            Assert.Throws<InvalidCompositionQueryException>(() =>
            {
               var result = CompositionQuery.Parse(query1);
            });

            var query2 = "wts.framework= framework &wts.type ! Page&$name != Map";

            Assert.Throws<InvalidCompositionQueryException>(() =>
            {
                var result = CompositionQuery.Parse(query2);
            });


            var query3 = "wts.framework==framework&wts.type!Page&$name == Map";

            Assert.Throws<InvalidCompositionQueryException>(() =>
            {
                var result = CompositionQuery.Parse(query3);
            });

            var query4 = "wts.framework   == framework & wts.type=Page& $name == Map";

            Assert.Throws<InvalidCompositionQueryException>(() =>
            {
                var result = CompositionQuery.Parse(query4);
            });
        }


        [Fact]
        public void Parse()
        {
            var query = "wts.framework == framework & wts.type != Page&$name == Map";
            var result = CompositionQuery.Parse(query);

            Assert.Collection(result.Items,
                r1 =>
                {
                    Assert.Equal(r1.Field, "wts.framework");
                    Assert.Equal(r1.Operator, QueryOperator.Equals);
                    Assert.Equal(r1.Value, "framework");
                    Assert.False(r1.IsContext);
                },
                r2 =>
                {
                    Assert.Equal(r2.Field, "wts.type");
                    Assert.Equal(r2.Operator, QueryOperator.NotEquals);
                    Assert.Equal(r2.Value, "Page");
                    Assert.False(r2.IsContext);
                },
                r3 =>
                {
                    Assert.Equal(r3.Field, "name");
                    Assert.Equal(r3.Operator, QueryOperator.Equals);
                    Assert.Equal(r3.Value, "Map");
                    Assert.True(r3.IsContext);
                });
        }

        [Fact]
        public void Parse_MultipleItems()
        {
            var query = new string[] 
            {
                "wts.framework == framework",
                "wts.type != Page",
                "$name == Map"
            };

            var result = CompositionQuery.Parse(query);

            Assert.Collection(result.Items,
                r1 =>
                {
                    Assert.Equal(r1.Field, "wts.framework");
                    Assert.Equal(r1.Operator, QueryOperator.Equals);
                    Assert.Equal(r1.Value, "framework");
                    Assert.False(r1.IsContext);
                },
                r2 =>
                {
                    Assert.Equal(r2.Field, "wts.type");
                    Assert.Equal(r2.Operator, QueryOperator.NotEquals);
                    Assert.Equal(r2.Value, "Page");
                    Assert.False(r2.IsContext);
                },
                r3 =>
                {
                    Assert.Equal(r3.Field, "name");
                    Assert.Equal(r3.Operator, QueryOperator.Equals);
                    Assert.Equal(r3.Value, "Map");
                    Assert.True(r3.IsContext);
                });
        }

        [Fact]
        public void Parse_NoValueInParam()
        {
            var query = "wts.framework == framework & wts.type";

            Assert.Throws<InvalidCompositionQueryException>(() =>
            {
                var result = CompositionQuery.Parse(query);
            });
        }

        [Fact]
        public void Match()
        {
            var data = GetFactData();

            var target = CompositionQuery.Parse("identity==item-identity&tag2==tagVal2");
            var result = target.Match(data, null);

            Assert.True(result);
        }

        [Fact]
        public void Match_WithContext()
        {
            var data = GetFactData();
            var target = CompositionQuery.Parse("identity==item-identity&tag2==tagVal2&$name==context-name");
            var context = new QueryablePropertyDictionary
            {
                new QueryableProperty("name", "context-name")
            };

            var result = target.Match(data, context);

            Assert.True(result);
        }

        [Fact]
        public void Match_NotEquals()
        {
            var data = GetFactData();
            var target = CompositionQuery.Parse("identity==item-identity&tag2!=tagVal1");
            var result = target.Match(data, null);

            Assert.True(result);
        }

        [Fact]
        public void Match_MultiValue()
        {
            var data = GetFactData();
            var target = CompositionQuery.Parse("identity==item-identity&tag3==tag3Val1");
            var result = target.Match(data, null);

            Assert.True(result);
        }

        [Fact]
        public void Match_MultiValueBoth()
        {
            var data = GetFactData();
            var target = CompositionQuery.Parse("identity==item-identity&tag3==tag3Val1|tag3Val3");
            var result = target.Match(data, null);

            Assert.True(result);
        }

        private ITemplateInfo GetFactData()
        {
            var templateInfo = new FakeTemplateInfo
            {
                Identity = $"item-identity",
                Name = $"item-name"
            };

            templateInfo.AddTag($"tag1", $"tagVal1");
            templateInfo.AddTag($"tag2", $"tagVal2");
            templateInfo.AddTag($"tag3", $"tag3Val1|tag3Val2");

            return templateInfo;
        }
    }

}
