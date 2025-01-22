using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.Modles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportOverDueFileUploader.ValidateIqBizLogic
{
    public class ResultAndVariance
    {
        public int Result { get; set; }
        public decimal? Variance { get; set; }
    }
    public static class Compression
    {
        private static Queue<ComparatorSetting> summaryItems = new();
        private static HashSet<string> seen = [];
        private static JToken? values1 = null;
        private static JToken? values2 = null;
        private static string Value1Json = "";
        private static string Value2Json = "";
        //private static ExportOverDueContext _context = new();

        //public static IDictionary<string, List<ComparisonResult>> CompareGdAndFi(List<FiGds> fiGds, List<ComparatorSetting> ComparatorSetting, long ReqStatusId, string BaseFeild = "FI")
        public static List<ComparisonResult> CompareGdAndFi(List<FiGds> fiGds, List<ComparatorSetting> ComparatorSetting, long ReqStatusId, string BaseFeild = "FI")
        {
            //IDictionary<string, List<ComparisonResult>> results = new Dictionary<string, List<ComparisonResult>>();
            List<ComparisonResult> summaryResults = [];
            List<ComparisonResult> comparisonResults = [];

            foreach (FiGds fiGd in fiGds)
            {
                foreach (var gd in fiGd.Gds)
                {
                    var result = CompareGdAndFi(JsonConvert.SerializeObject(gd.GdNotSerialized), fiGd.FiPayload, ComparatorSetting, ReqStatusId, BaseFeild, gd.GdFiId);
                    //results.Add(gd.GdNotSerialized.SelectToken("data.gdNumber")?.ToString() ?? "", result);
                    comparisonResults.AddRange(result);
                }

                while (summaryItems.Count > 0)
                {
                    var setting = summaryItems.Dequeue();

                    if (setting.Entity1Key is null || setting.Entity2Key is null)
                        continue;

                    if (setting.Entity1Key.Contains("itemInformation[i]") && setting.Entity2Key.Contains("itemInformation[i]"))
                    {
                        summaryResults.AddRange(
                            ProcessItemInformation(
                                JsonConvert.SerializeObject(fiGd.GdsNotSerialized),
                                fiGd.FiPayload,
                                setting,
                                ReqStatusId,
                                BaseFeild,
                                fiGd.FiId
                            )
                        );
                    }
                    else if (setting.Entity1Key.Contains(".Count()") && setting.Entity2Key.Contains(".Count()"))
                    {
                        summaryResults.AddRange(
                            ProcessItemCounts(
                                JsonConvert.SerializeObject(fiGd.GdsNotSerialized),
                                fiGd.FiPayload,
                                setting,
                                ReqStatusId,
                                BaseFeild,
                                fiGd.FiId
                            )
                        );
                    }
                    else
                    {
                        summaryResults.AddRange(
                            ProcessFields(
                                JsonConvert.SerializeObject(fiGd.GdsNotSerialized),
                                fiGd.FiPayload,
                                ReqStatusId,
                                setting,
                                BaseFeild,
                                fiGd.FiId
                            )
                        );
                    }
                }
                //results.Add(Guid.NewGuid().ToString(), summaryResults);
                seen = [];
                //_context.ComparisonResults.AddRange(comparisonResults);
                //_context.SaveChanges();
            }
            comparisonResults.AddRange(summaryResults);
            //return results;
            return comparisonResults;
        }

        public static List<ComparisonResult> CompareGdAndFi(string gdJson, string fiJson, List<ComparatorSetting> ComparatorSetting, long ReqStatusId, string BaseFeild = "FI", long? gId = null)
        {
            (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");
            //string TrailingFeild = (BaseFeild == "GD") ? "FI" : "GD";

            List<ComparisonResult> result = [];
            foreach (var setting in ComparatorSetting)
            {
                List<JToken> test = [];
                try
                {
                    if (setting.Entity1Key is null || setting.Entity2Key is null)
                    {
                        throw new Exception();
                    }
                    string uniqueKey = $"{setting.Entity1Key}-{setting.Entity2Key}";

                    if (setting.Entity1Key.Contains("itemInformation[i]") && setting.Entity2Key.Contains("itemInformation[i]"))
                    {
                        if (seen.Contains(uniqueKey))
                        {
                            continue;
                        }
                        summaryItems.Enqueue(setting);
                        seen.Add(uniqueKey);
                        continue;
                    }
                    else if (setting.Entity1Key.Contains(".Count()") && setting.Entity2Key.Contains(".Count()"))
                    {
                        if (seen.Contains(uniqueKey))
                        {
                            continue;
                        }
                        summaryItems.Enqueue(setting);
                        seen.Add(uniqueKey);
                        continue;
                    }
                    else
                    {
                        if (setting.Entity1Key == "data.financialInfo.assessedValueUsd"
                            || setting.Entity1Key == "data.paymentInformation.financialInstrumentValue")
                        {
                            if (seen.Contains(uniqueKey))
                            {
                                continue;
                            }
                            summaryItems.Enqueue(setting);
                            seen.Add(uniqueKey);
                            continue;
                        }
                        var res = ProcessFields(gdJson, fiJson, ReqStatusId, setting, BaseFeild, gId: gId);
                        result.AddRange(res);
                    }
                }

                catch (Exception)
                {
                    //return null;

                }
            }
            return result;
        }

        private static List<ComparisonResult> ProcessFields(string gdJson, string fiJson, long ReqStatusId, ComparatorSetting setting, string BaseFeild = "FI", long? fId = null, long? gId = null )
        {
            try
            {
                List<ComparisonResult> result = [];

                (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");
                string TrailingFeild = (BaseFeild == "GD") ? "FI" : "GD";

                if (setting.Entity1Key is null || setting.Entity2Key is null)
                {
                    throw new Exception();
                }

                if (setting.IsSameEntity == 1)
                {
                    values1 = GetKeyJsonGetter(Value1Json, setting.Entity1Key);
                    values2 = GetKeyJsonGetter(Value1Json, setting.Entity2Key);
                }
                else if (setting.IsSameEntity == 2)
                {
                    values1 = GetKeyJsonGetter(Value2Json, setting.Entity1Key);
                    values2 = GetKeyJsonGetter(Value2Json, setting.Entity2Key);
                }
                else
                {
                    values1 = GetKeyJsonGetter(Value1Json, setting.Entity1Key);
                    values2 = GetKeyJsonGetter(Value2Json, setting.Entity2Key);
                }

                var Comparision = CompareJsonTokens(TryConvertToFloat(values1), TryConvertToFloat(values2), setting.CalculateVariance);
                result.Add(new ComparisonResult
                {
                    ComparisonType = setting.ValidationType,
                    Entity1Key = $"{setting.Entity1Key}-({(setting.IsSameEntity == 2 ? TrailingFeild : BaseFeild)})",
                    Entity2Key = $"{setting.Entity2Key}-({(setting.IsSameEntity == 1 ? BaseFeild : TrailingFeild)})",
                    Entity2Value = values2?.ToString(),
                    Entity1Value = values1?.ToString(),
                    ComparisonName = setting.ValidationType,
                    Result = Comparision.Result,
                    Variance = Comparision?.Variance,
                    RequestStatusId = ReqStatusId,
                    TenantId = AppSettings.TenantId,
                    FiId = fId,
                    GdFiLinkId = gId
                });

                return result;
            }
            catch (Exception)
            {

                return [];
            }
        }

        private static List<ComparisonResult> ProcessItemInformation(string gdJson, string fiJson, ComparatorSetting setting, long ReqStatusId, string BaseFeild = "FI", long? fId = null, long? gId = null)
        {
            try
            {
                var (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");
                string TrailingFeild = (BaseFeild == "GD") ? "FI" : "GD";
                var result = new List<ComparisonResult>();

                if (setting.Entity1Key == null || setting.Entity2Key == null)
                {
                    throw new Exception("Entity keys cannot be null.");
                }

                var Tokens1 = GetJsonListValues(Value1Json, setting.Entity1Key[..setting.Entity1Key.IndexOf("[i]")]);
                var Tokens2 = GetJsonListValues(Value2Json, setting.Entity2Key[..setting.Entity2Key.IndexOf("[i]")]);

                // Build a hash map for Tokens2 based on hsCode
                var tokens2Map = Tokens2
                    .GroupBy(token => token["hsCode"]?.ToString())
                    .ToDictionary(group => group.Key!, group => group.ToList());

                foreach (var token1 in Tokens1)
                {
                    var hsCode1 = token1["hsCode"]?.ToString();
                    if (hsCode1 == null || !tokens2Map.TryGetValue(hsCode1, out var matchingTokens2))
                    {
                        // hsCode does not match
                        result.Add(new ComparisonResult
                        {
                            ComparisonType = $"{setting.ValidationType} Comparison > FI",
                            Entity1Key = setting.Entity1Key,
                            Entity2Key = setting.Entity2Key,
                            Entity1Value = token1["hsCode"]?.ToString(),
                            Entity2Value = "N/A",
                            ComparisonName = setting.ValidationType,
                            Result = 0,
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                            FiId = fId,
                            GdFiLinkId = gId
                        });
                        continue;
                    }

                    // Iterate over matching tokens with the same hsCode
                    foreach (var token2 in matchingTokens2)
                    {
                        //Hs Code Matches but have to do explicit match for variance.
                        var hsCode2 = token1["hsCode"]?.ToString();
                        var hsCodeComparision = CompareJsonTokens(hsCode1, hsCode2, setting.CalculateVariance);

                        result.Add(new ComparisonResult
                        {
                            ComparisonType = $"{setting.ValidationType} Comparison > FI",
                            Entity1Key = setting.Entity1Key,
                            Entity2Key = setting.Entity2Key,
                            Entity1Value = hsCode1,
                            Entity2Value = hsCode2,
                            ComparisonName = setting.ValidationType,
                            Result = hsCodeComparision.Result,
                            Variance = hsCodeComparision.Variance,
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                            FiId = fId,
                            GdFiLinkId = gId
                        });

                        var uom1 = token1["uom"]?.ToString();
                        var uom2 = token2["uom"]?.ToString();

                        var uomComparison = CompareJsonTokens(uom1, uom2, setting.CalculateVariance);
                            // uom matches
                        result.Add(new ComparisonResult
                        {
                            ComparisonType = $"UOM Comparison > FI",
                            Entity1Key = setting.Entity1Key.Replace("hsCode", "uom"),
                            Entity2Key = setting.Entity2Key.Replace("hsCode", "uom"),
                            Entity1Value = uom1,
                            Entity2Value = uom2,
                            ComparisonName = "UOM",
                            Result = uomComparison.Result,
                            Variance = uomComparison.Variance,
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                            FiId = fId,
                            GdFiLinkId = gId
                        });

                        // Perform additional comparisons for quantity
                        var quantity1 = token1["quantity"];
                        var quantity2 = token2["quantity"];
                        var quantityComparison = CompareJsonTokens(quantity1, quantity2, setting.CalculateVariance);

                        result.Add(new ComparisonResult
                        {
                            ComparisonType = $"Quantity Comparison > FI",
                            Entity1Key = setting.Entity1Key.Replace("hsCode", "quantity"),
                            Entity2Key = setting.Entity2Key.Replace("hsCode", "quantity"),
                            Entity1Value = quantity1?.ToString(),
                            Entity2Value = quantity2?.ToString(),
                            ComparisonName = "Quantity",
                            Result = quantityComparison.Result,
                            Variance = quantityComparison.Variance,
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                            FiId = fId,
                            GdFiLinkId = gId
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return [];
            }
        }

        private static List<ComparisonResult> ProcessItemCounts(string gdJson, string fiJson, ComparatorSetting setting, long ReqStatusId, string BaseFeild = "FI", long? fId = null, long? gId = null)
        {
            try
            {
                if (setting.Entity1Key is null || setting.Entity2Key is null)
                {
                    throw new Exception();
                }
                (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");
                string TrailingFeild = (BaseFeild == "GD") ? "FI" : "GD";
                List<ComparisonResult> result = [];

                values1 = GetKeyJsonGetter(Value1Json, setting.Entity1Key.Replace(".Count()", ""));
                values2 = GetKeyJsonGetter(Value2Json, setting.Entity2Key.Replace(".Count()", ""));
                var Comparision = CompareJsonTokens(values1, values2, setting.CalculateVariance);
                result.Add(new ComparisonResult
                {
                    ComparisonType = setting.ValidationType,
                    Entity1Key = setting.Entity1Key,
                    Entity2Key = setting.Entity2Key,
                    Entity2Value = values2?.Count().ToString(),
                    Entity1Value = values1?.Count().ToString(),
                    ComparisonName = setting.ValidationType,
                    Result = Comparision.Result,
                    Variance = Comparision.Variance,
                    RequestStatusId = ReqStatusId,
                    TenantId = AppSettings.TenantId,
                    FiId = fId,
                    GdFiLinkId = gId
                });

                return result;
            }
            catch (Exception)
            {
                return [];
            }
        }


        public static ResultAndVariance CompareJsonTokens(JToken? token1, JToken? token2, bool CalculateVariance)
        {
            ResultAndVariance result = new();
            try
            {
                if (token1 is null || token2 is null)
                {
                    result.Result = 1;
                }
                else if (token1 is not null && token2 is not null && (token1.Type == JTokenType.Integer || token1.Type == JTokenType.Float) && (token2.Type == JTokenType.Integer || token2.Type == JTokenType.Float))
                {
                    double value1 = token1.Type == JTokenType.Float ? token1.Value<double>() : token1.Value<int>();
                    double value2 = token2.Type == JTokenType.Float ? token2.Value<double>() : token2.Value<int>();
                    var x = Math.Abs(value1 - value2);

                    if (x < 1 && x > -1)
                    {
                        result.Result = 1;
                    }
                    else
                    {
                        result.Result = 0;
                    }
                    if (CalculateVariance)
                    {
                        if (!(x < 1 && x > -1))
                        {
                            result.Variance = CalculateVariances(TryConvertToFloat(value1), TryConvertToFloat(value2));
                        }
                    }
                }
                else if (token1 is not null && token2 is not null && (token1.Type == JTokenType.Array || token2.Type == JTokenType.Array))
                {
                    if (token1.Count() == token2.Count())
                    {
                        result.Result = 1;
                    }
                    else
                    {
                        result.Result = 0;
                    }
                    if (CalculateVariance)
                    {
                        result.Variance = CalculateVariances(TryConvertToFloat(token1.Count()), TryConvertToFloat(token2.Count()));
                    }
                }
                else if (token1 is not null && token2 is not null && (token1.Type == JTokenType.String && token2.Type == JTokenType.String))
                {
                    if (string.Compare(token1.ToString(), token2.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result.Result = 1;
                    }
                    else
                    {
                        result.Result = 0;
                    }
                }
                else
                {
                    if (Equals(token1, token2))
                    {
                        result.Result = 1;
                    }
                    else
                    {
                        result.Result = 0;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Information($"CalculateVariance->{ex.Message}");
                return result;
            }
        }

        public static JToken? GetKeyJsonGetter(string jsonString, string key)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                JToken? token = jsonObject.SelectToken(key);

                if (token?.Type == JTokenType.Array)
                {
                    var groupedItems = token
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();
                    return new JArray(groupedItems);
                }

                return token;
            }
            catch (Exception)
            {
                JArray jsonArray = JArray.Parse(jsonString);

                var tokens = jsonArray
                    .SelectMany(ja => ja.SelectTokens(key))
                    //.Distinct()
                    .ToList();

                var numericTokens = tokens
                    .Where(t => t.Type == JTokenType.Float || t.Type == JTokenType.Integer)
                    .ToList();

                if (IsListOfLists(tokens))
                {
                    var flattenedTokens = FlattenListOfLists(tokens);
                    var groupedItems = flattenedTokens
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();
                    return new JArray(groupedItems);
                }
                else if (numericTokens.Count != 0)
                {
                    var sum = numericTokens.Sum(t => t.Value<decimal>());
                    return sum;
                }

                var tokenFromTokensLst = tokens
                    .FirstOrDefault()?
                    .SelectToken(key);

                if (tokenFromTokensLst?.Type ==  JTokenType.Float)
                {
                    return tokenFromTokensLst.Value<decimal>();
                }

                var groupedItemsofLst =
                    tokenFromTokensLst?
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();
                return new JArray(groupedItemsofLst ?? []);
                //return tokens.FirstOrDefault();
            }
        }

        public static List<JToken> GetJsonListValues(string jsonString, string key)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                JToken? token = jsonObject.SelectToken(key);

                if (token?.Type == JTokenType.Array)
                {
                    var groupedItems = token
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();
                    return groupedItems;
                }

                return (token is not null && token.Type == JTokenType.Array) ? [.. token] : [];

            }
            catch (Exception)
            {
                JArray jsonArray = JArray.Parse(jsonString);

                var tokens = jsonArray
                .SelectMany(ja => ja.SelectTokens(key))
                //.Distinct()
                .ToList();

                if (!IsListOfLists(tokens))
                {

                    var groupedItemsofLst = tokens
                    .FirstOrDefault()?
                    .SelectToken(key)?
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();

                    return groupedItemsofLst ?? [];
                    //return tokens;
                }

                var flattenedTokens = FlattenListOfLists(tokens);

                var groupedItems = flattenedTokens
                    .GroupBy(item => new
                    {
                        hsCode = item["hsCode"]?.ToString(),
                        uom = item["uom"]?.ToString()
                    })
                    .Select(group =>
                    {
                        return new JObject
                        {
                            ["hsCode"] = group.Key.hsCode,
                            ["uom"] = group.Key.uom,
                            ["quantity"] = group.Sum(x => x["quantity"]?.Value<decimal>() ?? 0),
                            ["totalValue"] = group.Sum(x => x["totalValue"]?.Value<decimal>() ?? 0),
                            ["importValue"] = group.Sum(x => x["importValue"]?.Value<decimal>() ?? 0)
                        };
                    })
                    .Cast<JToken>()
                    .ToList();

                return groupedItems;
            }
        }

        private static IEnumerable<JToken> FlattenListOfLists(IEnumerable<JToken> tokens)
        {
            var flatList = new List<JToken>();
            var queue = new Queue<JToken>(tokens);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Type == JTokenType.Array)
                {
                    foreach (var child in current.Children())
                    {
                        queue.Enqueue(child);
                    }
                }
                else
                {
                    flatList.Add(current);
                }
            }

            return flatList;
        }

        private static bool IsListOfLists(IEnumerable<JToken> tokens)
        {
            return tokens.All(t => t.Type == JTokenType.Array);
        }


        public static decimal? CalculateVariances(float? val1, float? val2)
        {
            try
            {
                decimal? variance = null;
                if (val1 is not null && val2 is not null)
                {
                    float v = ((val1.Value - val2.Value) / val1.Value) * 100;

                    if (v > 100)
                    {
                        variance = 101;
                    }
                    else if (v < -100)
                    {
                        variance = -101;
                    }
                    else
                    {
                        variance = (decimal)v;
                    }
                }
                return variance;
            }
            catch (Exception ex)
            {
                Seriloger.LoggerInstance.Information($"CalculateVariance->{ex.Message}");
                return null; // Return null in case of an error
            }
        }

        private static float? TryConvertToFloat(object? value)
        {
            try
            {
                if (value is null)
                {
                    return null;
                }
                return float.TryParse(value.ToString(), out var result) ? result : null;
            }
            catch
            {
                return null;
            }
        }

    }

}


