﻿//---------------------------------------------------------------------
// <copyright file="TestContainerBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData;

    public class TestContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection services = new ServiceCollection();

        public IContainerBuilder AddService(
            Microsoft.OData.ServiceLifetime lifetime,
            Type serviceType,
            Type implementationType)
        {
            Debug.Assert(serviceType != null, "serviceType != null");
            Debug.Assert(implementationType != null, "implementationType != null");

            services.Add(new ServiceDescriptor(
                serviceType, implementationType, TranslateServiceLifetime(lifetime)));

            return this;
        }

        public IContainerBuilder AddService(
            Microsoft.OData.ServiceLifetime lifetime,
            Type serviceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            Debug.Assert(serviceType != null, "serviceType != null");
            Debug.Assert(implementationFactory != null, "implementationFactory != null");

            services.Add(new ServiceDescriptor(
                serviceType, implementationFactory, TranslateServiceLifetime(lifetime)));

            return this;
        }

        public IServiceProvider BuildContainer()
        {
            return services.BuildServiceProvider();
        }

        private static Extensions.DependencyInjection.ServiceLifetime TranslateServiceLifetime(
            Microsoft.OData.ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case Microsoft.OData.ServiceLifetime.Scoped:
                    return Extensions.DependencyInjection.ServiceLifetime.Scoped;
                case Microsoft.OData.ServiceLifetime.Singleton:
                    return Extensions.DependencyInjection.ServiceLifetime.Singleton;
                default:
                    return Extensions.DependencyInjection.ServiceLifetime.Transient;
            }
        }
    }
}