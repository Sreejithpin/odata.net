﻿//---------------------------------------------------------------------
// <copyright file="DollarSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class DollarSegmentTests : KeyAsSegmentTest
    {
        public DollarSegmentTests(ITestOutputHelper helper)
            :base(helper)
        {
        }

        //--Test007-- [Fact]
        public void InsertEntityWithKeyValueSameAsNavigationPropertyName()
        {
            var contextWrapper = this.CreateWrappedContext();
            var newLogin = new Login { Username = "LastLogin" };

            contextWrapper.Context.AddToLogin(newLogin);
            contextWrapper.SaveChanges();

            var loginQuery = contextWrapper.CreateQuery<Login>("Login").Where(l => l.Username == "LastLogin").ToArray();
            Assert.True(newLogin == loginQuery.Single(), "Query result does not equal newly added login");
        }

        //--Test007-- [Fact]
        public void ClientExecuteEntitySetDerivedType()
        {
            var contextWrapper = this.CreateWrappedContext();
            var discontinuedProductQuery = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri + "/Product/$/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct")).ToArray();
        }

        //--Test007-- [Fact]
        public void ClientExecuteEntitySetDerivedTypeDollarSegmentAtEnd()
        {
            var contextWrapper = this.CreateWrappedContext();
            var discontinuedProductQuery = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri + "/Product/$/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct/$")).ToArray();
        }

        //--Test007-- [Fact]
        public void ClientExecuteProjectPropertyDefinedOnDerivedType()
        {
            var contextWrapper = this.CreateWrappedContext();
            var discontinuedProductDatesQuery = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri + "/Product/$/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct?$select=Discontinued")).ToArray();
        }

        //--Test007-- [Fact]
        public void ClientExecuteProjectPropertyDefinedOnDerivedTypeMultipleDollarSegments()
        {
            var contextWrapper = this.CreateWrappedContext();
            var discontinuedProductDatesQuery = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri + "/Product/$/$/$/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct?$select=Discontinued")).ToArray();
        }

        //--Test007-- [Fact]
        public void InvokeFeedBoundActionDefinedOnDerivedType()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.Execute(
                new Uri(this.ServiceUri + "/Person/$/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee/$/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries"), 
                "POST", 
                new OperationParameter[]
                    {
                        new BodyOperationParameter("n", 200),
                    }); 
        }

        //--Test007-- [Fact]
        public void InvokeEntryBoundActionDefinedOnDerivedType()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.Execute(
                new Uri(this.ServiceUri + "/Person/$/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee/-10/$/Microsoft.Test.OData.Services.AstoriaDefaultService.Sack"), 
                "POST", 
                new OperationParameter[] { }); 
        }

        //--Test007-- [Fact]
        public void InvokeActionDefinedOnDerivedTypeMultipleDollarSegments()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.Execute(
                new Uri(this.ServiceUri + "/Person/$/$/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee/$/$/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries"), 
                "POST", 
                new OperationParameter[]
                    {
                        new BodyOperationParameter("n", -200),
                    }); 
        }

        //--Test007-- [Fact]
        public void InvokeActionDefinedOnDerivedTypeDollarSegmentAtUriEnd()
        {
            var contextWrapper = this.CreateWrappedContext();
            
            // Actions must be leaf segments, we do not allow anything to follow them
            contextWrapper.Execute(
                new Uri(this.ServiceUri + "/Person/$/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee/$/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries/$"), 
                "POST", 
                new OperationParameter[]
                {
                    new BodyOperationParameter("n", -200),
                }); 
        }
    }
}
