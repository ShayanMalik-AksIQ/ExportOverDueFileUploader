using ExportOverDueFileUploader.DBmodels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportOverDueFileUploader.Modles.JsonHelper;
using ExportOverDueFileUploader.Modles;

namespace ExportOverDueFileUploader.ValidateIqBizLogic
{
    public static class Comparison
    {
        public static List<ComparisonResult> CompareGdAndFi(string gdJson, string fiJson, List<ComparatorSetting> ComparatorSetting, long ReqStatusId)
        {

            JToken? valuesGd = null;
            JToken? valuesFi = null;
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
                    if ((setting.Entity1Key.Contains("[i]") && setting.Entity2Key.Contains("[i]")))
                    {
                        int count = 1;
                        var dd = setting.Entity2Key.Substring(0, setting.Entity2Key.IndexOf("[i]"));
                        var ss = setting.Entity1Key.Substring(0, setting.Entity1Key.IndexOf("[i]"));
                        var TokensGd = GetJsonListValues(gdJson, setting.Entity1Key.Substring(0, setting.Entity1Key.IndexOf("[i]")));
                        var TokensFi = GetJsonListValues(fiJson, setting.Entity2Key.Substring(0, setting.Entity2Key.IndexOf("[i]")));
                        int totalCount = Math.Min(TokensFi.Count(), TokensGd.Count());

                        test = TokensGd;
                        for (int i = 0; i < TokensGd.Count(); i++)
                        {
                            bool isMatched = false;
                            string[] GdParts = setting.Entity1Key.Split('.');
                            string GdLastPart = GdParts[^1];
                            string[] FiParts = setting.Entity2Key.Split('.');
                            string FiLastPart = FiParts[^1];
                            valuesGd = TokensGd[i][GdLastPart];

                            valuesGd = TokensGd[i]["hsCode"];

                            List<ComparisonResult> ComparisonResult = new List<ComparisonResult>();              //ALL RESULTS LIST THAT ARE TO BE FILTRED AND HIGHEST PRIORITY RECORD MUST BE INSERT IN result LIST
                            List<priorityRecord> priorityList = new List<priorityRecord>();

                            for (int j = 0; j < TokensFi.Count(); j++)
                            {
                                priorityRecord record = new priorityRecord();
                                bool isEntityMatched, isUomMatched, isQuantityMatched = false;
                                int comapreResult = 0;
                                valuesGd = TokensGd[i]["hsCode"];
                                valuesFi = TokensFi[j]["hsCode"];

                                record.startIndex = ComparisonResult.Count();

                                comapreResult = CompareJsonTokens(valuesGd, valuesFi);
                                if (comapreResult == 1)
                                {
                                    //HsCode Match

                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"{setting.ValidationType} Comparrision : {count}",
                                        Entity1Key = $"{setting.Entity1Key}",
                                        Entity2Key = $"{setting.Entity2Key}",
                                        Entity1Value = valuesGd?.ToString(),
                                        Entity2Value = valuesFi?.ToString(),
                                        ComparisonName = setting.ValidationType,
                                        Result = CompareJsonTokens(valuesGd, valuesFi),
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    isEntityMatched = true;

                                    if (isEntityMatched)
                                    {
                                        valuesGd = TokensGd[i]["uom"];
                                        valuesFi = TokensFi[j]["uom"];
                                        ,
                                        comapreResult = CompareJsonTokens(valuesGd, valuesFi);
                                        if (comapreResult == 1)
                                        {
                                            //UOM Matched
                                            ComparisonResult.Add(new ComparisonResult
                                            {
                                                ComparisonType = $"UOM Comparrision : {count}",
                                                Entity1Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{i}"),
                                                Entity2Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{j}"),
                                                Entity1Value = valuesGd?.ToString(),
                                                Entity2Value = valuesFi?.ToString(),
                                                ComparisonName = setting.ValidationType,
                                                Result = CompareJsonTokens(valuesGd, valuesFi),
                                                RequestStatusId = ReqStatusId,
                                                TenantId=AppSettings.TenantId,
                                            });
                                            isUomMatched = true;

                                            if (isUomMatched)
                                            {
                                                valuesGd = TokensGd[i]["quantity"];
                                                valuesFi = TokensFi[j]["quantity"];

                                                comapreResult = CompareJsonTokens(valuesGd, valuesFi);
                                                if (comapreResult == 1)
                                                {
                                                    //Quantity Matched

                                                    ComparisonResult.Add(new ComparisonResult
                                                    {
                                                        ComparisonType = $"Quantity Comparrision : {count}",
                                                        Entity1Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{i}"),
                                                        Entity2Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{j}"),
                                                        Entity1Value = valuesGd?.ToString(),
                                                        Entity2Value = valuesFi?.ToString(),
                                                        ComparisonName = setting.ValidationType,
                                                        Result = CompareJsonTokens(valuesGd, valuesFi),
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
                                                        ComparisonType = $"Quantity Comparrision : {count}",
                                                        ComparisonName = setting.ValidationType,
                                                        Entity1Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{i}"),
                                                        Entity2Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{j}"),
                                                        Entity1Value = valuesGd?.ToString(),
                                                        Entity2Value = valuesFi?.ToString(),
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
                                                ComparisonType = $"UOM Comparrision : {count}",
                                                Entity1Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{i}"),
                                                Entity2Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{j}"),
                                                Entity1Value = valuesGd?.ToString(),
                                                Entity2Value = valuesFi?.ToString(),
                                                ComparisonName = setting.ValidationType,
                                                Result = 0,
                                                RequestStatusId = ReqStatusId,
                                                TenantId = AppSettings.TenantId,
                                            });
                                            ComparisonResult.Add(new ComparisonResult
                                            {
                                                ComparisonType = $"Quantity Comparrision : {count}",
                                                Entity1Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{i}"),
                                                Entity2Key = $"N/A",
                                                Entity1Value = valuesGd?.ToString(),
                                                ComparisonName = setting.ValidationType,
                                                Entity2Value = valuesFi?.ToString(),
                                                Result = 2,
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
                                        ComparisonType = $"{setting.ValidationType} Comparrision : {count}",
                                        Entity1Key = $"{setting.Entity1Key}",
                                        Entity2Key = $"{setting.Entity2Key}",
                                        Entity1Value = valuesGd?.ToString(),
                                        Entity2Value = valuesFi?.ToString(),
                                        ComparisonName = setting.ValidationType,
                                        Result = 0,
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"UOM Comparrision : {count}",
                                        Entity1Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{i}"),                //Data.iteminfo[i].uom
                                        Entity2Key = setting.Entity1Key.Replace("hscode", "uom").Replace("i", $"{i}"),                //Data.iteminfo[i].uom
                                        Entity1Value = TokensGd[i]["uom"] != null ? TokensGd[i]["uom"].ToString() : "uom",
                                        Entity2Value = "N/A",
                                        ComparisonName = setting.ValidationType,
                                        Result = 2,
                                        RequestStatusId = ReqStatusId,
                                        TenantId = AppSettings.TenantId,
                                    });
                                    ComparisonResult.Add(new ComparisonResult
                                    {
                                        ComparisonType = $"Quantity Comparrision : {count}",
                                        Entity1Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{i}"),           //Data.iteminfo[i].quantity
                                        Entity2Key = setting.Entity1Key.Replace("hscode", "quantity").Replace("i", $"{i}"),           //Data.iteminfo[i].quantity
                                        Entity1Value = TokensGd[i]["quantity"] != null ? TokensGd[i]["quantity"].ToString() : "quantity",
                                        Entity2Value = "N/A",
                                        ComparisonName = setting.ValidationType,
                                        Result = 2,
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
                        valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key.Replace(".Count()", ""));
                        valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key.Replace(".Count()", ""));
                        result.Add(new ComparisonResult
                        {
                            ComparisonType = setting.ValidationType,
                            Entity1Key = setting.Entity1Key,
                            Entity2Key = setting.Entity2Key,
                            Entity1Value = valuesGd?.Count().ToString(),
                            Entity2Value = valuesFi?.Count().ToString(),
                            ComparisonName = setting.ValidationType,
                            Result = CompareJsonTokens(valuesGd, valuesFi),
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                        });
                    }
                    else
                    {
                        if (setting.IsSameEntity == 1)
                        {
                            valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key);
                            valuesFi = GetKeyJsonGetter(gdJson, setting.Entity2Key);
                        }
                        else if (setting.IsSameEntity == 2)
                        {

                            valuesGd = GetKeyJsonGetter(fiJson, setting.Entity1Key);
                            valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key);

                        }
                        else
                        {
                            valuesGd = GetKeyJsonGetter(gdJson, setting.Entity1Key);
                            valuesFi = GetKeyJsonGetter(fiJson, setting.Entity2Key);
                        }


                        result.Add(new ComparisonResult
                        {
                            ComparisonType = setting.ValidationType,
                            Entity1Key = setting.Entity1Key,
                            Entity2Key = setting.Entity2Key,
                            Entity1Value = valuesGd?.ToString(),
                            Entity2Value = valuesFi?.ToString(),
                            ComparisonName = setting.ValidationType,
                            Result = CompareJsonTokens(valuesGd, valuesFi),
                            RequestStatusId = ReqStatusId,
                            TenantId = AppSettings.TenantId,
                        });
                    }


                }

                catch (Exception ex)
                {


                }
            }
            return result;


        }
        public static int CompareJsonTokens(JToken token1, JToken token2)
        {
            try
            {
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


