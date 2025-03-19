using ExportOverDueFileUploader.DBmodels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2
{
    public class ResultAndVariance
    {
        public int Result { get; set; }
        public decimal? Variance { get; set; }
    }
    public static class Comparison_V2
    {
        private static Queue<ComparatorSetting> summaryItems = new();
        private static HashSet<string> seen = [];
        private static JToken? values1 = null;
        private static JToken? values2 = null;
        private static string Value1Json = "";
        private static string Value2Json = "";

        public static List<ComparisonResult> CompareGdAndFi(
            List<ComparisonInput> baseRecords,
            List<ComparatorSetting> ComparatorSetting,
            long ReqStatusId,
            ComparisonType comparisonType)
        {
            List<ComparisonResult> summaryResults = [];
            List<ComparisonResult> comparisonResults = [];

            foreach (ComparisonInput baseRecord in baseRecords)
            {
                foreach (var relatedRecord in baseRecord.RelatedRecords)
                {
                    var result = CompareGdAndFi(JsonConvert.SerializeObject(relatedRecord.NotSerialized),
                        baseRecord.Payload,
                        ComparatorSetting,
                        ReqStatusId,
                        comparisonType,
                        baseRecord.Type.ToString(),
                        relatedRecord.RelationId);
                    comparisonResults.AddRange(result);
                }

                long? fId = null;
                long? gId = null;
                long? gdfid = null;

                //If 1 FI And * Gds the set fi id.
                if (baseRecord.Type == DocumentType.FI)
                {
                    fId = baseRecord.Id;
                }
                //If 1 Gd And * Fis then set gd id.
                else if (baseRecord.Type == DocumentType.GD)
                {
                    gId = baseRecord.Id;
                }

                //Performs the comparison on aggregiate fi or gd
                while (summaryItems.Count > 0)
                {
                    var setting = summaryItems.Dequeue();

                    if (setting.Entity1Key is null || setting.Entity2Key is null)
                        continue;

                    if (setting.Entity1Key.Contains("itemInformation[i]") && setting.Entity2Key.Contains("itemInformation[i]"))
                    {
                        summaryResults.AddRange(
                            ProcessItemInformation(
                                JsonConvert.SerializeObject(baseRecord.NotSerialized),
                                baseRecord.Payload,
                                setting,
                                ReqStatusId,
                                comparisonType,
                                baseRecord.Type.ToString(),
                                fId,
                                gId,
                                gdfid
                            )
                        );
                    }
                    else if (setting.Entity1Key.Contains(".Count()") && setting.Entity2Key.Contains(".Count()"))
                    {
                        summaryResults.AddRange(
                            ProcessItemCounts(
                                JsonConvert.SerializeObject(baseRecord.NotSerialized),
                                baseRecord.Payload,
                                setting,
                                ReqStatusId,
                                comparisonType,
                                baseRecord.Type.ToString(),
                                fId,
                                gId,
                                gdfid
                            )
                        );
                    }
                    else
                    {
                        summaryResults.AddRange(
                            ProcessFields(
                                JsonConvert.SerializeObject(baseRecord.NotSerialized),
                                baseRecord.Payload,
                                ReqStatusId,
                                comparisonType,
                                setting,
                                baseRecord.Type.ToString(),
                                fId,
                                gId,
                                gdfid
                            )
                        );
                    }
                }
                seen = [];
            }
            comparisonResults.AddRange(summaryResults);
            return comparisonResults;
        }

        /*
         * This method gets fi and gd payloads, if the field is aggregiated field, it then adds the record in the queue for later processing.
         * Seen make sure each key is added once for all the related records.
        */
        public static List<ComparisonResult> CompareGdAndFi(string gdJson,
            string fiJson,
            List<ComparatorSetting> ComparatorSetting,
            long ReqStatusId,
            ComparisonType comparisonType,
            string BaseFeild = "FI",
            long? figdId = null)
        {
            (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");

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

                    if (setting.IsAggregiated)
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
                        var res = ProcessFields(gdJson, fiJson, ReqStatusId, comparisonType, setting, BaseFeild, figdId: figdId);
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

        //Performs a comparison for the given field type.
        private static List<ComparisonResult> ProcessFields(
            string gdJson,
            string fiJson,
            long ReqStatusId,
            ComparisonType comparisonType,
            ComparatorSetting setting,
            string BaseFeild = "FI",
            long? fId = null,
            long? gId = null,
            long? figdId = null)
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

                var Comparision = CompareJsonTokens(values1, values2, setting.CalculateVariance);

                result.Add(GetResult(
                    setting,
                    ReqStatusId,
                    fId,
                    gId,
                    figdId,
                    BaseFeild,
                    values1,
                    values2,
                    Comparision.Result,
                    Comparision.Variance,
                    setting.ValidationType,
                    comparisonType));

                return result;
            }
            catch (Exception)
            {

                return [];
            }
        }

        //Compares all the hscodes in base and related field, is summarized always.
        private static List<ComparisonResult> ProcessItemInformation(
            string gdJson,
            string fiJson,
            ComparatorSetting setting,
            long ReqStatusId,
            ComparisonType comparisonType,
            string BaseFeild = "FI",
            long? fId = null,
            long? gId = null,
            long? figdId = null)
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

                // Build a hash map for Tokens2 based on hsCode.
                var tokens2Map = Tokens2
                    .GroupBy(token => token["hsCode"]?.ToString())
                    .ToDictionary(group => group.Key!, group => group.ToList());

                // Keep track of hsCodes encountered in Tokens1
                var tokens1HsCodes = new HashSet<string>();

                foreach (var token1 in Tokens1)
                {
                    var hsCode1 = token1["hsCode"]?.ToString();
                    if (hsCode1 == null || !tokens2Map.TryGetValue(hsCode1, out var matchingTokens2))
                    {
                        int match = 0;
                        // If hscode does not match terminate further processing for same hscode of base field and add result to result list.
                        result.Add(GetResult(setting, ReqStatusId, fId, gId, figdId, BaseFeild, hsCode1, null, match, null, "hscode", comparisonType));
                        continue;
                    }
                    tokens1HsCodes.Add(hsCode1);
                    // Iterate over matching tokens with the same hsCode
                    foreach (var token2 in matchingTokens2)
                    {
                        //Hs Code Matches but have to do explicit match for variance.
                        var hsCode2 = token1["hsCode"]?.ToString();
                        var hsCodeComparision = CompareJsonTokens(hsCode1, hsCode2, false); // false as there'll be no need to calc variance for hscode.

                        result.Add(GetResult(
                            setting,
                            ReqStatusId,
                            fId,
                            gId,
                            figdId,
                            BaseFeild,
                            hsCode1,
                            hsCode2,
                            hsCodeComparision.Result,
                            hsCodeComparision.Variance,
                            "hscode",
                            comparisonType));

                        var uom1 = token1["uom"]?.ToString();
                        var uom2 = token2["uom"]?.ToString();

                        //Continue with uom comparison.
                        var uomComparison = CompareJsonTokens(uom1, uom2, false); // false as there'll be no need to calc variance for uom.
                        result.Add(GetResult(setting,
                            ReqStatusId,
                            fId,
                            gId,
                            figdId,
                            BaseFeild,
                            uom1,
                            uom2,
                            uomComparison.Result,
                            uomComparison.Variance,
                            "uom",
                            comparisonType));

                        // Perform comparisons for quantity
                        var quantity1 = token1["quantity"];
                        var quantity2 = token2["quantity"];

                        var quantityComparison = CompareJsonTokens(quantity1, quantity2, setting.CalculateVariance);
                        result.Add(GetResult(
                            setting,
                            ReqStatusId,
                            fId,
                            gId,
                            figdId,
                            BaseFeild,
                            quantity1,
                            quantity2,
                            quantityComparison.Result,
                            quantityComparison.Variance,
                            "quantity",
                            comparisonType));
                    }
                }

                foreach (var token2 in Tokens2)
                {
                    var hsCode2 = token2["hsCode"]?.ToString();
                    if (hsCode2 is not null && !tokens1HsCodes.Contains(hsCode2))
                    {
                        int match = 0;
                        // Add result for unmatched hsCodes from Tokens2
                        result.Add(GetResult(setting, ReqStatusId, fId, gId, figdId, BaseFeild, null, hsCode2, match, null, "hscode", comparisonType));
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

        private static List<ComparisonResult> ProcessItemCounts(
            string gdJson,
            string fiJson,
            ComparatorSetting setting,
            long ReqStatusId,
            ComparisonType comparisonType,
            string BaseFeild = "FI",
            long? fId = null,
            long? gId = null,
            long? figdId = null)
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

                //Compares both input tokens and returns the result.
                var Comparision = CompareJsonTokens(values1, values2, setting.CalculateVariance);
                result.Add(GetResult(
                    setting,
                    ReqStatusId,
                    fId,
                    gId,
                    figdId,
                    BaseFeild,
                    values1?.Count(),
                    values2?.Count(),
                    Comparision.Result,
                    Comparision.Variance,
                    "itemcount",
                    comparisonType));
                return result;
            }
            catch (Exception)
            {
                return [];
            }
        }

        /*
         * Returns the appropiate child object of comparison result. If the comparison input type is import then returns the import child object.
         * If the comparison type is export then returns the export child object. The bit IsAggregiated check whether the input is for aggregiated 
         * or single comparison and then returns the appropiate child object against the input.
        */
        private static ComparisonResult GetResult(
            ComparatorSetting setting,
            long ReqStatusId,
            long? fId,
            long? gId,
            long? figdId,
            string comparisonCategory,
            JToken? entity1Value,
            JToken? entity2Value,
            int? result,
            decimal? variance,
            string? keyType,
            ComparisonType comparisonType
            )
        {
            if (comparisonType == ComparisonType.Import)
            {
                if (setting.IsAggregiated)
                {
                    return new AggregiatedResultImport
                    {
                        ComparisonType = $"{keyType?.ToUpper()} Comparison > {comparisonCategory}",
                        Entity1Key = keyType != null ? setting.Entity1Key!.Replace("hsCode", keyType) : setting.Entity1Key,
                        Entity2Key = keyType != null ? setting.Entity2Key!.Replace("hsCode", keyType) : setting.Entity2Key,
                        Entity1Value = entity1Value?.ToString() ?? "N/A",
                        Entity2Value = entity2Value?.ToString() ?? "N/A",
                        ComparisonName = keyType,
                        Result = result ?? 0,
                        Variance = variance,
                        RequestStatusId = ReqStatusId,
                        TenantId = AppSettings.TenantId,
                        FiId = fId,
                        GdId = gId
                    };
                }
                else
                {
                    return new ComparisonResultImport
                    {
                        ComparisonType = $"{keyType?.ToUpper()} Comparison > {comparisonCategory}",
                        Entity1Key = keyType != null ? setting.Entity1Key!.Replace("hsCode", keyType) : setting.Entity1Key,
                        Entity2Key = keyType != null ? setting.Entity2Key!.Replace("hsCode", keyType) : setting.Entity2Key,
                        Entity1Value = entity1Value?.ToString() ?? "N/A",
                        Entity2Value = entity2Value?.ToString() ?? "N/A",
                        ComparisonName = keyType,
                        Result = result ?? 0,
                        Variance = variance,
                        RequestStatusId = ReqStatusId,
                        TenantId = AppSettings.TenantId,
                        GdFiLinkId = figdId
                    };
                }
            }
            else if (comparisonType == ComparisonType.Export)
            {
                if (setting.IsAggregiated)
                {
                    return new AggregiatedResultExport
                    {
                        ComparisonType = $"{keyType?.ToUpper()} Comparison > {comparisonCategory}",
                        Entity1Key = keyType != null ? setting.Entity1Key!.Replace("hsCode", keyType) : setting.Entity1Key,
                        Entity2Key = keyType != null ? setting.Entity2Key!.Replace("hsCode", keyType) : setting.Entity2Key,
                        Entity1Value = entity1Value?.ToString() ?? "N/A",
                        Entity2Value = entity2Value?.ToString() ?? "N/A",
                        ComparisonName = keyType,
                        Result = result ?? 0,
                        Variance = variance,
                        RequestStatusId = ReqStatusId,
                        TenantId = AppSettings.TenantId,
                        FiId = fId,
                        GdId = gId,
                    };
                }
                else
                {
                    return new ComparisonResultExport
                    {
                        ComparisonType = $"{keyType?.ToUpper()} Comparison > {comparisonCategory}",
                        Entity1Key = keyType != null ? setting.Entity1Key!.Replace("hsCode", keyType) : setting.Entity1Key,
                        Entity2Key = keyType != null ? setting.Entity2Key!.Replace("hsCode", keyType) : setting.Entity2Key,
                        Entity1Value = entity1Value?.ToString() ?? "N/A",
                        Entity2Value = entity2Value?.ToString() ?? "N/A",
                        ComparisonName = keyType,
                        Result = result ?? 0,
                        Variance = variance,
                        RequestStatusId = ReqStatusId,
                        TenantId = AppSettings.TenantId,
                        Gd_Fi_LinkId = figdId
                    };
                }
            }
            return null;
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

        //Get the value from specified key of json object
        public static JToken? GetKeyJsonGetter(string jsonString, string key)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                JToken? token = jsonObject.SelectToken(key);

                //If value is array then group all the hscodes by their hscode and uom
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
                //If the given object was jArray (when related entities are multiple) then parse it into jArray.
                JArray jsonArray = JArray.Parse(jsonString);

                //Get all the tokens for a key.
                var tokens = jsonArray
                    .SelectMany(ja => ja.SelectTokens(key))
                    //.Distinct()
                    .ToList();

                //Selects the numeric tokens for sum.
                var numericTokens = tokens
                    .Where(t => t.Type == JTokenType.Float || t.Type == JTokenType.Integer)
                    .ToList();

                //Checks whether the tokens are list of list, if then flatten the list of list and then group afterwards.
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

                //Sum all the numeric tokens.
                else if (numericTokens.Count != 0)
                {
                    var sum = numericTokens.Sum(t => t.Value<decimal>());
                    return sum;
                }

                //If the tokens is not list of list then select the first item in list and and perform same step as above for it. Applicable in minor cases.
                var tokenFromTokensLst = tokens
                    .FirstOrDefault()?
                    .SelectToken(key);

                if (tokenFromTokensLst?.Type == JTokenType.Float)
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



