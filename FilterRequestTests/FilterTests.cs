using FilterRequestApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace FilterRequestTests
{
    public class FilterTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FilterTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test1()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetFromJsonAsync<FilterRequest>("/test?filter[dob-gte]=19850929&filter[dob-lte]=20221231&filter[lastName-contains]=doe&sort[lastName]=desc&page[number]=2&page[size]=50");

            Assert.NotNull(response.FilterValues);

            Assert.Equal(3, response.FilterValues.Count);

            Assert.Equal("dob", response.Filters[0].Field);
            Assert.Equal(FilterModifier.GTE, response.Filters[0].Modifier);
            Assert.Equal("19850929", response.Filters[0].Value);

            Assert.Equal("dob", response.Filters[1].Field);
            Assert.Equal(FilterModifier.LTE, response.Filters[1].Modifier);
            Assert.Equal("20221231", response.Filters[1].Value);

            Assert.Equal("lastName", response.Filters[2].Field);
            Assert.Equal(FilterModifier.Contains, response.Filters[2].Modifier);
            Assert.Equal("doe", response.Filters[2].Value);

            Assert.NotNull(response.Sort);
            Assert.Equal("lastName", response.Sort.Field);
            Assert.Equal(SortDirection.Desc, response.Sort.Direction);

            Assert.NotNull(response.SortValues);
            Assert.NotNull(response.PageValues);
        }
    }
}