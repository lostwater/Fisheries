using Fisheries.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Fisheries
{
    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return;

            string clientId;
            string clientSecret;

            if (context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                UserManager<IdentityUser> userManager =
                    context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                ApplicationDbContext dbContext =
                    context.OwinContext.Get<ApplicationDbContext>();

                try
                {
                    var client = new Client();
                    //Client client = await dbContext
                    //    .Clients
                    //    .FirstOrDefaultAsync(clientEntity => clientEntity.Id == clientId);

                    if (client != null &&
                        userManager.PasswordHasher.VerifyHashedPassword(
                            client.ClientSecretHash, clientSecret) == PasswordVerificationResult.Success)
                    {
                        // Client has been verified.
                        context.OwinContext.Set<Client>("oauth:client", client);
                        context.Validated(clientId);
                    }
                    else
                    {
                        // Client could not be validated.
                        context.SetError("invalid_client", "Client credentials are invalid.");
                        context.Rejected();
                    }
                }
                catch
                {
                    // Could not get the client through the IClientManager implementation.
                    context.SetError("server_error");
                    context.Rejected();
                }
            }
            else
            {
                // The client credentials could not be retrieved.
                context.SetError(
                    "invalid_client",
                    "Client credentials could not be retrieved through the Authorization header.");

                context.Rejected();
            }
        }

        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (var _repo = new AuthRepository())
            {
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "用户名或者密码不正确");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", "user"));

                context.Validated(identity);
            }
            return;

            //Client client = context.OwinContext.Get<Client>("oauth:client");
            var client = new Client();
            if (client.AllowedGrant == OAuthGrant.ResourceOwner)
            {
                // Client flow matches the requested flow. Continue...
                UserManager<IdentityUser> userManager =
                    context.OwinContext.GetUserManager<UserManager<IdentityUser>>();

                IdentityUser user;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user.
                    context.SetError("server_error");
                    context.Rejected();

                    // Return here so that we don't process further. Not ideal but needed to be done here.
                    return;
                }

                if (user != null)
                {
                    try
                    {
                        // User is found. Signal this by calling context.Validated
                        ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                            user,
                            DefaultAuthenticationTypes.ExternalBearer);

                        context.Validated(identity);
                    }
                    catch
                    {
                        // The ClaimsIdentity could not be created by the UserManager.
                        context.SetError("server_error");
                        context.Rejected();
                    }
                }
                else
                {
                    // The resource owner credentials are invalid or resource owner does not exist.
                    context.SetError(
                        "access_denied",
                        "The resource owner credentials are invalid or resource owner does not exist.");

                    context.Rejected();
                }
            }
            else
            {
                // Client is not allowed for the 'Resource Owner Password Credentials Grant'.
                context.SetError(
                    "invalid_grant",
                    "Client is not allowed for the 'Resource Owner Password Credentials Grant'");

                context.Rejected();
            }
        }
    }
}