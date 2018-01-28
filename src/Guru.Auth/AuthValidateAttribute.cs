using Guru.AspNetCore.Attributes;
using Guru.Auth.Abstractions;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using System;

namespace Guru.Auth
{
    public class AuthValidateAttribute : HandlingBeforeAttribute
    {
        private static IAuthValidator _Validator;

        static AuthValidateAttribute()
        {
            _Validator = DependencyContainer.Resolve<IAuthValidator>();
        }

        public override HandlingResult Handle(string id, Type returnType, params object[] args)
        {
            if (!args.HasLength() || !args.Exists(x => x is IAuthRequest))
            {
                return CreateAuthResponse(returnType);
            }

            var authRequest = args.FirstOrDefault(x => x is IAuthRequest) as IAuthRequest;
            _Validator.Validate(authRequest);

            if (string.IsNullOrEmpty(authRequest.Head.Uid))
            {
                return CreateAuthResponse(returnType);
            }

            return HandlingResult.Succeed();
        }

        private HandlingResult CreateAuthResponse(Type returnType)
        {
            if (typeof(IAuthResponse).IsAssignableFrom(returnType))
            {
                var response = returnType.CreateInstance() as IAuthResponse;
                response.Head = new AuthResponseHeader()
                {
                    Message = "auth is not valid.",
                };

                return HandlingResult.Fail(response);
            }
            else
            {
                return HandlingResult.Fail(null);
            }
        }
    }
}