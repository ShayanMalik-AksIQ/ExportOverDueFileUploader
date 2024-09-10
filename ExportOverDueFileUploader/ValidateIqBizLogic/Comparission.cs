using ExportOverDueFileUploader.DBmodels;
using ExportOverDueFileUploader.Modles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportOverDueFileUploader.ValidateIqBizLogic
{
    public static class Compression
    {
        public static List<ComparisonResult> CompareGdAndFi(string gdJson, string fiJson, List<ComparatorSetting> ComparatorSetting, long ReqStatusId, string BaseFeild = "FI")
        {

            JToken? values1 = null;
            JToken? values2 = null;
            string Value1Json = "";
            string Value2Json = "";
            (Value1Json, Value2Json) = (BaseFeild == "GD") ? (gdJson, fiJson) : (BaseFeild == "FI") ? (fiJson, gdJson) : ("", "");
            string TrailingFeild = (BaseFeild == "GD") ? "FI" : "GD";

            List<ComparisonResult> result = new List<ComparisonResult>();
            #region old
            //foreach (var setting in ComparatorSetting)
            //{
            //    if ((setting.Entity1Key.Contains("[i]") && setting.Entity2Key.Contains("[i]")))
            //    {
            //        int count = 0;
            //        var dd = setting.Entity2Key.Substring(0, setting.Entity2Key.IndexOf("[i]"));
            //        var ss = setting.Entity1Key.Substring(0, setting.Entity1Key.IndexOf("[i]"));
            //        var TokensGd = GetJsonListValues(gdJson, setting.Entity1Key.Substring(0, setting.Entity1Key.IndexOf("[i]")));
            //        var TokensFi = GetJsonListValues(fiJson, setting.Entity2Key.Substring(0, setting.Entity2Key.IndexOf("[i]")));
            //        int totalCount = Math.Min(TokensFi.Count(), TokensGd.Count());

            //        for (int i = 0; i < TokensGd.Count(); i++)
            //        {
            //            bool isMatched = false;
            //            string[] GdParts = setting.Entity1Key.Split('.');
            //            string GdLastPart = GdParts[^1];
            //            string[] FiParts = setting.Entity2Key.Split('.');
            //            string FiLastPart = FiParts[^1];
            //            valuesGd = TokensGd[i][GdLastPart];
            //            for (int j = 0; j < TokensFi.Count(); j++)
            //            {
            //                valuesFi = TokensFi[j][FiLastPart];
            //                var comapreResult = CompareJsonTokens(valuesGd, valuesFi);
            //                if (comapreResult == 1)
            //                {
            //                    result.Add(new ComparisonResult
            //                    {
            //                        ComparisonType = $"Entity 1 index {i} with Entity 2 index {j} " + $"{setting.ValidationType} (Comparision Number: {count})",
            //                        Entity1Key = $"{setting.Entity1Key}",
            //                        Entity2Key = $"{setting.Entity2Key}",
            //                        Entity1Value = valuesGd?.ToString(),
            //                        Entity2Value = valuesFi?.ToString(),
            //                        Result = CompareJsonTokens(valuesGd, valuesFi),
            //                        RequestStatusId = ReqStatusId
            //                    });
            //                    isMatched = true;
            //                    break;
            //                }
            //                count++;
            //            }
            //            if (!isMatched)
            //            {
            //                result.Add(new ComparisonResult
            //                {
            //                    ComparisonType = $"Entity 1 index {i} " + $"{setting.ValidationType} (Does Not contain a Match)",
            //                    Entity1Key = $"{setting.Entity1Key}",
            //                    Entity2Key = $"{setting.Entity2Key}",
            //                    Entity1Value = valuesGd?.ToString(),
            //                    Entity2Value = /*valuesFi?.ToString()*/"",
            //                    RequestStatusId = ReqStatusId,
            //                    Result = 0
            //                });
            //            }
            //        }
            //    }
            //    else if (setting.Entity1Key.Contains(".Count()") && setting.Entity2Key.Contains(".Count()"))
            //    {
            //        valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key.Replace(".Count()", ""));
            //        valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key.Replace(".Count()", ""));
            //        result.Add(new ComparisonResult
            //        {
            //            ComparisonType = setting.ValidationType,
            //            Entity1Key = setting.Entity1Key,
            //            Entity2Key = setting.Entity2Key,
            //            Entity1Value = valuesGd?.Count().ToString(),
            //            Entity2Value = valuesFi?.Count().ToString(),
            //            RequestStatusId = ReqStatusId,
            //            Result = CompareJsonTokens(valuesGd, valuesFi)
            //        });
            //    }
            //    else
            //    {
            //        if (setting.IsSameEntity == 1)
            //        {
            //            valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key);
            //            valuesFi = GetKeyJsonGetter(gdJson, setting.Entity2Key);
            //        }
            //        else if (setting.IsSameEntity == 2)
            //        {

            //            valuesGd = GetKeyJsonGetter(fiJson, setting.Entity1Key);
            //            valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key);

            //        }
            //        else
            //        {
            //            valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key);
            //            valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key);
            //        }


            //        result.Add(new ComparisonResult
            //        {
            //            ComparisonType = setting.ValidationType,
            //            Entity1Key = setting.Entity1Key,
            //            Entity2Key = setting.Entity2Key,
            //            Entity1Value = valuesGd?.ToString(),
            //            Entity2Value = valuesFi?.ToString(),
            //            Result = CompareJsonTokens(valuesGd, valuesFi),
            //            RequestStatusId=ReqStatusId
            //        });
            //    }
            //}
            //    return result;


            //}

            #endregion

            foreach (var setting in ComparatorSetting)
            {
                List<JToken> test = new List<JToken>();
                try
                {
                    if ((setting.Entity1Key.Contains("itemInformation[i]") && setting.Entity2Key.Contains("itemInformation[i]")))
                    {
                        int count = 1;
                        var Tokens1 = GetJsonListValues(Value1Json, setting.Entity1Key.Substring(0, setting.Entity1Key.IndexOf("[i]")));
                        var Tokens2 = GetJsonListValues(Value2Json, setting.Entity2Key.Substring(0, setting.Entity2Key.IndexOf("[i]")));


                        int baseCounts = (BaseFeild == "GD") ? Tokens2.Count() : Tokens1.Count();
                        int tralingCounts = (BaseFeild == "GD") ? Tokens1.Count() : Tokens2.Count();

                        if (baseCounts == 2 && tralingCounts == 1)
                        {
                        }
                        //test = TokensGd;
                        for (int i = 0; i < baseCounts; i++)
                        {
                            List<ComparisonResult> ComparisonResult = new List<ComparisonResult>();              //ALL RESULTS LIST THAT ARE TO BE FILTRED AND HIGHEST PRIORITY RECORD MUST BE INSERT IN result LIST
                            List<priorityRecord> priorityList = new List<priorityRecord>();

                            for (int j = 0; j < tralingCounts; j++)
                            {
                                priorityRecord record = new priorityRecord();
                                bool isEntityMatched, isUomMatched;
                                int compareResult = 0;
                                values1 = Tokens1[i]["hsCode"];
                                values2 = Tokens2[j]["hsCode"];

                                record.startIndex = ComparisonResult.Count();

                                compareResult = CompareJsonTokens(values1, values2);
                                if (compareResult == 1)
                                {
                                    //HsCode Match

                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"{setting.ValidationType} Comparison > FI-{i + 1}:GD-{j + 1}",
                                        Entity1Key = $"{setting.Entity1Key}",
                                        Entity2Key = $"{setting.Entity2Key}",
                                        Entity1Value = values1?.ToString(),
                                        Entity2Value = values2?.ToString(),
                                        ComparisonName = setting.ValidationType,
                                        Result = CompareJsonTokens(values1, values2),
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    isEntityMatched = true;

                                    if (isEntityMatched)
                                    {
                                        values1 = Tokens1[i]["uom"];
                                        values2 = Tokens2[j]["uom"];

                                        compareResult = CompareJsonTokens(values1, values2);
                                        if (compareResult == 1)
                                        {
                                            //UOM Matched
                                            ComparisonResult.Add(new ComparisonResult
                                            {
                                                ComparisonType = $"UOM Comparison > FI-{i + 1}:GD-{j + 1}",
                                                Entity1Key = setting.Entity1Key.Replace("hsCode", "uom").Replace("[i]", $"[{i}]"),
                                                Entity2Key = setting.Entity2Key.Replace("hsCode", "uom").Replace("[i]", $"[{j}]"),
                                                Entity1Value = values1?.ToString(),
                                                Entity2Value = values2?.ToString(),
                                                ComparisonName = "UOM",
                                                Result = CompareJsonTokens(values1, values2),
                                                RequestStatusId = ReqStatusId,
                                                TenantId = AppSettings.TenantId,
                                            });
                                            isUomMatched = true;

                                            if (isUomMatched)
                                            {
                                                values1 = Tokens1[i]["quantity"];
                                                values2 = Tokens2[j]["quantity"];

                                                compareResult = CompareJsonTokens(values1, values2);
                                                if (compareResult == 1)
                                                {
                                                    //Quantity Matched

                                                    ComparisonResult.Add(new ComparisonResult
                                                    {
                                                        ComparisonType = $"Quantity Comparison > FI-{i + 1}:GD-{j + 1}",
                                                        Entity1Key = setting.Entity1Key.Replace("hsCode", "quantity").Replace("[i]", $"[{i}]"),
                                                        Entity2Key = setting.Entity2Key.Replace("hsCode", "quantity").Replace("[i]", $"[{j}]"),
                                                        Entity1Value = values1?.ToString(),
                                                        Entity2Value = values2?.ToString(),
                                                        ComparisonName = "Quantity",
                                                        Result = CompareJsonTokens(values1, values2),
                                                        RequestStatusId = ReqStatusId,
                                                        TenantId = AppSettings.TenantId,
                                                    });

                                                    //STATUS CODE 3
                                                    //PRIORITY CODE 3
                                                    record.priority = 3;
                                                    record.endIndex = ComparisonResult.Count();

                                                }
                                                else
                                                {
                                                    //Quantity is Not Matched

                                                    ComparisonResult.Add(new ComparisonResult
                                                    {
                                                        ComparisonType = $"Quantity Comparison > FI-{i + 1}:GD-{j + 1}",
                                                        ComparisonName = "Quantity",
                                                        Entity1Key = setting.Entity1Key.Replace("hsCode", "quantity").Replace("[i]", $"[{i}]"),
                                                        Entity2Key = setting.Entity2Key.Replace("hsCode", "quantity").Replace("[i]", $"[{j}]"),
                                                        Entity1Value = values1?.ToString(),
                                                        Entity2Value = values2?.ToString(),
                                                        Result = 0,
                                                        RequestStatusId = ReqStatusId,
                                                        TenantId = AppSettings.TenantId,
                                                    });

                                                    //PRIORITY CODE 2
                                                    record.priority = 2;
                                                    record.endIndex = ComparisonResult.Count();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // UOM Not Matched

                                            ComparisonResult.Add(new ComparisonResult
                                            {
                                                ComparisonType = $"UOM Comparison > FI-{i + 1}:GD-{j + 1}",
                                                Entity2Key = setting.Entity1Key.Replace("hsCode", "uom").Replace("[i]", $"[{i}]"),
                                                Entity1Key = setting.Entity2Key.Replace("hsCode", "uom").Replace("[i]", $"[{j}]"),
                                                Entity1Value = values1?.ToString(),
                                                Entity2Value = values2?.ToString(),
                                                ComparisonName = "UOM",
                                                Result = 0,
                                                RequestStatusId = ReqStatusId,
                                                TenantId = AppSettings.TenantId,
                                            });
                                            ComparisonResult.Add(new ComparisonResult
                                            {
                                                ComparisonType = $"Quantity Comparison > FI-{i + 1}:GD-{j + 1}",
                                                Entity1Key = setting.Entity1Key.Replace("hsCode", "quantity").Replace("[i]", $"[{i}]"),
                                                Entity2Key = setting.Entity2Key.Replace("hsCode", "quantity").Replace("[i]", $"[{i}]"),
                                                Entity1Value = Tokens1[i]["quantity"].ToString(),
                                                Entity2Value = Tokens2[j]["quantity"].ToString(),
                                                ComparisonName = "Quantity",
                                                Result = CompareJsonTokens(Tokens1[i]["quantity"], Tokens2[j]["quantity"]),
                                                RequestStatusId = ReqStatusId,
                                                TenantId = AppSettings.TenantId,
                                            });

                                            //PRIORITY CODE 1
                                            record.priority = 1;
                                            record.endIndex = ComparisonResult.Count();
                                        }
                                    }
                                }
                                else
                                {
                                    //HsCode Not Matched

                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"{setting.ValidationType} Comparison > FI-{i + 1}:GD-{j + 1}",
                                        Entity1Key = $"{setting.Entity1Key}",
                                        Entity2Key = $"{setting.Entity2Key}",
                                        Entity1Value = values1?.ToString(),
                                        Entity2Value = i < tralingCounts ? Tokens2[i]["hsCode"].ToString() : $"N/A",
                                        ComparisonName = setting.ValidationType,
                                        Result = 0,
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"UOM Comparison > FI-{i + 1}:GD-{j + 1}",
                                        Entity1Key = setting.Entity1Key.Replace("hsCode", "uom").Replace("[i]", $"[{i}]"),                //Data.iteminfo[i].uom
                                        Entity2Key = setting.Entity1Key.Replace("hsCode", "uom").Replace("[i]", $"[{j}]"),                //Data.iteminfo[i].uom
                                        Entity1Value = Tokens1[i]["uom"] != null ? Tokens1[i]["uom"].ToString() : "uom",
                                        Entity2Value = i < tralingCounts ? Tokens2[i]["uom"].ToString() : $"N/A",
                                        ComparisonName = "UOM",
                                        Result = i < tralingCounts ? CompareJsonTokens(Tokens1[i]["uom"], Tokens2[i]["uom"]) : 2,
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"Quantity Comparison > FI-{i + 1}:GD-{j + 1}",
                                        Entity1Key = setting.Entity1Key.Replace("hsCode", "quantity").Replace("[i]", $"[{i}]"),           //Data.iteminfo[i].quantity
                                        Entity2Key = setting.Entity1Key.Replace("hsCode", "quantity").Replace("[i]", $"[{j}]"),           //Data.iteminfo[i].quantity
                                        Entity1Value = Tokens1[i]["quantity"] != null ? Tokens1[i]["quantity"].ToString() : "quantity",
                                        Entity2Value = i < tralingCounts ? Tokens2[j]["quantity"].ToString() : $"N/A",
                                        ComparisonName = "Quantity",
                                        Result = i < tralingCounts ? CompareJsonTokens(Tokens1[i]["quantity"], Tokens2[i]["quantity"]) : 2,
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });

                                    //PRIORITY CODE 0
                                    record.priority = 0;
                                    record.endIndex = ComparisonResult.Count();
                                }

                                priorityList.Add(record);
                                count++;
                            }

                            //SELECTING ONLY HIGHEST PRIORITY ITEM ONLY
                            var highestPriorityItem = priorityList.MaxBy(x => x.priority);
                            if (highestPriorityItem != null)
                            {
                                var highestPriorityRecord = ComparisonResult.GetRange(highestPriorityItem.startIndex, highestPriorityItem.endIndex - highestPriorityItem.startIndex);
                                foreach (var item in highestPriorityRecord)
                                {
                                    if (highestPriorityRecord[0].Result == 0)
                                    {

                                    }
                                    result.Add(item);
                                }
                            }
                        }
                    }
                    else if (setting.Entity1Key.Contains(".Count()") && setting.Entity2Key.Contains(".Count()"))
                    {
                        values1 = GetKeyJsonGetter(Value1Json, setting.Entity1Key.Replace(".Count()", ""));
                        values2 = GetKeyJsonGetter(Value2Json, setting.Entity2Key.Replace(".Count()", ""));
                        result.Add(new ComparisonResult
                        {
                            ComparisonType = setting.ValidationType,
                            Entity1Key = setting.Entity1Key,
                            Entity2Key = setting.Entity2Key,
                            Entity2Value = values2?.Count().ToString(),
                            Entity1Value = values1?.Count().ToString(),
                            ComparisonName = setting.ValidationType,
                            Result = CompareJsonTokens(values1, values2),
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                        });
                    }
                    else
                    {
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


                        result.Add(new ComparisonResult
                        {
                            ComparisonType = setting.ValidationType,
                            Entity1Key = $"{setting.Entity1Key}-({(setting.IsSameEntity == 2 ? TrailingFeild : BaseFeild)})",
                            Entity2Key = $"{setting.Entity2Key}-({(setting.IsSameEntity == 1 ? BaseFeild : TrailingFeild)})",
                            Entity2Value = values2?.ToString(),
                            Entity1Value = values1?.ToString(),
                            ComparisonName = setting.ValidationType,
                            Result = CompareJsonTokens(values1, values2),
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                        });
                    }




                }

                catch (Exception)
                {
                    throw;

                }
            }
            return result;


        }
        public static int CompareJsonTokens(JToken token1, JToken token2, JToken? token3 = null)
        {
            try
            {
                if (token3 != null)
                {
                    if ((token1 != null || token2 != null) && ((token1.Type == JTokenType.Integer || token1.Type == JTokenType.Float) && (token2.Type == JTokenType.Integer || token2.Type == JTokenType.Float)))
                    {
                        double value1 = token1.Type == JTokenType.Float ? token1.Value<double>() : token1.Value<int>();
                        double value2 = token2.Type == JTokenType.Float ? token2.Value<double>() : token2.Value<int>();
                        double value3 = token3.Type == JTokenType.Float ? token2.Value<double>() : token3.Value<int>();

                        double unitPriceFi = value1 / value3; // invoice price/quantity
                        var Result = Math.Abs(unitPriceFi - value2);
                        if (Result < 1 && Result > -1)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }


                if (token1 == null || token2 == null)
                {
                    return 1;
                }
                if ((token1 != null || token2 != null) && ((token1.Type == JTokenType.Integer || token1.Type == JTokenType.Float) && (token2.Type == JTokenType.Integer || token2.Type == JTokenType.Float)))
                {
                    double value1 = token1.Type == JTokenType.Float ? token1.Value<double>() : token1.Value<int>();
                    double value2 = token2.Type == JTokenType.Float ? token2.Value<double>() : token2.Value<int>();
                    var x = Math.Abs(value1 - value2);

                    if (Math.Abs(value1 - value2) < 1 && Math.Abs(value1 - value2) > -1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (token1.Type == JTokenType.Array || token1.Type == JTokenType.Array)
                {
                    if (token1.Count() == token2.Count())
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (token1.Type == JTokenType.String)
                {
                    if (string.Compare(token1.ToString(), token2.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }

                else
                {
                    if (JToken.Equals(token1, token2))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            catch (JsonReaderException)
            {
                throw new ArgumentException("Invalid JSON input.");
            }
        }

        public static JToken? GetKeyJsonGetter(string jsonString, string key)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            JToken token = jsonObject.SelectToken(key);

            if (token != null)
            {
                return token;
            }
            else
            {
                return null;
            }

        }
        //public static int CompareJsonTokens(JToken token1, JToken token2)
        //{
        //    try
        //    {
        //        if (token1 == null || token2 == null)
        //        {
        //            return 1;
        //        }
        //        if ((token1 != null || token2 != null) && ((token1.Type == JTokenType.Integer || token1.Type == JTokenType.Float) && (token2.Type == JTokenType.Integer || token2.Type == JTokenType.Float)))
        //        {
        //            double value1 = token1.Type == JTokenType.Float ? token1.Value<double>() : token1.Value<int>();
        //            double value2 = token2.Type == JTokenType.Float ? token2.Value<double>() : token2.Value<int>();
        //            var x = Math.Abs(value1 - value2);

        //            if (Math.Abs(value1 - value2) < 1 && Math.Abs(value1 - value2) > -1)
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 0;
        //            }
        //        }
        //        else if (token1.Type == JTokenType.Array || token1.Type == JTokenType.Array)
        //        {
        //            if (token1.Count() == token2.Count())
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 0;
        //            }
        //        }
        //        else if (token1.Type == JTokenType.String)
        //        {
        //            if (string.Compare(token1.ToString(), token2.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 0;
        //            }

        //        }

        //        else
        //        {
        //            if (JToken.Equals(token1, token2))
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 0;
        //            }

        //        }
        //    }
        //    catch (JsonReaderException)
        //    {
        //        throw new ArgumentException("Invalid JSON input.");
        //    }
        //}

        public static List<JToken> GetJsonListValues(string jsonString, string key)
        {

            JObject jsonObject = JObject.Parse(jsonString);
            JToken token = jsonObject.SelectToken(key);

            if (token != null && token.Type == JTokenType.Array)
            {
                return token.ToList();
            }
            else
            {
                List<JToken> x = new List<JToken>();
                var data = jsonObject.SelectToken(key);
                x.Add(data);
                return x;
            }
        }
    }

}


