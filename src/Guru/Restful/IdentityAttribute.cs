using System;
using Guru.AspNetCore.Attributes;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Restful.Abstractions;

namespace Guru.Restful
{
    public class IdentityAttribute : HandlingBeforeAttribute
    {
        private readonly IIdentityValidator _IdentityValidator;

        private IdentityValidating _IdentityValidating = IdentityValidating.Ignore;

        public IdentityAttribute(IdentityValidating validating)
        {
            _IdentityValidating = validating;
            _IdentityValidator = DependencyContainer.Resolve<IIdentityValidator>();
        }

        public override HandlingResult Handle(string id, Type returnType, params object[] args)
        {
            if (_IdentityValidating == IdentityValidating.Ignore)
            {
                return HandlingResult.Succeed();
            }
            else
            {
                var validatingResult = false;
                var request = args.FirstOrDefault(x => x is RequestBase) as RequestBase;
                if (request != null)
                {
                    validatingResult = _IdentityValidator.Validate(request.Head);
                }
                if (_IdentityValidating == IdentityValidating.Optional)
                {
                    return HandlingResult.Succeed();
                }
                // Required
                if (validatingResult)
                {
                    return HandlingResult.Succeed();
                }
                else
                {
                    if (typeof(ResponseBase).IsAssignableFrom(returnType))
                    {
                        return HandlingResult.Fail((returnType.CreateInstance() as ResponseBase).Illegal());
                    }
                    else
                    {
                        return HandlingResult.Fail(null);
                    }
                }
            }
        }
    }
}