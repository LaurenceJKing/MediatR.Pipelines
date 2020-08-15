# MediatR.Pipelines
Helpful MediatR pipelines.

# Pipelines
- [Authorization](#Authorization)
- [Caching](#Caching)
- [Validation](#Validation)
- [Localization](#Localization)
- [Logging](#Logging)

# Authorization

The Authorization behavior uses an ```IAuthorizationService``` to determine whether the current user has permission to access the response.

You must provide an ```AuthorizationPolicy```, a policy name (a ```string```), an ```IEnumerable<IAuthorizationRequirement>``` or a single ```IAuthoizationRequirement``` to authorize against.

## Install
```PM> Install-Package MediatR.Pipelines.Authorization```

# Caching

The caching behavior will cache the response using an ```IMemoryCache```. Requests must implement the ```IIdempotentRequest``` interface and responses will only be cached if the ```CaheKey``` property on the request is not null.

## Install
```PM> Install-Package MediatR.Pipelines.Caching```

# Validation

The validation behavior uses [FluentValidation](https://fluentvalidation.net) to validate the request. If a request is not valid then a ```ValidationException``` is thrown.

## Install

```PM> Install-Package MediatR.Pipelines.FluentValidation```

# Localization

The localization behavior scans the response, looking for settable string properties decorated with a ```LocalizeAttribute```. It will then get the localized string and inject it into that property.

## Install

```PM> Install-Package MediatR.Pipelines.Localization```

# Logging

The logging behavior will log any exceptions thrown previously in the pipeline. 

## Install

```PM> Install-Package MediatR.Pipelines.Logging```
