# Identity service

Of course there are some full-fledged Authentication services we can use like `IdentityServer4` but they do all the magic behind the curtain.
But I want to show some internal "kitchen" of Authentication process. Like JWT token creation and using third-party identity provider like Google or Facebook.

So we do all the heavy lifting ourselves :)

Great JWT Authentication Tutorial is here [.NET 6.0 - JWT Authentication Tutorial with Example API](https://jasonwatmore.com/post/2021/12/14/net-6-jwt-authentication-tutorial-with-example-api)

Thanks Nick Chapsas for [great Youtube video](https://www.youtube.com/watch?v=I2PChWTwmM8&t=1318s) with explanations how to use third-party identity provider

We'll use AWS Cognito service but only for storing users in user pool.

Also we'll implement Refresh token and add some extra security - Refresh Token Automatic Reuse Detection
[What Are Refresh Tokens and How to Use Them Securely](https://auth0.com/blog/refresh-tokens-what-are-they-and-when-to-use-them/)

## Using RSA for JWT

In order to use JWT we need to sign token and then validate signature.
We can use plain SymmetricSecurityKey - so it is an arbitrary cryptic string that is used both for signing and validation.

In our case `Identity` service signs token and `API Gateway` validates it. So  both services have this SymmetricSecurityKey.
Well - at least in theory API Gateway has all information to sign token. But we do not want it.

So we can add more security to this solution - use Asymmetric algorithm to sign token.

First we need to create key pair

```bash
    openssl genrsa -out private_key.pem 2048
    openssl rsa -pubout -in private_key.pem -out public_key.pem
```

We'll get files with private and public keys

- `private_key.pem`
- `public_key.pem`

File looks like this

```text
-----BEGIN RSA PRIVATE KEY-----
MII...
-----END RSA PRIVATE KEY-----
```

To use private key in our configuration file we have to remove those comment lines and all carriage returns, so be sure to make it one-liner when storing them in appsettings.json, vault or env variable.
The same for public key.

In `Identity` service we'll use PrivateKey.
In `API Gateway` we'll store PublicKey

Create `JWTRSAPrivateKey` variable and import it

```csharp
            var privateKey = Convert.FromBase64String(_appSettings.JWTRSAPrivateKey);

            RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);

            signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
```

then use it creating `SecurityTokenDescriptor`

API Gateway:
Create `JWTRSAPublicKey` variable and import it

```csharp
    var publicKey = Convert.FromBase64String(builder.Configuration["AppSettings:JWTRSAPublicKey"]);

    RSA rsa = RSA.Create();
    rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
    securityKey = new RsaSecurityKey(rsa);
```

then use it creating `TokenValidationParameters`

That simple!
Now only `Identity` service can sign tokens and this service is in our private network. And PublicKey is not a sensitive information and could be placed on a public facing service.

## What is out of scope

Some "must have in production" features like Email verification, Forget password are left out of scope.
