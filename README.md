# .NET 8 Blazor Server API Authentication with PostgreSQL
Blazor Server, a part of the ASP.NET Core framework, allows developers to build interactive web UIs using C# instead of JavaScript. With the release of .NET 8, Blazor Server has introduced several enhancements, particularly in the realm of authentication and API integration. This article delves into setting up authentication for a Blazor Server application using JWT (JSON Web Tokens) and PostgreSQL as the database.

# Project Setup
To get started, we need two main projects: the Blazor Server app (AdminPortal) and the API project (API). Both projects target .NET 8.0 and utilize several NuGet packages to handle authentication, database interactions, and more.

# NuGet Packages Used
AdminPortal (Blazor Server App)
1. Microsoft.AspNetCore.Authentication.JwtBearer (8.0.8): This package is used to authenticate users using JWT tokens.
2. Microsoft.AspNetCore.Components.WebAssembly.Authentication (8.0.8): Provides authentication support for Blazor WebAssembly applications.
3. System.Net.Http.Json (8.0.0): Simplifies the process of sending and receiving JSON data over HTTP.

# API Project
1. Microsoft.AspNetCore.Authentication.JwtBearer (6.0.0): Used for JWT authentication.
2. Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.8): Integrates ASP.NET Core Identity with Entity Framework Core.
3. Microsoft.AspNetCore.Mvc.NewtonsoftJson (8.0.8): Adds support for using Newtonsoft.Json with ASP.NET Core MVC.
4. Microsoft.EntityFrameworkCore.Design (8.0.8): Provides design-time utilities for Entity Framework Core.
5. Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4): Entity Framework Core provider for PostgreSQL.
6. Swashbuckle.AspNetCore (6.7.3): Generates Swagger documents for APIs built with ASP.NET Core.
7. System.IdentityModel.Tokens.Jwt (8.0.2): Provides support for creating and validating JWT tokens.

# Conclusion
By leveraging .NET 8, Blazor Server, and PostgreSQL, you can build robust and secure web applications. The combination of JWT authentication and PostgreSQL ensures that your application is both secure and scalable. The NuGet packages used in this setup provide all the necessary tools to implement authentication and database interactions seamlessly. With these configurations, you are well on your way to building a modern web application with .NET 8 and Blazor Server.

For Additional Resources feel free to: 
- Visit [Kali Linux Code](https://kalilinuxcode.com/) For more information, tutorials, and code samples.

  ![](https://github.com/ssssage/AdminPortalElixirHand/blob/master/images/Full%20Featured%20CRUD%20App%20based%20on%20Blazor%20.NET%208.png)

