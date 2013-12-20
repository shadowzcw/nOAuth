using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DotNetOpenAuth.Messaging.Bindings;

namespace nOAuth.AuthorizationServer.Infrastructure
{
    /// <summary>
    /// A database-persisted nonce store.
    /// </summary>
    public class DatabaseKeyNonceStore : INonceStore, ICryptoKeyStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseKeyNonceStore"/> class.
        /// </summary>
        public DatabaseKeyNonceStore()
        {
        }

        #region INonceStore Members

        /// <summary>
        /// Stores a given nonce and timestamp.
        /// </summary>
        /// <param name="context">The context, or namespace, within which the
        /// <paramref name="nonce"/> must be unique.
        /// The context SHOULD be treated as case-sensitive.
        /// The value will never be <c>null</c> but may be the empty string.</param>
        /// <param name="nonce">A series of random characters.</param>
        /// <param name="timestampUtc">The UTC timestamp that together with the nonce string make it unique
        /// within the given <paramref name="context"/>.
        /// The timestamp may also be used by the data store to clear out old nonces.</param>
        /// <returns>
        /// True if the context+nonce+timestamp (combination) was not previously in the database.
        /// False if the nonce was stored previously with the same timestamp and context.
        /// </returns>
        /// <remarks>
        /// The nonce must be stored for no less than the maximum time window a message may
        /// be processed within before being discarded as an expired message.
        /// This maximum message age can be looked up via the
        /// <see cref="DotNetOpenAuth.Configuration.MessagingElement.MaximumMessageLifetime"/>
        /// property, accessible via the <see cref="Configuration"/>
        /// property.
        /// </remarks>
        public bool StoreNonce(string context, string nonce, DateTime timestampUtc)
        {
            var db = new OAuthEntities();
            db.oauth_nonce.AddObject(new oauth_nonce() { Context = context, Code = nonce, Timestamp = timestampUtc });
            try
            {
                db.SaveChanges();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        #endregion

        #region ICryptoKeyStore Members

        public CryptoKey GetKey(string bucket, string handle)
        {
            //var db = new OAuthEntities();
            //var _db = db.symmetriccryptokeys.Where(k => k.Bucket == bucket && k.Handle == handle).ToList();
            //// Perform a case senstive match
            //var matches = from key in _db
            //              where string.Equals(key.Bucket, bucket, StringComparison.Ordinal) &&
            //              string.Equals(key.Handle, handle, StringComparison.Ordinal)
            //              select new CryptoKey(key.Secret, key.ExpiresUtc.AsUtc());
            //return matches.FirstOrDefault();
            var db = new OAuthEntities();
            var cryptoKey = db.oauth_symmetriccryptokey.Where(c => c.Bucket == bucket && c.Handle == handle).OrderByDescending(c => c.ExpiresUtc).FirstOrDefault();
            return new CryptoKey(cryptoKey.Secret, cryptoKey.ExpiresUtc.AsUtc());
        }

        public IEnumerable<KeyValuePair<string, CryptoKey>> GetKeys(string bucket)
        {
            var db = new OAuthEntities();
            //return from key in db.symmetriccryptokeys
            //       where key.Bucket == bucket
            //       orderby key.ExpiresUtc descending
            //       select new KeyValuePair<string, CryptoKey>(key.Handle, new CryptoKey(key.Secret, key.ExpiresUtc.AsUtc()));

            //var db = new OAuthEntities();
            //var cryptoKey = db.symmetriccryptokeys.Where(c => c.Bucket == bucket).OrderByDescending(c => c.ExpiresUtc);
            //return cryptoKey.Select(c => new KeyValuePair<string, CryptoKey>(c.Handle, new CryptoKey(c.Secret, c.ExpiresUtc.AsUtc())));

            var matches = from key in db.oauth_symmetriccryptokey
                          where key.Bucket == bucket
                          orderby key.ExpiresUtc descending
                          select key;

            List<KeyValuePair<string, CryptoKey>> en = new List<KeyValuePair<string, CryptoKey>>();

            foreach (var key in matches)
                en.Add(new KeyValuePair<string, CryptoKey>(key.Handle, new CryptoKey(key.Secret, key.ExpiresUtc.AsUtc())));

            return en.AsEnumerable<KeyValuePair<string, CryptoKey>>();
        }

        public void StoreKey(string bucket, string handle, CryptoKey key)
        {
            var keyRow = new oauth_symmetriccryptokey()
            {
                Bucket = bucket,
                Handle = handle,
                Secret = key.Key,
                ExpiresUtc = key.ExpiresUtc,
            };
            var db = new OAuthEntities();
            db.oauth_symmetriccryptokey.AddObject(keyRow);
            db.SaveChanges();
        }

        public void RemoveKey(string bucket, string handle)
        {
            var db = new OAuthEntities();
            var match = db.oauth_symmetriccryptokey.FirstOrDefault(k => k.Bucket == bucket && k.Handle == handle);
            if (match != null)
            {
                db.oauth_symmetriccryptokey.DeleteObject(match);
            }
        }

        #endregion
    }
}