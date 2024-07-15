global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.IO.Abstractions;
global using System.Net;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using AnthropicClient;
global using AnthropicClient.Models;

global using Codeblaze.SemanticKernel.Connectors.Ollama;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;
global using Microsoft.Playwright;
global using Microsoft.SemanticKernel.Connectors.Chroma;
global using Microsoft.SemanticKernel.Embeddings;
global using Microsoft.SemanticKernel.Memory;
global using Microsoft.SemanticKernel.Text;

global using OnxAdmin.API.Agents;
global using OnxAdmin.API.Diagnostics;
global using OnxAdmin.API.Extensions;
global using OnxAdmin.API.Factories;
global using OnxAdmin.API.Json;
global using OnxAdmin.API.Middleware;
global using OnxAdmin.API.Models;
global using OnxAdmin.API.Options;
global using OnxAdmin.API.Requests;
global using OnxAdmin.API.Services;

global using ReverseMarkdown;
