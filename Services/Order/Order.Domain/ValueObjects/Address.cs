﻿using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Order.Domain.ValueObjects;

[Owned]
public class Address : ValueObject
{
    private static readonly string ZipCodePattern = @"^[A-Za-z]{2}\d{4}$"; // AZ1001 or az1001

    public string City { get; private set; }
    public string District { get; private set; }
    public string Street { get; private set; }
    public string ZipCode { get; private set; }

    public Address()
    {
        
    }

    public Address(string city, string district, string street, string zipCode)
    {
        Guard.Against.NullOrEmpty(city, nameof(city));
        Guard.Against.NullOrEmpty(district, nameof(district));
        Guard.Against.NullOrEmpty(street, nameof(street));
        SetZipCode(zipCode);

        City = city;
        District = district;
        Street = street;
        ZipCode = zipCode;
    }

    public void SetZipCode(string zipCode)
    {
        Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));

        if (Regex.IsMatch(zipCode, ZipCodePattern))
        {
            ZipCode = zipCode.ToUpper();
        }
        else
        {
            throw new ArgumentException("Zip code is invalid");
        }
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return District;
        yield return Street;
        yield return ZipCode;
    }
}
