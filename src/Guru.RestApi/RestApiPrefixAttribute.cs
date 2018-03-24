using System;
using Guru.AspNetCore.Attributes;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.RestApi.Abstractions;

namespace Guru.RestApi
{
    public class RestApiPrefixAttribute : HandlingBeforeAttribute
    {
        private readonly static IAuthManager _AuthManager;

        static RestApiPrefixAttribute()
        {
            _AuthManager = DependencyContainer.Resolve<IAuthManager>();
        }

        private AuthValidatingEnum _AuthValidatingEnum = AuthValidatingEnum.Ignore;

        public RestApiPrefixAttribute(AuthValidatingEnum authValidatingEnum)
        {
            _AuthValidatingEnum = authValidatingEnum;
        }

        public override HandlingResult Handle(string id, Type returnType, params object[] args)
        {
            var authValidatingResult = AuthValidate(returnType, args);
            if (authValidatingResult != null)
            {
                return authValidatingResult;
            }

            return HandlingResult.Succeed();
        }

        private HandlingResult AuthValidate(Type returnType, params object[] args)
        {
            if (_AuthValidatingEnum == AuthValidatingEnum.Required)
            {
                if (!args.HasLength())
                {
                    return CreateAuthFailureResult(returnType);
                }

                var request = args.FirstOrDefault(x => x is IAuthRestApiRequest);
                if (request == null)
                {
                    return CreateAuthFailureResult(returnType);
                }

                var result = _AuthManager.Validate(request as IAuthRestApiRequest);

                if (!result)
                {
                    return CreateAuthFailureResult(returnType);
                }
            }
            else if (_AuthValidatingEnum == AuthValidatingEnum.Optional)
            {
                if (!args.HasLength())
                {
                    return null;
                }

                var request = args.FirstOrDefault(x => x is IAuthRestApiRequest);
                if (request == null)
                {
                    return null;
                }

                var result = _AuthManager.Validate(request as IAuthRestApiRequest);

                if (!result)
                {
                    return null;
                }
            }

            return null;
        }

        private HandlingResult CreateAuthFailureResult(Type returnType)
        {
            if (typeof(IRestApiResponse).IsAssignableFrom(returnType))
            {
                var response = returnType.CreateInstance() as IRestApiResponse;
                response.Head = new RestApiResponseHead() { Status = -99, Message = "auth is not valid." };
                return HandlingResult.Fail(response);
            }
            else
            {
                return HandlingResult.Fail(null);
            }
        }
    }
}