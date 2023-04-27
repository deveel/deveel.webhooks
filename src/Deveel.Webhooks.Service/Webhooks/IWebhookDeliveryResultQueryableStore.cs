﻿// Copyright 2022-2023 Deveel
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

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides a queryable store for the <see cref="IWebhookDeliveryResultStore{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IWebhookDeliveryResultQueryableStore<TResult> : IWebhookDeliveryResultStore<TResult>
        where TResult : class, IWebhookDeliveryResult {
        /// <summary>
        /// Gets a queryable object that can be used to query the store.
        /// </summary>
        /// <returns>
        /// Returns an instance of <see cref="IQueryable{TResult}"/> that can be used
        /// to query the store.
        /// </returns>
        IQueryable<TResult> AsQueryable();
    }
}
