using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FilterRequestApi
{
    [ApiController]
    [Route("test")]
    public class TestController
    {
        [HttpGet]
        public IActionResult Get([FromQuery] FilterRequest request)
        {
            return new OkObjectResult(request);
        }
    }

    public class FilterRequest : RequestBase
    {
        // This is where you would put any additional query parameters.
    }

    public abstract class RequestBase
    {
        private Lazy<List<Filter>> _filters;
        private Lazy<Sort> _sort;

        public RequestBase()
        {
            _filters = new Lazy<List<Filter>>(() => {
                var results = new List<Filter>();

                foreach (KeyValuePair<string, string> filterValue in FilterValues)
                {
                    results.Add(new Filter(filterValue.Key, filterValue.Value));
                }

                return results;
            }, true);

            _sort = new Lazy<Sort>(() => {
                KeyValuePair<string, string> sortValue = SortValues.Last();

                return new Sort(sortValue.Key, sortValue.Value);
                }
            , true);
        }

        [FromQuery(Name = "filter")]
        public Dictionary<string, string> FilterValues { get; set; }

        [FromQuery(Name = "sort")]
        public Dictionary<string, string> SortValues { get; set; }

        [FromQuery(Name = "page")]
        public Dictionary<string, string> PageValues { get; set; }

        public List<Filter> Filters => _filters.Value;

        public Sort Sort => _sort.Value;
    }

    public sealed class Sort
    {
        public Sort(string field, string direction)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException($"'{nameof(field)}' cannot be null or whitespace.", nameof(field));
            }

            Field = field;

            if (string.IsNullOrWhiteSpace(direction))
            {
                Direction = SortDirection.Asc;
            }
            else
            {
                if(Enum.TryParse(direction, true, out SortDirection sortDirection))
                {
                    Direction = sortDirection;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(direction), $"Invalid Sort Direction {direction}.");
                }
            }
        }

        public string Field { get; }
        public SortDirection Direction { get; }
    }

    public sealed class Filter
    {
        public Filter(string field, string value)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException($"'{nameof(field)}' cannot be null or whitespace.", nameof(field));
            }

            string[] filterSegments = field.Split('-');
            Field = filterSegments[0];

            if (filterSegments.Length > 1)
            {
                string modifierRaw = filterSegments[1];
                if (Enum.TryParse(modifierRaw, true, out FilterModifier modifier))
                {
                    Modifier = modifier;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(field), $"Invalid Modifier {modifierRaw}.");
                }
            }
            Value = value;
        }

        public string Field { get; }

        public FilterModifier Modifier { get; }

        public string Value { get; }
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }

    public enum FilterModifier
    {
        EQ,
        GT,
        GTE,
        LT,
        LTE,
        Contains
    }
}
