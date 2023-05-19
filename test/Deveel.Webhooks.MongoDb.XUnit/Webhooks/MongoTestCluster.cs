// Copyright 2022-2023 Deveel
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

using System.Text;

using EphemeralMongo;

namespace Deveel.Webhooks {
	public class MongoTestCluster : IDisposable {
		private readonly IMongoRunner mongo;

		public MongoTestCluster() {
			mongo = MongoRunner.Run(new MongoRunnerOptions {
				KillMongoProcessesWhenCurrentProcessExits = true
			});

			var connString = new StringBuilder(mongo.ConnectionString);
			if (!connString[^1].Equals('/'))
				connString.Append('/');
			
			ConnectionString = connString.ToString();
		}

		public string ConnectionString { get; }

		public void Dispose() {
			mongo?.Dispose();
		}
	}
}
