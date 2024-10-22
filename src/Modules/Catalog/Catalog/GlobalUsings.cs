global using Shared.DDD;
global using Catalog.Products.Models;
global using Catalog.Products.Events;
global using System.Reflection;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Catalog.Data;
global using Shared.Data.Seed;
global using Catalog.Contracts.Products.Dtos;
global using SharedContracts.CQRS;
global using Mapster;
global using MediatR;

global using Carter;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using FluentValidation;

global using Catalog.Products.Exceptions;
global using Catalog.Contracts.Products.Features.GetProductById;
