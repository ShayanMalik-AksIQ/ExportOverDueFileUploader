﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ExportOverDueFileUploader.DBmodels;

public partial class GoodsDeclaration
{
    public long Id { get; set; }

    public string? ESB_LOG_ID { get; set; }

    public string? MESSAGE_ID { get; set; }

    public DateTime? TRANSMISSION_DATETIME { get; set; }

    public string? MESSAGE_TYPE { get; set; }

    public string? DIRECTION { get; set; }

    public string? TRANSACTION_ID { get; set; }

    public string? PROTOCOL { get; set; }

    public string? STATUS_CODE { get; set; }

    public string? PAYLOAD { get; set; }

    public string? CREATED_DATE { get; set; }

    public string? CREATED_BY { get; set; }

    public string? MESSAGE { get; set; }

    public int TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeleterUserId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public string? DESTINATION_ADDRESS { get; set; }

    public string? SOURCE_ADDRESS { get; set; }

    public string? LstfinInsUniqueNumbers { get; set; }

    public string? itemInformationJson { get; set; }

    public string? blDate { get; set; }

    public string? gdNumber { get; set; }

    public float? totalDeclaredValue { get; set; }

    public string? gdStatus { get; set; }

    public DateTime? GDDate { get; set; }

    public string? CurrencyCode { get; set; }

    public string? ShipmentCity { get; set; }

    public string? ShipmentDate { get; set; }

    public float? exchangeRate { get; set; }

    public string? consigneeName { get; set; }

    public string? finInsUniqueNumber { get; set; }

    public string? modeOfPayment { get; set; }

    public long? FileAuditId { get; set; }


    public virtual ICollection<GD_FI_Link> GD_FI_Links { get; set; } = new List<GD_FI_Link>();
    [NotMapped]
    public List<FiNumberAndMode> FiNumbersAndModes
    {
        get
        {
            List<FiNumberAndMode> fiNumberAndModes = new List<FiNumberAndMode>();
            if (LstfinInsUniqueNumbers != null)
            {

                foreach (var fi in LstfinInsUniqueNumbers.Split(","))
                {
                    if(fi.StartsWith("("))
                    {
                        fiNumberAndModes.Add(new FiNumberAndMode { ModeOFPayment = fi.Trim('(', ')') });
                    }
                    else
                    {

                    fiNumberAndModes.Add(Parse(fi));
                    }
                }

            }
            return fiNumberAndModes;
        }
    }

    [NotMapped]
    public DateTime? BLDateVale
    {
        get
        {
            if (!blDate.IsNullOrEmpty())
            {

                int a = Convert.ToInt16(blDate.Substring(6, 2));
                int b = Convert.ToInt16(blDate.Substring(4, 2));
                int d = Convert.ToInt16(blDate.Substring(0, 4));
                return new DateTime(d, b, a);
            }
            else { return null; }
        }
    }

    private static FiNumberAndMode Parse(string input) =>
    new FiNumberAndMode
    {
        FiNumber = Regex.Match(input, @"^(?<FiNumber>[\w-]+)(\((?<Value>\d+)\))?$").Groups["FiNumber"].Value,
        ModeOFPayment = Regex.Match(input, @"^(?<FiNumber>[\w-]+)(\((?<Value>\d+)\))?$").Groups["Value"]?.Value ?? null

    };
}


public class FiNumberAndMode
{
    public string FiNumber { get; set; } // JSB-EXP-015272-11092023
    public string ModeOFPayment { get; set; } // 308


}

