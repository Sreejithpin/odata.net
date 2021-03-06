﻿//---------------------------------------------------------------------
// <copyright file="FixupNoSetAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Tells enricher not to generate Set for EntityType or Association
    /// </summary>
    public class FixupNoSetAnnotation : TagAnnotation
    {
    }
}
