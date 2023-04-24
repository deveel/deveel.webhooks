using MongoDB.Driver;

using MongoFramework;

namespace Deveel.Webhooks {
    /// <summary>
    /// 
    /// </summary>
    public sealed class MongoDbWebhookOptions {
        /// <summary>
        /// Gets or sets the connection string to the MongoDB database,
        /// when not in a multi-tenant context.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// A default name of the database to use when not
        /// provided in the connection string.
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Builds a URL to a mongo server using the connection string
        /// provided and the configurations set in this object.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to the MongoDB database.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="MongoUrl"/> that is
        /// used to connect to the database server.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public MongoUrl BuildMongoUrl(string connectionString) {
            if (String.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("The connection string to the MongoDB database is not specified");

            var url = new MongoUrlBuilder(connectionString);

            if (!String.IsNullOrWhiteSpace(DatabaseName) &&
                String.IsNullOrWhiteSpace(url.DatabaseName)) {
                url.DatabaseName = DatabaseName;
            }

            if (String.IsNullOrWhiteSpace(url.DatabaseName))
                throw new ArgumentException("The name of the database is not specified");

            return url.ToMongoUrl();
        }

        /// <summary>
        /// Builds a URL to a mongo server using the configurations
        /// provided by this object.
        /// </summary>
        /// <returns>
        /// Returns an instance of <see cref="MongoUrl"/> that is
        /// configured from the options set
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the connection string to the database is not specified or
        /// if the name of the database is not specified.
        /// </exception>
        public MongoUrl BuildMongoUrl() {
            if (String.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentException("The connection string to the MongoDB database is not specified");

            return BuildMongoUrl(ConnectionString);
        }

        /// <summary>
        /// Builds a connection to a mongo server using the configurations
        /// provided by this object.
        /// </summary>
        /// <returns>
        /// Returns a new instance of <see cref="MongoDbConnection"/> that
        /// is used to connect to the database server.
        /// </returns>
        public MongoDbConnection BuildConnection() => MongoDbConnection.FromUrl(BuildMongoUrl());

        /// <summary>
        /// Builds a connection to a mongo server using the connection string
        /// provided and the configurations set in this object.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to the MongoDB database.
        /// </param>
        /// <returns>
        /// Returns a new instance of <see cref="MongoDbConnection"/> that
        /// is used to connect to the database server.
        /// </returns>
        public MongoDbConnection BuildConnection(string connectionString) => MongoDbConnection.FromUrl(BuildMongoUrl(connectionString));
    }
}
