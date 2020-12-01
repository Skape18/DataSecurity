using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;

namespace Lab5_6_7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols = SslProtocols.Tls13;
                        });
                        kestrelOptions.Listen(IPAddress.Any, 5001, listenOptions =>
                        {
                            var allowedCipherAlgorithms = new[] { CipherAlgorithmType.Aes128, CipherAlgorithmType.Aes256 };
                            var allowedHashTypes = new[] { HashAlgorithmType.Sha256, HashAlgorithmType.Sha384, HashAlgorithmType.Sha512 };

                            listenOptions.Use((context, next) =>
                            {
                                var tlsFeature = context.Features.Get<ITlsHandshakeFeature>();
                                if (allowedCipherAlgorithms.Contains(tlsFeature.CipherAlgorithm) &&
                                    allowedHashTypes.Contains(tlsFeature.HashAlgorithm))
                                {
                                    return next();
                                }

                                throw new NotSupportedException($"Prohibited cipher or hash: {tlsFeature.HashAlgorithm} {tlsFeature.CipherAlgorithm}");
                            });

                            /* works only for linux
                            listenOptions.OnAuthenticate = (context, sslOptions) =>
                            {
                                sslOptions.CipherSuitesPolicy = new CipherSuitesPolicy(
                                    new[]
                                    {
                                            TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                                            TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                                    });
                            }; */
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
