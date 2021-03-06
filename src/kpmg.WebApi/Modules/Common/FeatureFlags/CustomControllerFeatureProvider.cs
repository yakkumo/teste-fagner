#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

#endregion

namespace kpmg.WebApi.Modules.Common.FeatureFlags
{
    /// <summary>
    ///     Custom Controller Feature Provider.
    /// </summary>
    public sealed class CustomControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IFeatureManager _featureManager;

        /// <summary>
        ///     Custom Controller Feature Provider constructor.
        /// </summary>
        public CustomControllerFeatureProvider(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        /// <summary>
        ///     Populate Features.
        /// </summary>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            for (var i = feature.Controllers.Count - 1; i >= 0; i--)
            {
                Type controller = feature.Controllers[i].AsType();
                foreach (CustomAttributeData customAttribute in controller.CustomAttributes)
                {
                    if (customAttribute.AttributeType.FullName != typeof(FeatureGateAttribute).FullName) continue;

                    var constructorArgument = customAttribute.ConstructorArguments.First();
                    if (!(constructorArgument.Value is IEnumerable arguments)) continue;

                    foreach (var argumentValue in arguments)
                    {
                        var typedArgument = (CustomAttributeTypedArgument) argumentValue!;
                        var typedArgumentValue = (CustomFeature) (int) typedArgument.Value!;
                        var isFeatureEnabled = _featureManager
                            .IsEnabledAsync(typedArgumentValue.ToString())
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        if (!isFeatureEnabled) feature.Controllers.RemoveAt(i);
                    }
                }
            }
        }
    }
}