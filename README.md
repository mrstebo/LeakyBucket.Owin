# LeakyBucket.Owin

Owin middleware that implements the leaky bucket algorithm for request throttling.

[![Build status](https://ci.appveyor.com/api/projects/status/hwebgi9ilfv5sx71/branch/master?svg=true)](https://ci.appveyor.com/project/mrstebo/leakybucket-owin/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/mrstebo/LeakyBucket.Owin/badge.svg?branch=master)](https://coveralls.io/github/mrstebo/LeakyBucket.Owin?branch=master)
[![NuGet](http://img.shields.io/nuget/v/LeakyBucket.Owin.svg?style=flat)](https://www.nuget.org/packages/LeakyBucket.Owin/)

This package is available via install the [NuGet](https://www.nuget.org/packages/LeakyBucket.Owin):

```powershell
Install-Package LeakyBucket.Owin
```

Then, you can enable LeakyBucket rate limiting by adding the `LeakyBucketMiddleware` to the Owin app:

```cs
using System;
using LeakyBucket.Owin;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyWebApp.Startup))]

namespace MyWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseLeakyBucket(new LeakyBucketConfiguration
            {
              MaxNumberOfRequests = 100,
              RefreshRate = TimeSpan.FromSeconds(1)
            });
        }
    }
}
```

## How it works

1. When a request comes in it will extract the `IClientIdentifier` from the `IOwinContext` with the function specified in the configuration.
2. It will check for any requests that have passed the specified `RefreshRate` and remove them from the `IRequestStore`.
3. It will then add the new request to the `IRequestStore`.
4. It will check how many requests are remaining.
5. If it exceeds the `MaxNumberOfRequests` then it will return a **429 Too Many Requests** status code, otherwise it will continue on with the request.


## Configuration

The `LeakyBucketConfiguration` can specifiy an `IRequestStore`, that is set to the `DefaultRequestStore` by default, which is responsible for holding the current number of requests for each client.

The `LeakyBucketConfiguration` can specifies a function that determines what a client is. By default it creates a `DefaultClientIdentifier` that holds a reference to the remote address.
