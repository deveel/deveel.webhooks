// Copyright 2022-2024 Antonello Provenzano
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Xunit;

namespace Deveel.Webhooks {
    public class WebhookNotifierBuilderExtensionsTests {
        private readonly IServiceCollection services;
        private readonly WebhookNotifierBuilder<TestWebhook> builder;

        public WebhookNotifierBuilderExtensionsTests() {
            services = new ServiceCollection();
            builder = new WebhookNotifierBuilder<TestWebhook>(services);
        }

        [Fact]
        public void UseMongoSubscriptionResolver_WithValidBuilder_ReturnsBuilderInstance() {
            // Act
            var result = builder.UseMongoSubscriptionResolver();

            // Assert
            Assert.NotNull(result);
            Assert.Same(builder, result);
        }

        [Fact]
        public void UseMongoSubscriptionResolver_WithValidBuilder_RegistersMongoWebhookSubscriptionType() {
            // Act
            builder.UseMongoSubscriptionResolver();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var resolverService = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IWebhookSubscriptionResolver) ||
                s.ServiceType.IsGenericType && 
                s.ServiceType.GetGenericTypeDefinition() == typeof(IWebhookSubscriptionResolver<>));
            
            Assert.NotNull(resolverService);
        }

        [Fact]
        public void UseMongoSubscriptionResolver_WithNullBuilder_ThrowsArgumentNullException() {
            // Arrange
            WebhookNotifierBuilder<TestWebhook> nullBuilder = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => nullBuilder.UseMongoSubscriptionResolver());
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithGenericResult_ReturnsBuilderInstance() {
            // Act
            var result = builder.UseMongoDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>();

            // Assert
            Assert.NotNull(result);
            Assert.Same(builder, result);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithGenericResult_RegistersCorrectService() {
            // Act
            builder.UseMongoDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>();

            // Assert
            var serviceDescriptor = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>) &&
                s.ImplementationType == typeof(MongoDbWebhookDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>));
            
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithGenericResult_AllowsMultipleRegistrations() {
            // Act
            builder.UseMongoDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>();
            builder.UseMongoDeliveryResultLogger<TestWebhook, CustomMongoWebhookDeliveryResult>();

            // Assert
            var serviceDescriptors = services.Where(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>)).ToList();
            
            Assert.Equal(2, serviceDescriptors.Count);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithDefaultResult_ReturnsBuilderInstance() {
            // Act
            var result = builder.UseMongoDeliveryResultLogger();

            // Assert
            Assert.NotNull(result);
            Assert.Same(builder, result);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithDefaultResult_RegistersCorrectService() {
            // Act
            builder.UseMongoDeliveryResultLogger();

            // Assert
            var serviceDescriptor = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>) &&
                s.ImplementationType == typeof(MongoDbWebhookDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>));
            
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithNullBuilder_ThrowsArgumentNullException() {
            // Arrange
            WebhookNotifierBuilder<TestWebhook> nullBuilder = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => nullBuilder.UseMongoDeliveryResultLogger());
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithGenericResultAndNullBuilder_ThrowsArgumentNullException() {
            // Arrange
            WebhookNotifierBuilder<TestWebhook> nullBuilder = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                nullBuilder.UseMongoDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>());
        }

        [Theory]
        [InlineData(typeof(TestWebhook))]
        [InlineData(typeof(CustomWebhook))]
        public void UseMongoSubscriptionResolver_WithDifferentWebhookTypes_WorksCorrectly(Type webhookType) {
            // Arrange
            var builderType = typeof(WebhookNotifierBuilder<>).MakeGenericType(webhookType);
            var builder = Activator.CreateInstance(builderType, services);
            var method = typeof(WebhookNotifierBuilderExtensions)
                .GetMethod(nameof(WebhookNotifierBuilderExtensions.UseMongoSubscriptionResolver))!
                .MakeGenericMethod(webhookType);

            // Act
            var result = method.Invoke(null, new[] { builder });

            // Assert
            Assert.NotNull(result);
            Assert.Same(builder, result);
        }

        [Theory]
        [InlineData(typeof(TestWebhook), typeof(MongoWebhookDeliveryResult))]
        [InlineData(typeof(CustomWebhook), typeof(CustomMongoWebhookDeliveryResult))]
        public void UseMongoDeliveryResultLogger_WithDifferentTypes_RegistersCorrectly(
            Type webhookType, Type resultType) {
            // Arrange
            var services = new ServiceCollection();
            var builderType = typeof(WebhookNotifierBuilder<>).MakeGenericType(webhookType);
            var builder = Activator.CreateInstance(builderType, services);
            var method = typeof(WebhookNotifierBuilderExtensions)
                .GetMethod(nameof(WebhookNotifierBuilderExtensions.UseMongoDeliveryResultLogger), 
                    new[] { builderType })!
                .MakeGenericMethod(webhookType, resultType);

            // Act
            method.Invoke(null, new[] { builder });

            // Assert
            var loggerServiceType = typeof(IWebhookDeliveryResultLogger<>).MakeGenericType(webhookType);
            var implementationType = typeof(MongoDbWebhookDeliveryResultLogger<,>)
                .MakeGenericType(webhookType, resultType);
            
            var serviceDescriptor = services.FirstOrDefault(s => 
                s.ServiceType == loggerServiceType && 
                s.ImplementationType == implementationType);
            
            Assert.NotNull(serviceDescriptor);
        }

        [Fact]
        public void UseMongoSubscriptionResolver_CanBeChainedWithOtherMethods() {
            // Act
            var result = builder
                .UseMongoSubscriptionResolver()
                .UseMongoDeliveryResultLogger()
                .UseMongoDeliveryResultLogger<TestWebhook, CustomMongoWebhookDeliveryResult>();

            // Assert
            Assert.NotNull(result);
            Assert.Same(builder, result);
            
            // Verify all services are registered
            var subscriptionResolverService = services.Any(s => 
                s.ServiceType == typeof(IWebhookSubscriptionResolver) ||
                (s.ServiceType.IsGenericType && 
                 s.ServiceType.GetGenericTypeDefinition() == typeof(IWebhookSubscriptionResolver<>)));
            
            var loggerServices = services.Where(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>)).ToList();

            Assert.True(subscriptionResolverService);
            Assert.Equal(2, loggerServices.Count);
        }

        [Fact]
        public void UseMongoDeliveryResultLogger_WithCustomResultType_AllowsInheritance() {
            // Act
            builder.UseMongoDeliveryResultLogger<TestWebhook, ExtendedMongoWebhookDeliveryResult>();

            // Assert
            var serviceDescriptor = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>) &&
                s.ImplementationType == typeof(MongoDbWebhookDeliveryResultLogger<TestWebhook, ExtendedMongoWebhookDeliveryResult>));
            
            Assert.NotNull(serviceDescriptor);
        }

        [Fact]
        public void Extension_Methods_AreStatic() {
            // Assert
            var type = typeof(WebhookNotifierBuilderExtensions);
            Assert.True(type.IsAbstract);
            Assert.True(type.IsSealed);
        }

        [Fact]
        public void Extension_Methods_HaveCorrectSignatures() {
            // Arrange
            var type = typeof(WebhookNotifierBuilderExtensions);
            
            // Act
            var useMongoSubscriptionResolver = type.GetMethod("UseMongoSubscriptionResolver");
            var useMongoDeliveryResultLoggerGeneric = type.GetMethods()
                .FirstOrDefault(m => m.Name == "UseMongoDeliveryResultLogger" && 
                                   m.GetGenericArguments().Length == 2);
            var useMongoDeliveryResultLoggerDefault = type.GetMethods()
                .FirstOrDefault(m => m.Name == "UseMongoDeliveryResultLogger" && 
                                   m.GetGenericArguments().Length == 1);

            // Assert
            Assert.NotNull(useMongoSubscriptionResolver);
            Assert.NotNull(useMongoDeliveryResultLoggerGeneric);
            Assert.NotNull(useMongoDeliveryResultLoggerDefault);
            
            Assert.True(useMongoSubscriptionResolver.IsStatic);
            Assert.True(useMongoDeliveryResultLoggerGeneric.IsStatic);
            Assert.True(useMongoDeliveryResultLoggerDefault.IsStatic);
        }
    }

    // Test helper classes
    public class TestWebhook {
        public string Id { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    public class CustomWebhook {
        public string Identifier { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }

    public class CustomMongoWebhookDeliveryResult : MongoWebhookDeliveryResult {
        public string CustomProperty { get; set; } = string.Empty;
    }

    public class ExtendedMongoWebhookDeliveryResult : MongoWebhookDeliveryResult {
        public DateTimeOffset ProcessedAt { get; set; }
        public string ProcessingNotes { get; set; } = string.Empty;
    }
}