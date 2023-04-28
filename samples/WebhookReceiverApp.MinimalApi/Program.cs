using Deveel.Webhooks;
using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Models;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddWebhookReceiver<IdentityWebhook>()
	.Configure(options => {
		options.VerifySignature = true;
		options.Signature.Location = WebhookSignatureLocation.QueryString;
		options.Signature.ParameterName = "sig";
		options.Signature.Secret = builder.Configuration["Receiver:Secret"];
	})
	.AddHandler<UserRegisteredHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
	app.UseHttpsRedirection();

app.UseWebhookReceiver<IdentityWebhook>("/webhooks/identity");

app.Run();