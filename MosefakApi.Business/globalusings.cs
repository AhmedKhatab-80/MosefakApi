global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using MosefakApp.Core.Dtos.Authentication;
global using MosefakApp.Core.Dtos.Authentication.Requests;
global using MosefakApp.Core.Dtos.Authentication.Responses;
global using MosefakApp.Core.Dtos.User.Requests;
global using MosefakApp.Core.IServices.Authentication;
global using MosefakApp.Core.IServices.Email;
global using MosefakApp.Domains.Entities.Identity;
global using MosefakApp.Infrastructure.constants;
global using MosefakApp.Shared.Exceptions.Base;
global using System.Security.Cryptography;
global using System.Text;
global using BadRequest = MosefakApp.Shared.Exceptions.Base.BadRequest;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;
global using MosefakApp.Core.IServices.Cache;
global using StackExchange.Redis;
global using Microsoft.AspNetCore.Hosting;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;
global using AutoMapper;
global using MosefakApp.Core.Dtos.Role.Request;
global using MosefakApp.Core.Dtos.Role.Responses;
global using MosefakApp.Core.IServices.Role;
global using MosefakApp.Infrastructure.Identity.context;
global using MosefakApp.Core.Dtos.User.Responses;
global using MosefakApp.Core.IServices.User;
global using MosefakApp.Core.Dtos.ClinicAddress.Responses;
global using MosefakApp.Core.Dtos.Doctor.Requests;
global using MosefakApp.Core.Dtos.Doctor.Responses;
global using MosefakApp.Core.Dtos.Review.Responses;
global using MosefakApp.Core.Dtos.Schedule.Responses;
global using MosefakApp.Core.Dtos.Specialization.Responses;
global using MosefakApp.Core.IServices;
global using MosefakApp.Core.IUnit;
global using MosefakApp.Domains.Entities;
global using MosefakApp.Core.IRepositories.Non_Generic;
global using MosefakApp.Core.Dtos.Appointment.Responses;
global using MosefakApp.Domains.Enums;
global using MosefakApp.Core.Dtos.Appointment.Requests;
global using MosefakApp.Core.IServices.Image;
global using Microsoft.Extensions.Configuration;
global using System.Transactions;
global using MosefakApp.Core.IServices.Data_Protection;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.Extensions.Caching.Memory;
global using MosefakApp.Core.IServices.Logging;







