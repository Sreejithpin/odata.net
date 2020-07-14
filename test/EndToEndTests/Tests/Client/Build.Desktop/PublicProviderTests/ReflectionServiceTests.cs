﻿//---------------------------------------------------------------------
// <copyright file="ReflectionServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PublicProviderTests
{
    using System;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PublicProviderReflectionServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using HttpWebRequestMessage = Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage;
    using Xunit.Abstractions;
    using Xunit;

    public class ReflectionServiceTests : EndToEndTestBase
    {
        public ReflectionServiceTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.PublicProviderReflectionService, helper)
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        //--Test007-- [Fact]
        public void ValidReadReflectionEntity()
        {
            var context = CreateWrappedContext<DefaultContainer>().Context;

            //Verify feed is working
            Assert.NotNull(context.Car.ToArray());
            //Verify filter is working
            Assert.NotNull(context.Car.Where(c => c.Description == Guid.NewGuid().ToString()).ToArray());
            //Verify paging and count is working
            Assert.True(context.Car.Count() == 10);
            //Verify navigation link is working
            Assert.NotNull(context.PersonMetadata.Expand("Person").FirstOrDefault().Person);
        }

        //--Test007-- [Fact]
        public void ValidCUDReflectionEntity()
        {
            string desc = Guid.NewGuid().ToString();
            var context = CreateWrappedContext<DefaultContainer>().Context;
            var binaryTestData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            //create
            var car = new Car { Description = desc, VIN = 1};
            context.AddToCar(car);
            context.SetSaveStream(car, new MemoryStream(new byte[] { 66, 67 }), true, "text/plain", "slug");
            var memoryStream = new MemoryStream(binaryTestData);
            context.SetSaveStream(car, "Video", memoryStream, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            var memoryStream2 = new MemoryStream(binaryTestData);
            context.SetSaveStream(car, "Photo", memoryStream2, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            context.SaveChanges();
            Assert.True(car.VIN != 0);
            Assert.Equal(1, context.Car.Where(c => c.Description == desc).Count());

            //update
            string newdesc = Guid.NewGuid().ToString();
            car.Description = newdesc;
            context.UpdateObject(car);
            context.SaveChanges();
            Assert.Equal(1, context.Car.Where(c => c.Description == newdesc).Count());

            //delete
            context.DeleteObject(car);
            context.SaveChanges();
            Assert.Equal(0, context.Car.Where(c => c.Description == newdesc).Count());
        }

        [Fact(Skip= "VSUpgrade19 - ContextHelper issue")]
        public void ValidServiceOperationReflectionEntity()
        {
            var context = CreateWrappedContext<DefaultContainer>().Context;

            int count = context.GetPersonCount();
            Assert.Equal(count, context.Person.Count());

            var expectedPerson = context.Person.FirstOrDefault();

            var person = context.GetPersonByExactName(expectedPerson.Name);
            Assert.Equal(expectedPerson.PersonId, person.PersonId);

            var persons = context.GetPersonsByName(expectedPerson.Name.Substring(0, 3)).ToArray();
            Assert.True(persons.Any());
            Assert.Contains(persons, p => p.PersonId == expectedPerson.PersonId);
        }
#endif

        //--Test007-- [Fact]
        public void ValidMetadata()
        {
            var message = new HttpWebRequestMessage(new Uri(ServiceUri + "$metadata"));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                var model = messageReader.ReadMetadataDocument();
                var container = model.EntityContainer;

                // Verify all the entities are exposed
                var entities = container.Elements.Where(e => e is IEdmEntitySet).ToArray();
                Assert.Equal(24, entities.Count());

                // Verify all the service operations are exposed
                var functions = container.Elements.Where(e => e is IEdmOperationImport).ToArray();
                Assert.Equal(3, functions.Count());
            }
        }

        //--Test007-- [Fact]
        public void ValidServiceDocument()
        {
            var metadataMessage = new HttpWebRequestMessage(new Uri(ServiceUri + "$metadata"));
            metadataMessage.SetHeader("Accept", MimeTypes.ApplicationXml);

            var metadataMessageReader = new ODataMessageReader(metadataMessage.GetResponse());
            var model = metadataMessageReader.ReadMetadataDocument();

            var message = new HttpWebRequestMessage(ServiceUri);
            message.SetHeader("Accept", MimeTypes.ApplicationJson);
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceUri };

            using (var messageReader = new ODataMessageReader(message.GetResponse(), readerSettings, model))
            {
                var workspace = messageReader.ReadServiceDocument();
                Assert.Equal(24, workspace.EntitySets.Count());
            }
        }
    }
}
