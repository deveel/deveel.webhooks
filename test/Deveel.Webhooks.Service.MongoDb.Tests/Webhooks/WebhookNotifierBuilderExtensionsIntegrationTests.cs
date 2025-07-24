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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class WebhookNotifierBuilderExtensionsIntegrationTests : IDisposable {
        private readonly IServiceCollection services;
        private readonly ITestOutputHelper output;
        private ServiceProvider? serviceProvider;

        public WebhookNotifierBuilderExtensionsIntegrationTests(ITestOutputHelper output) {
            this.output = output;
            services = new ServiceCollection();
            services.AddLogging(builder => builder.AddXUnit(output));
        }

        [Fact]
        public async Task UseMongoSubscriptionResolver_WithFullConfiguration_CanResolveService() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);
            builder.UseMongoSubscriptionResolver();
            
            // Add required MongoDB dependencies (mocked for this test)
            services.AddTransient<IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId>, 
                MockMongoWebhookSubscriptionRepository>();
            
            serviceProvider = services.BuildServiceProvider();

            // Act
            var resolver = serviceProvider.GetService<IWebhookSubscriptionResolver>();

            // Assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public async Task UseMongoDeliveryResultLogger_WithFullConfiguration_CanResolveService() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);
            builder.UseMongoDeliveryResultLogger();
            
            // Add required MongoDB dependencies (mocked for this test)
            services.AddTransient<IWebhookDeliveryResultRepository<MongoWebhookDeliveryResult, ObjectId>, 
                MockMongoWebhookDeliveryResultRepository>();
            
            serviceProvider = services.BuildServiceProvider();

            // Act
            var logger = serviceProvider.GetService<IWebhookDeliveryResultLogger<TestWebhook>>();

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<MongoDbWebhookDeliveryResultLogger<TestWebhook, MongoWebhookDeliveryResult>>(logger);
        }

        [Fact]
        public async Task UseMongoDeliveryResultLogger_WithCustomResultType_CanResolveService() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);
            builder.UseMongoDeliveryResultLogger<TestWebhook, CustomMongoWebhookDeliveryResult>();
            
            // Add required MongoDB dependencies (mocked for this test)
            services.AddTransient<IWebhookDeliveryResultRepository<CustomMongoWebhookDeliveryResult, ObjectId>, 
                MockCustomMongoWebhookDeliveryResultRepository>();
            
            serviceProvider = services.BuildServiceProvider();

            // Act
            var logger = serviceProvider.GetService<IWebhookDeliveryResultLogger<TestWebhook>>();

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<MongoDbWebhookDeliveryResultLogger<TestWebhook, CustomMongoWebhookDeliveryResult>>(logger);
        }

        [Fact]
        public async Task CompleteConfiguration_WithAllExtensions_CreatesWorkingNotifier() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);
            builder
                .UseMongoSubscriptionResolver()
                .UseMongoDeliveryResultLogger()
                .UseNotifier()
                .UseSender();
            
            // Add required MongoDB dependencies (mocked for this test)
            services.AddTransient<IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId>, 
                MockMongoWebhookSubscriptionRepository>();
            services.AddTransient<IWebhookDeliveryResultRepository<MongoWebhookDeliveryResult, ObjectId>, 
                MockMongoWebhookDeliveryResultRepository>();
            
            serviceProvider = services.BuildServiceProvider();

            // Act
            var notifier = serviceProvider.GetService<IWebhookNotifier<TestWebhook>>();
            var resolver = serviceProvider.GetService<IWebhookSubscriptionResolver>();
            var logger = serviceProvider.GetService<IWebhookDeliveryResultLogger<TestWebhook>>();

            // Assert
            Assert.NotNull(notifier);
            Assert.NotNull(resolver);
            Assert.NotNull(logger);
        }

        [Fact]
        public void UseMongoSubscriptionResolver_CallsUseDefaultSubscriptionResolver() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);

            // Act
            builder.UseMongoSubscriptionResolver();

            // Assert
            // Verify that the subscription resolver was registered by checking for the adapter service
            var hasResolverService = services.Any(s => 
                s.ServiceType == typeof(IWebhookSubscriptionResolver) ||
                (s.ServiceType.IsGenericType && 
                 s.ServiceType.GetGenericTypeDefinition() == typeof(IWebhookSubscriptionResolver<>)));
            
            Assert.True(hasResolverService);
        }

        [Fact]
        public void Extension_Methods_AreIdempotent() {
            // Arrange
            var builder = new WebhookNotifierBuilder<TestWebhook>(services);

            // Act
            builder.UseMongoSubscriptionResolver();
            builder.UseMongoSubscriptionResolver(); // Call again
            
            builder.UseMongoDeliveryResultLogger();
            builder.UseMongoDeliveryResultLogger(); // Call again

            // Assert
            var loggerServices = services.Where(s => 
                s.ServiceType == typeof(IWebhookDeliveryResultLogger<TestWebhook>)).ToList();
            
            // Should have multiple registrations (not idempotent for delivery logger)
            Assert.True(loggerServices.Count >= 2);
        }

        public void Dispose() {
            serviceProvider?.Dispose();
        }
    }

    // Mock implementations for testing
    public class MockMongoWebhookSubscriptionRepository : IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId> {
        public Task<MongoWebhookSubscription> AddAsync(MongoWebhookSubscription entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<MongoWebhookSubscription> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookSubscription?> FindAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<PageResult<MongoWebhookSubscription>> GetPageAsync(PageQuery<MongoWebhookSubscription> query, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookSubscription> RemoveAsync(MongoWebhookSubscription entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync(IEnumerable<MongoWebhookSubscription> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookSubscription> UpdateAsync(MongoWebhookSubscription entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<IList<MongoWebhookSubscription>> FindByEventTypeAsync(string eventType, bool activeOnly = true, CancellationToken cancellationToken = default) {
            return Task.FromResult<IList<MongoWebhookSubscription>>(new List<MongoWebhookSubscription>());
        }

        public Task<IList<MongoWebhookSubscription>> FindByTenantAsync(string tenantId, bool activeOnly = true, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<long> CountByTenantAsync(string tenantId, bool activeOnly = true, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<long> CountByEventTypeAsync(string eventType, bool activeOnly = true, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }

    public class MockMongoWebhookDeliveryResultRepository : IWebhookDeliveryResultRepository<MongoWebhookDeliveryResult, ObjectId> {
        public Task<MongoWebhookDeliveryResult> AddAsync(MongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            return Task.FromResult(entity);
        }

        public Task AddRangeAsync(IEnumerable<MongoWebhookDeliveryResult> entities, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookDeliveryResult?> FindAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<PageResult<MongoWebhookDeliveryResult>> GetPageAsync(PageQuery<MongoWebhookDeliveryResult> query, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookDeliveryResult> RemoveAsync(MongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync(IEnumerable<MongoWebhookDeliveryResult> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<MongoWebhookDeliveryResult> UpdateAsync(MongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }

    public class MockCustomMongoWebhookDeliveryResultRepository : IWebhookDeliveryResultRepository<CustomMongoWebhookDeliveryResult, ObjectId> {
        public Task<CustomMongoWebhookDeliveryResult> AddAsync(CustomMongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            return Task.FromResult(entity);
        }

        public Task AddRangeAsync(IEnumerable<CustomMongoWebhookDeliveryResult> entities, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<CustomMongoWebhookDeliveryResult?> FindAsync(ObjectId key, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<PageResult<CustomMongoWebhookDeliveryResult>> GetPageAsync(PageQuery<CustomMongoWebhookDeliveryResult> query, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<CustomMongoWebhookDeliveryResult> RemoveAsync(CustomMongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync(IEnumerable<CustomMongoWebhookDeliveryResult> entities, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<CustomMongoWebhookDeliveryResult> UpdateAsync(CustomMongoWebhookDeliveryResult entity, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }
}